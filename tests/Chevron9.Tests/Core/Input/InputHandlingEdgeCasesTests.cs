using Chevron9.Core.Data.Input;
using Chevron9.Core.Interfaces;
using Chevron9.Core.Types;
using Chevron9.Shared.Primitives;
using static Chevron9.Core.Data.Input.InputKeys;

namespace Chevron9.Tests.Core.Input;

[TestFixture]
public class InputHandlingEdgeCasesTests
{
    private sealed class TestInputDevice : IInputDevice
    {
        private readonly Dictionary<InputKey, bool> _keyStates = new();
        private readonly Dictionary<InputKey, bool> _pressedStates = new();
        private readonly Dictionary<InputKey, bool> _releasedStates = new();
        private readonly Dictionary<(InputKey, InputKeyModifierType), bool> _comboPressedStates = new();
        private readonly Dictionary<(InputKey, InputKeyModifierType), bool> _comboReleasedStates = new();
        private InputKeyModifierType _activeModifiers;
        private Position _mousePosition;
        private readonly Dictionary<MouseButtonType, bool> _mouseButtonStates = new();
        private readonly Dictionary<MouseButtonType, bool> _mouseClickedStates = new();
        private Position _mouseWheelDelta;

        public void Poll() { }

        public bool IsDown(InputKey key) => _keyStates.GetValueOrDefault(key, false);

        public bool WasPressed(InputKey key) => _pressedStates.GetValueOrDefault(key, false);

        public bool WasReleased(InputKey key) => _releasedStates.GetValueOrDefault(key, false);

        public bool IsDown(InputKey key, InputKeyModifierType modifiers)
        {
            return _keyStates.GetValueOrDefault(key, false) &&
                   (_activeModifiers & modifiers) == modifiers;
        }

        public bool WasPressed(InputKey key, InputKeyModifierType modifiers)
        {
            return _comboPressedStates.GetValueOrDefault((key, modifiers), false);
        }

        public bool WasReleased(InputKey key, InputKeyModifierType modifiers)
        {
            return _comboReleasedStates.GetValueOrDefault((key, modifiers), false);
        }

        public InputKeyModifierType GetActiveModifiers() => _activeModifiers;

        public Position MousePosition() => _mousePosition;

        public bool MouseDown(MouseButtonType button) => _mouseButtonStates.GetValueOrDefault(button, false);

        public bool MouseClicked(MouseButtonType button) => _mouseClickedStates.GetValueOrDefault(button, false);

        public Position MouseWheelDelta() => _mouseWheelDelta;

        // Helper methods for test setup
        public void SetKeyState(InputKey key, bool isDown) => _keyStates[key] = isDown;
        public void SetKeyPressed(InputKey key, bool pressed) => _pressedStates[key] = pressed;
        public void SetKeyReleased(InputKey key, bool released) => _releasedStates[key] = released;
        public void SetModifiers(InputKeyModifierType modifiers) => _activeModifiers = modifiers;
        public void SetMousePosition(Position position) => _mousePosition = position;
        public void SetMouseButton(MouseButtonType button, bool isDown) => _mouseButtonStates[button] = isDown;
        public void SetMouseClicked(MouseButtonType button, bool clicked) => _mouseClickedStates[button] = clicked;
        public void SetMouseWheelDelta(Position delta) => _mouseWheelDelta = delta;
        public void SetComboPressed(InputKey key, InputKeyModifierType modifiers, bool pressed)
            => _comboPressedStates[(key, modifiers)] = pressed;
        public void SetComboReleased(InputKey key, InputKeyModifierType modifiers, bool released)
            => _comboReleasedStates[(key, modifiers)] = released;
    }

    private sealed class TestScene : IScene
    {
        public string Name { get; }
        public bool HandleInputCalled { get; private set; }
        public IInputDevice? LastInputDevice { get; private set; }
        public bool ShouldConsumeInput { get; set; }
        public IReadOnlyList<ILayer> Layers => Array.Empty<ILayer>();

        public TestScene(string name)
        {
            Name = name;
        }

        public void Enter() { }
        public void Close() { }
        public void Update(double dt, IInputDevice input) { }
        public void Render(IRenderCommandCollector rq, float alpha) { }

        public bool HandleInput(IInputDevice input)
        {
            HandleInputCalled = true;
            LastInputDevice = input;
            return ShouldConsumeInput;
        }

        public void Dispose() { }
    }

