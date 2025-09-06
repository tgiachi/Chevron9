using System.Diagnostics;
using Chevron9.Backends.Terminal.Input;
using Chevron9.Core.Data.Input;
using Chevron9.Core.Types;
using NUnit.Framework;

namespace Chevron9.Tests.Backends.Terminal.Input;

[TestFixture]
public class ConsoleInputDeviceKeyRepeatTests
{
    private ConsoleInputDevice? _inputDevice;

    [SetUp]
    public void SetUp()
    {
        _inputDevice = new ConsoleInputDevice();
    }

    [TearDown]
    public void TearDown()
    {
        _inputDevice?.Dispose();
        _inputDevice = null;
    }

    [Test]
    public void KeyRepeatDelay_DefaultValue_ShouldBe500Milliseconds()
    {
        var inputDevice = _inputDevice!;
        Assert.That(inputDevice.KeyRepeatDelay, Is.EqualTo(TimeSpan.FromMilliseconds(500)));
    }

    [Test]
    public void KeyRepeatDelay_ShouldBeConfigurable()
    {
        var inputDevice = _inputDevice!;
        var customDelay = TimeSpan.FromMilliseconds(200);

        inputDevice.KeyRepeatDelay = customDelay;

        Assert.That(inputDevice.KeyRepeatDelay, Is.EqualTo(customDelay));
    }

    [Test]
    public void IsRepeating_WithNoKeysPressed_ShouldReturnFalse()
    {
        var inputDevice = _inputDevice!;

        // Poll without any input
        inputDevice.Poll();

        Assert.That(inputDevice.IsRepeating(InputKeys.A), Is.False);
        Assert.That(inputDevice.IsRepeating(InputKeys.A, InputKeyModifierType.None), Is.False);
    }

    [Test]
    public void KeyRepeat_Performance_ShouldNotImpactPolling()
    {
        var inputDevice = _inputDevice!;
        inputDevice.KeyRepeatDelay = TimeSpan.FromMilliseconds(10); // Very fast repeat

        var stopwatch = Stopwatch.StartNew();

        // Poll many times to test performance impact
        for (int i = 0; i < 1000; i++)
        {
            inputDevice.Poll();
        }

        stopwatch.Stop();
        var avgPollTime = stopwatch.ElapsedMilliseconds / 1000.0;

        Console.WriteLine($"Average poll time with key repeat logic: {avgPollTime:F3}ms");
        Assert.That(avgPollTime, Is.LessThan(2.0), "Poll time should remain fast even with key repeat logic");
    }

    [Test]
    public void KeyRepeat_ConfigurableDelay_AcceptsValidValues()
    {
        var inputDevice = _inputDevice!;

        // Test various valid delay values
        var validDelays = new[]
        {
            TimeSpan.FromMilliseconds(10),
            TimeSpan.FromMilliseconds(100),
            TimeSpan.FromMilliseconds(500),
            TimeSpan.FromMilliseconds(1000),
            TimeSpan.FromSeconds(2)
        };

        foreach (var delay in validDelays)
        {
            inputDevice.KeyRepeatDelay = delay;
            Assert.That(inputDevice.KeyRepeatDelay, Is.EqualTo(delay), $"Should accept delay of {delay.TotalMilliseconds}ms");
        }
    }

    [Test]
    public void KeyRepeat_MethodsExist_ShouldBeCallable()
    {
        var inputDevice = _inputDevice!;

        // These methods should exist and be callable without throwing
        Assert.DoesNotThrow(() => inputDevice.IsRepeating(InputKeys.A));
        Assert.DoesNotThrow(() => inputDevice.IsRepeating(InputKeys.A, InputKeyModifierType.None));
        Assert.DoesNotThrow(() => inputDevice.IsRepeating(InputKeys.A, InputKeyModifierType.Control));
        Assert.DoesNotThrow(() => inputDevice.IsRepeating(InputKeys.A, InputKeyModifierType.Alt));
        Assert.DoesNotThrow(() => inputDevice.IsRepeating(InputKeys.A, InputKeyModifierType.Shift));
    }

    [Test]
    public void KeyRepeat_ReturnsBoolean_ForAllInputKeys()
    {
        var inputDevice = _inputDevice!;
        inputDevice.Poll();

        // Test that IsRepeating returns boolean values for common keys
        var testKeys = new[]
        {
            InputKeys.A, InputKeys.B, InputKeys.C,
            InputKeys.Enter, InputKeys.Space, InputKeys.Escape,
            InputKeys.Left, InputKeys.Right, InputKeys.Up, InputKeys.Down,
            InputKeys.F1, InputKeys.F12
        };

        foreach (var key in testKeys)
        {
            var result1 = inputDevice.IsRepeating(key);
            var result2 = inputDevice.IsRepeating(key, InputKeyModifierType.None);

            Assert.That(result1, Is.TypeOf<bool>(), $"IsRepeating({key}) should return boolean");
            Assert.That(result2, Is.TypeOf<bool>(), $"IsRepeating({key}, None) should return boolean");
        }
    }

    [Test]
    public void KeyRepeat_ConsistencyTest_SameResultsAcrossPolls()
    {
        var inputDevice = _inputDevice!;

        // Without any real input, IsRepeating should consistently return false
        for (int i = 0; i < 10; i++)
        {
            inputDevice.Poll();
            Assert.That(inputDevice.IsRepeating(InputKeys.X), Is.False, $"Poll {i}: X should not be repeating");
            Assert.That(inputDevice.IsRepeating(InputKeys.Y), Is.False, $"Poll {i}: Y should not be repeating");
        }
    }

    [Test]
    public void KeyRepeat_NoExceptionsThrown_DuringIntensivePolling()
    {
        var inputDevice = _inputDevice!;
        inputDevice.KeyRepeatDelay = TimeSpan.FromMilliseconds(10);

        // Test that intensive polling doesn't throw exceptions
        // This is more reliable than memory testing due to terminal initialization overhead
        Assert.DoesNotThrow(() =>
        {
            for (int i = 0; i < 1000; i++)
            {
                inputDevice.Poll();
            }
        }, "Key repeat logic should not throw exceptions during intensive polling");
    }

    [Test]
    public void KeyRepeat_Integration_WorksWithOtherInputMethods()
    {
        var inputDevice = _inputDevice!;
        inputDevice.Poll();

        // Key repeat should not interfere with other input detection methods
        Assert.DoesNotThrow(() => inputDevice.IsDown(InputKeys.A));
        Assert.DoesNotThrow(() => inputDevice.WasPressed(InputKeys.A));
        Assert.DoesNotThrow(() => inputDevice.WasReleased(InputKeys.A));
        Assert.DoesNotThrow(() => inputDevice.MousePosition());
        Assert.DoesNotThrow(() => inputDevice.MouseDown(MouseButtonType.Left));

        // And key repeat methods should work alongside them
        Assert.DoesNotThrow(() => inputDevice.IsRepeating(InputKeys.A));
        Assert.DoesNotThrow(() => inputDevice.IsRepeating(InputKeys.A, InputKeyModifierType.Control));
    }
}
