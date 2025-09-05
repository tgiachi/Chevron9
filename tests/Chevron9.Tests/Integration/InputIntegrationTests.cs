using Chevron9.Core.Data.Input;
using Chevron9.Core.Interfaces;
using Chevron9.Core.Scenes;
using Chevron9.Core.Types;
using Chevron9.Shared.Primitives;

namespace Chevron9.Tests.Integration;

[TestFixture]
public class InputIntegrationTests
{
    private sealed class TestInputDevice : IInputDevice
    {
        public bool IsDown(InputKey key) => key.Code == InputKeys.A.Code;
        public bool WasPressed(InputKey key) => key.Code == InputKeys.Space.Code;
        public bool WasReleased(InputKey key) => key.Code == InputKeys.Enter.Code;
        public bool IsDown(InputKey key, InputKeyModifierType modifiers) => false;
        public bool WasPressed(InputKey key, InputKeyModifierType modifiers) => false;
        public bool WasReleased(InputKey key, InputKeyModifierType modifiers) => false;
        public InputKeyModifierType GetActiveModifiers() => InputKeyModifierType.Control;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Method is part of IInputDevice interface")]
        public Position MousePosition() => new Position(100, 200);
        public bool MouseDown(MouseButtonType button) => button == MouseButtonType.Left;
        public bool MouseClicked(MouseButtonType button) => button == MouseButtonType.Right;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Method is part of IInputDevice interface")]
        public Position MouseWheelDelta() => new Position(0, 120);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Method is part of IInputDevice interface")]
        public Position MouseDelta() => new Position(5, -3);
        public bool MousePressed(MouseButtonType button) => button == MouseButtonType.Middle;
        public bool MouseReleased(MouseButtonType button) => false;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Method is part of IInputDevice interface")]
        public bool IsPointerInside() => true;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Method is part of IInputDevice interface")]
        public bool HasFocus() => true;
        public void Poll() { }
    }

    private sealed class TestScene : IScene
    {
        public string Name { get; }
        public IReadOnlyList<ILayer> Layers => Array.Empty<ILayer>();
        public bool InputHandled { get; private set; }
        public IInputDevice? LastInputDevice { get; private set; }

        public TestScene(string name)
        {
            Name = name;
        }

        public void Enter() { }
        public void Close() { }
        public void Dispose() { }
        public void Update(double fixedDt, IInputDevice input) { }
        public void Render(IRenderCommandCollector rq, float alpha) { }

        public bool HandleInput(IInputDevice input)
        {
            InputHandled = true;
            LastInputDevice = input;
            return true; // Consume input
        }
    }

    [Test]
    public void InputDevice_SceneManager_Integration_EndToEnd()
    {
        // Arrange
        var input = new TestInputDevice();
        var scene = new TestScene("TestScene");
        var sceneManager = new SceneManager();
        sceneManager.Push(scene);

        // Act - Handle input through scene manager
        var result = sceneManager.HandleInput(input);

        // Assert - Input was properly routed
        Assert.That(scene.InputHandled, Is.True);
        Assert.That(scene.LastInputDevice, Is.EqualTo(input));
        Assert.That(result, Is.True); // Scene consumed input

        // Cleanup
        sceneManager.Dispose();
    }

    [Test]
    public void InputDevice_KeyboardState_Integration()
    {
        // Arrange
        var input = new TestInputDevice();

        // Act & Assert - Test keyboard state
        Assert.That(input.IsDown(InputKeys.A), Is.True);
        Assert.That(input.IsDown(InputKeys.B), Is.False);
        Assert.That(input.WasPressed(InputKeys.Space), Is.True);
        Assert.That(input.WasPressed(InputKeys.Tab), Is.False);
        Assert.That(input.WasReleased(InputKeys.Enter), Is.True);
        Assert.That(input.GetActiveModifiers(), Is.EqualTo(InputKeyModifierType.Control));
    }

    [Test]
    public void InputDevice_MouseState_Integration()
    {
        // Arrange
        var input = new TestInputDevice();

        // Act & Assert - Test mouse state
        Assert.That(input.MousePosition(), Is.EqualTo(new Position(100, 200)));
        Assert.That(input.MouseDown(MouseButtonType.Left), Is.True);
        Assert.That(input.MouseDown(MouseButtonType.Right), Is.False);
        Assert.That(input.MouseClicked(MouseButtonType.Right), Is.True);
        Assert.That(input.MousePressed(MouseButtonType.Middle), Is.True);
        Assert.That(input.MouseWheelDelta(), Is.EqualTo(new Position(0, 120)));
        Assert.That(input.MouseDelta(), Is.EqualTo(new Position(5, -3)));
        Assert.That(input.IsPointerInside(), Is.True);
        Assert.That(input.HasFocus(), Is.True);
    }

    [Test]
    public void InputDevice_SceneStack_InputPropagation()
    {
        // Arrange
        var input = new TestInputDevice();
        var scene1 = new TestScene("Scene1");
        var scene2 = new TestScene("Scene2"); // Top scene
        var sceneManager = new SceneManager();

        sceneManager.Push(scene1);
        sceneManager.Push(scene2);

        // Act - Handle input (should go to top scene only)
        var result = sceneManager.HandleInput(input);

        // Assert - Only top scene received input
        Assert.That(scene2.InputHandled, Is.True);
        Assert.That(scene2.LastInputDevice, Is.EqualTo(input));
        Assert.That(scene1.InputHandled, Is.False);
        Assert.That(result, Is.True);

        // Cleanup
        sceneManager.Dispose();
    }
}