    [Test]
    public void InputDevice_KeyStateChanges_AreHandledCorrectly()
    {
        var input = new TestInputDevice();

        // Initially no keys are down
        Assert.That(input.IsDown(A), Is.False);
        Assert.That(input.WasPressed(A), Is.False);
        Assert.That(input.WasReleased(A), Is.False);

        // Simulate key press
        input.SetKeyState(A, true);
        input.SetKeyPressed(A, true);

        Assert.That(input.IsDown(A), Is.True);
        Assert.That(input.WasPressed(A), Is.True);
        Assert.That(input.WasReleased(A), Is.False);

        // Simulate key release
        input.SetKeyState(A, false);
        input.SetKeyPressed(A, false);
        input.SetKeyReleased(A, true);

        Assert.That(input.IsDown(A), Is.False);
        Assert.That(input.WasPressed(A), Is.False);
        Assert.That(input.WasReleased(A), Is.True);
    }

    [Test]
    public void InputDevice_ModifierKeys_AreHandledCorrectly()
    {
        var input = new TestInputDevice();

        // Set Ctrl modifier
        input.SetModifiers(InputKeyModifierType.Control);

        // Test Ctrl+A combination
        input.SetKeyState(A, true);
        input.SetComboPressed(A, InputKeyModifierType.Control, true);

        Assert.That(input.IsDown(A, InputKeyModifierType.Control), Is.True);
        Assert.That(input.WasPressed(A, InputKeyModifierType.Control), Is.True);

        // Test with wrong modifier
        Assert.That(input.IsDown(A, InputKeyModifierType.Shift), Is.False);
        Assert.That(input.WasPressed(A, InputKeyModifierType.Shift), Is.False);
    }

    [Test]
    public void InputDevice_MouseStateChanges_AreHandledCorrectly()
    {
        var input = new TestInputDevice();

        // Set mouse position
        var position = new Position(100, 200);
        input.SetMousePosition(position);

        Assert.That(input.MousePosition(), Is.EqualTo(position));

        // Test mouse button
        input.SetMouseButton(MouseButtonType.Left, true);
        input.SetMouseClicked(MouseButtonType.Left, true);

        Assert.That(input.MouseDown(MouseButtonType.Left), Is.True);
        Assert.That(input.MouseClicked(MouseButtonType.Left), Is.True);

        // Test mouse wheel
        var wheelDelta = new Position(0, 120);
        input.SetMouseWheelDelta(wheelDelta);

        Assert.That(input.MouseWheelDelta(), Is.EqualTo(wheelDelta));
    }

    [Test]
    public void InputDevice_MultipleModifiers_WorkCorrectly()
    {
        var input = new TestInputDevice();

        // Set Ctrl + Shift
        input.SetModifiers(InputKeyModifierType.Control | InputKeyModifierType.Shift);
        input.SetKeyState(C, true);
        input.SetComboPressed(C, InputKeyModifierType.Control | InputKeyModifierType.Shift, true);

        // Should work with both modifiers
        Assert.That(input.IsDown(C, InputKeyModifierType.Control | InputKeyModifierType.Shift), Is.True);
        Assert.That(input.WasPressed(C, InputKeyModifierType.Control | InputKeyModifierType.Shift), Is.True);

        // Should work with single modifier that's active
        Assert.That(input.IsDown(C, InputKeyModifierType.Control), Is.True);
        // Note: WasPressed with modifiers checks exact combo, not subset
        Assert.That(input.WasPressed(C, InputKeyModifierType.Control | InputKeyModifierType.Shift), Is.True);
    }

    [Test]
    public void InputDevice_UnknownKeys_ReturnFalse()
    {
        var input = new TestInputDevice();

        // Test with uninitialized key
        Assert.That(input.IsDown(new InputKey("Unknown", 999)), Is.False);
        Assert.That(input.WasPressed(new InputKey("Unknown", 999)), Is.False);
        Assert.That(input.WasReleased(new InputKey("Unknown", 999)), Is.False);
    }

    [Test]
    public void InputDevice_UnknownMouseButtons_ReturnFalse()
    {
        var input = new TestInputDevice();

        // Test with uninitialized mouse button
        Assert.That(input.MouseDown(unchecked((MouseButtonType)999)), Is.False);
        Assert.That(input.MouseClicked(unchecked((MouseButtonType)999)), Is.False);
    }

    [Test]
    public void InputDevice_EdgeCase_KeyPressedAndReleasedSameFrame()
    {
        var input = new TestInputDevice();

        // Simulate key pressed and released in same frame (edge case)
        input.SetKeyState(Space, false); // Key is up
        input.SetKeyPressed(Space, true); // But was pressed
        input.SetKeyReleased(Space, true); // And was released

        Assert.That(input.IsDown(Space), Is.False);
        Assert.That(input.WasPressed(Space), Is.True);
        Assert.That(input.WasReleased(Space), Is.True);
    }

