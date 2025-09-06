using System.Diagnostics;
using Chevron9.Backends.Terminal.Input;
using Chevron9.Core.Data.Input;
using NUnit.Framework;

namespace Chevron9.Tests.Backends.Terminal.Input;

[TestFixture]
public class ConsoleInputDevicePerformanceTests
{
    private ConsoleInputDevice? _inputDevice;

    [Test]
    public void Poll_ExecutionTime_ShouldBeFasterThan16Milliseconds()
    {
        // For 30 FPS, each frame should complete in ~33.33ms, Poll() should be much faster
        // We test for <16ms to ensure Poll() uses less than half the frame budget
        var inputDevice = CreateInputDevice();
        var stopwatch = new Stopwatch();

        // Warmup calls to eliminate JIT compilation overhead
        for (int i = 0; i < 10; i++)
        {
            inputDevice.Poll();
        }

        // Measure multiple Poll() calls
        var measurements = new List<long>();
        for (int i = 0; i < 100; i++)
        {
            stopwatch.Restart();
            inputDevice.Poll();
            stopwatch.Stop();
            measurements.Add(stopwatch.ElapsedTicks);
        }

        var avgTicks = measurements.Average();
        var maxTicks = measurements.Max();
        var avgMilliseconds = (avgTicks / (double)Stopwatch.Frequency) * 1000;
        var maxMilliseconds = (maxTicks / (double)Stopwatch.Frequency) * 1000;

        Console.WriteLine($"Poll() Average time: {avgMilliseconds:F3}ms");
        Console.WriteLine($"Poll() Maximum time: {maxMilliseconds:F3}ms");
        Console.WriteLine($"Poll() calls per second capacity: {1000 / avgMilliseconds:F0}");

        Assert.That(avgMilliseconds, Is.LessThan(16.0), $"Average Poll() time ({avgMilliseconds:F3}ms) should be less than 16ms for 30 FPS");
        Assert.That(maxMilliseconds, Is.LessThan(33.0), $"Maximum Poll() time ({maxMilliseconds:F3}ms) should be less than 33ms");
    }

    [Test]
    public void Poll_ConsistentFrameTiming_ShouldMaintain30FPS()
    {
        var inputDevice = CreateInputDevice();
        var frameInterval = TimeSpan.FromMilliseconds(33.33); // 30 FPS
        var frameTimes = new List<double>();
        var stopwatch = Stopwatch.StartNew();
        var lastFrameTime = stopwatch.Elapsed;

        // Simulate 30 FPS polling for 1 second (30 frames)
        for (int frame = 0; frame < 30; frame++)
        {
            var targetTime = TimeSpan.FromMilliseconds(frame * 33.33);

            // Wait until it's time for the next frame
            while (stopwatch.Elapsed < targetTime)
            {
                Thread.Sleep(1);
            }

            var frameStart = stopwatch.Elapsed;
            inputDevice.Poll();
            var frameEnd = stopwatch.Elapsed;

            var actualFrameTime = (frameEnd - lastFrameTime).TotalMilliseconds;
            frameTimes.Add(actualFrameTime);
            lastFrameTime = frameEnd;
        }

        var avgFrameTime = frameTimes.Skip(1).Average(); // Skip first frame (can be irregular)
        var maxFrameTime = frameTimes.Skip(1).Max();
        var minFrameTime = frameTimes.Skip(1).Min();
        var frameTimeVariance = frameTimes.Skip(1).Select(t => Math.Pow(t - avgFrameTime, 2)).Average();
        var frameTimeStdDev = Math.Sqrt(frameTimeVariance);

        Console.WriteLine($"Average frame time: {avgFrameTime:F2}ms (target: 33.33ms)");
        Console.WriteLine($"Frame time range: {minFrameTime:F2}ms - {maxFrameTime:F2}ms");
        Console.WriteLine($"Frame time std deviation: {frameTimeStdDev:F2}ms");
        Console.WriteLine($"Effective FPS: {1000 / avgFrameTime:F1}");

        // Frame timing should be consistent with low variance
        Assert.That(Math.Abs(avgFrameTime - 33.33), Is.LessThan(5.0), $"Average frame time should be close to 33.33ms, got {avgFrameTime:F2}ms");
        Assert.That(frameTimeStdDev, Is.LessThan(10.0), $"Frame time should be consistent (std dev < 10ms), got {frameTimeStdDev:F2}ms");
    }

    [Test]
    public void Poll_MemoryAllocation_ShouldBeMinimal()
    {
        var inputDevice = CreateInputDevice();

        // Force garbage collection before test
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var initialMemory = GC.GetTotalMemory(false);

        // Call Poll() many times to detect memory leaks
        for (int i = 0; i < 1000; i++)
        {
            inputDevice.Poll();
        }

        var finalMemory = GC.GetTotalMemory(false);
        var memoryIncrease = finalMemory - initialMemory;

        Console.WriteLine($"Memory before: {initialMemory:N0} bytes");
        Console.WriteLine($"Memory after: {finalMemory:N0} bytes");
        Console.WriteLine($"Memory increase: {memoryIncrease:N0} bytes");
        Console.WriteLine($"Memory per Poll(): {(double)memoryIncrease / 1000:F2} bytes");

        // Memory increase should be reasonable (less than 500KB for 1000 calls)
        // Terminal initialization can cause some initial allocations
        Assert.That(memoryIncrease, Is.LessThan(512_000), $"Memory increase should be reasonable, got {memoryIncrease:N0} bytes for 1000 Poll() calls");
    }