    [Test]
    public void InputDevice_EdgeCase_AllModifiersActive()
    {
        var input = new TestInputDevice();

        // Set all possible modifiers
        var allModifiers = InputKeyModifierType.Control | InputKeyModifierType.Shift |
                           InputKeyModifierType.Alt | InputKeyModifierType.Super;
        input.SetModifiers(allModifiers);

        Assert.That(input.GetActiveModifiers(), Is.EqualTo(allModifiers));
    }

    [Test]
    public void InputDevice_EdgeCase_NoModifiers()
    {
        var input = new TestInputDevice();

        // No modifiers active
        input.SetModifiers(InputKeyModifierType.None);

        Assert.That(input.GetActiveModifiers(), Is.EqualTo(InputKeyModifierType.None));
    }

    [Test]
    public void InputDevice_EdgeCase_InvalidModifierCombination()
    {
        var input = new TestInputDevice();

        // Set invalid modifier combination
        input.SetModifiers((InputKeyModifierType)999);

        // Should still work for basic key checks
        input.SetKeyState(X, true);
        Assert.That(input.IsDown(X), Is.True);
    }

    [Test]
    public void SceneManager_InputHandling_WithNullInput_DoesNotThrow()
    {
        using var manager = new Chevron9.Core.Scenes.SceneManager();
        var scene = new TestScene("TestScene");
        manager.Push(scene);

        Assert.DoesNotThrow(() => manager.HandleInput(null!));
    }

    [Test]
    public void SceneManager_InputHandling_WithConsumingScene_ReturnsTrue()
    {
        using var manager = new Chevron9.Core.Scenes.SceneManager();
        var scene = new TestScene("TestScene") { ShouldConsumeInput = true };
        var input = new TestInputDevice();
        manager.Push(scene);

        var result = manager.HandleInput(input);

        Assert.That(result, Is.True);
        Assert.That(scene.HandleInputCalled, Is.True);
        Assert.That(scene.LastInputDevice, Is.EqualTo(input));
    }

    [Test]
    public void SceneManager_InputHandling_WithNonConsumingScene_ReturnsFalse()
    {
        using var manager = new Chevron9.Core.Scenes.SceneManager();
        var scene = new TestScene("TestScene") { ShouldConsumeInput = false };
        var input = new TestInputDevice();
        manager.Push(scene);

        var result = manager.HandleInput(input);

        Assert.That(result, Is.False);
        Assert.That(scene.HandleInputCalled, Is.True);
    }

    [Test]
    public void SceneManager_InputHandling_WithMultipleScenes_OnlyTopSceneReceivesInput()
    {
        using var manager = new Chevron9.Core.Scenes.SceneManager();
        var scene1 = new TestScene("Scene1");
        var scene2 = new TestScene("Scene2");
        var input = new TestInputDevice();

        manager.Push(scene1);
        manager.Push(scene2);

        manager.HandleInput(input);

        Assert.That(scene1.HandleInputCalled, Is.False);
        Assert.That(scene2.HandleInputCalled, Is.True);
    }

    [Test]
    public void InputDevice_EdgeCase_RapidKeyStateChanges()
    {
        var input = new TestInputDevice();

        // Simulate rapid key state changes
        input.SetKeyState(Tab, true);
        input.SetKeyPressed(Tab, true);

        // Immediately change again
        input.SetKeyState(Tab, false);
        input.SetKeyPressed(Tab, false);
        input.SetKeyReleased(Tab, true);

        // Should reflect final state
        Assert.That(input.IsDown(Tab), Is.False);
        Assert.That(input.WasPressed(Tab), Is.False);
        Assert.That(input.WasReleased(Tab), Is.True);
    }

    [Test]
    public void InputDevice_EdgeCase_MousePositionBoundaryValues()
    {
        var input = new TestInputDevice();

        // Test boundary values
        input.SetMousePosition(new Position(int.MinValue, int.MaxValue));
        var pos = input.MousePosition();

        Assert.That(pos.X, Is.EqualTo(int.MinValue));
        Assert.That(pos.Y, Is.EqualTo(int.MaxValue));
    }

    [Test]
    public void InputDevice_EdgeCase_AllMouseButtonsPressed()
    {
        var input = new TestInputDevice();

        // Press all mouse buttons
        foreach (MouseButtonType button in Enum.GetValues(typeof(MouseButtonType)))
        {
            input.SetMouseButton(button, true);
            input.SetMouseClicked(button, true);

            Assert.That(input.MouseDown(button), Is.True);
            Assert.That(input.MouseClicked(button), Is.True);
        }
    }
}