    [Test]
    public void Poll_ConcurrentAccess_ShouldBeThreadSafe()
    {
        var inputDevice = CreateInputDevice();
        var exceptions = new List<Exception>();
        var completedTasks = 0;
        const int taskCount = 4;
        const int pollsPerTask = 250; // Total 1000 Poll() calls

        var tasks = new List<Task>();
        for (int i = 0; i < taskCount; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    for (int j = 0; j < pollsPerTask; j++)
                    {
                        inputDevice.Poll();

                        // Simulate some work between polls
                        Thread.Sleep(1);
                    }
                    Interlocked.Increment(ref completedTasks);
                }
                catch (Exception ex)
                {
                    lock (exceptions)
                    {
                        exceptions.Add(ex);
                    }
                }
            }));
        }

        var completed = Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(30));

        Console.WriteLine($"Completed tasks: {completedTasks}/{taskCount}");
        Console.WriteLine($"Exceptions encountered: {exceptions.Count}");

        Assert.That(completed, Is.True, "All concurrent polling tasks should complete within 30 seconds");
        Assert.That(completedTasks, Is.EqualTo(taskCount));
        Assert.That(exceptions, Is.Empty);
    }

    [Test]
    public void Poll_LongRunningStressTest_ShouldMaintainPerformance()
    {
        var inputDevice = CreateInputDevice();
        var measurements = new List<double>();
        var batchSize = 100;
        var totalBatches = 10; // 1000 total calls

        for (int batch = 0; batch < totalBatches; batch++)
        {
            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < batchSize; i++)
            {
                inputDevice.Poll();
            }

            stopwatch.Stop();
            var avgTimePerCall = stopwatch.ElapsedMilliseconds / (double)batchSize;
            measurements.Add(avgTimePerCall);

            // Small delay between batches to simulate real usage
            Thread.Sleep(50);
        }

        var firstBatchAvg = measurements.Take(3).Average();
        var lastBatchAvg = measurements.TakeLast(3).Average();
        var overallAvg = measurements.Average();
        var maxBatchAvg = measurements.Max();

        Console.WriteLine($"First 3 batches average: {firstBatchAvg:F3}ms per Poll()");
        Console.WriteLine($"Last 3 batches average: {lastBatchAvg:F3}ms per Poll()");
        Console.WriteLine($"Overall average: {overallAvg:F3}ms per Poll()");
        Console.WriteLine($"Worst batch average: {maxBatchAvg:F3}ms per Poll()");

        // Performance should not degrade significantly over time
        var performanceDegradation = lastBatchAvg - firstBatchAvg;
        Assert.That(performanceDegradation, Is.LessThan(5.0), $"Performance should not degrade significantly, got {performanceDegradation:F3}ms increase");
        Assert.That(overallAvg, Is.LessThan(2.0), $"Overall average Poll() time should be under 2ms, got {overallAvg:F3}ms");
    }

    [Test]
    public void Poll_InputStateConsistency_ShouldMaintainCorrectState()
    {
        var inputDevice = CreateInputDevice();
        var inconsistencies = 0;

        // Test that repeated polling doesn't corrupt input state
        for (int i = 0; i < 500; i++)
        {
            var initialMousePos = inputDevice.MousePosition();
            var initialHasFocus = inputDevice.HasFocus();
            var initialPointerInside = inputDevice.IsPointerInside();

            inputDevice.Poll();

            // State should remain consistent when no input is available
            var afterMousePos = inputDevice.MousePosition();
            var afterHasFocus = inputDevice.HasFocus();
            var afterPointerInside = inputDevice.IsPointerInside();

            if (initialMousePos.X != afterMousePos.X || initialMousePos.Y != afterMousePos.Y)
            {
                // Mouse position changes are valid, but should be consistent
                continue;
            }

            if (initialHasFocus != afterHasFocus || initialPointerInside != afterPointerInside)
            {
                inconsistencies++;
            }
        }

        Console.WriteLine($"State inconsistencies detected: {inconsistencies}/500 polls");
        Assert.That(inconsistencies, Is.LessThan(10), $"Input state should remain consistent, found {inconsistencies} inconsistencies in 500 polls");
    }

    private ConsoleInputDevice CreateInputDevice()
    {
        if (_inputDevice == null)
        {
            _inputDevice = new ConsoleInputDevice();
        }
        return _inputDevice;
    }

    [TearDown]
    public void TearDown()
    {
        _inputDevice?.Dispose();
        _inputDevice = null;
    }
}
