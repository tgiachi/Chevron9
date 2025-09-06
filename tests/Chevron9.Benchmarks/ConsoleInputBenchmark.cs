using System.Diagnostics;
using Chevron9.Backends.Terminal.Input;
using Chevron9.Core.Data.Input;

namespace Chevron9.Benchmarks;

/// <summary>
///     Real-world benchmark application that tests ConsoleInputDevice at 30 FPS
///     Run this to verify actual performance in a terminal environment
/// </summary>
public static class ConsoleInputBenchmark
{
    private const double TargetFPS = 30.0;
    private const double FrameTimeMs = 1000.0 / TargetFPS; // ~33.33ms
    
    public static void Main(string[] args)
    {
        Console.WriteLine("Chevron9 ConsoleInputDevice 30 FPS Benchmark");
        Console.WriteLine("=============================================");
        Console.WriteLine($"Target FPS: {TargetFPS}");
        Console.WriteLine($"Target frame time: {FrameTimeMs:F2}ms");
        Console.WriteLine();
        Console.WriteLine("Press any key to interact, ESC to quit, or wait for auto-benchmark...");
        Console.WriteLine();
        
        using var inputDevice = new ConsoleInputDevice();
        var benchmark = new FrameBenchmark();
        
        var stopwatch = Stopwatch.StartNew();
        var nextFrameTime = 0.0;
        var frameCount = 0;
        
        while (true)
        {
            var currentTime = stopwatch.ElapsedMilliseconds;
            
            // Check if it's time for the next frame
            if (currentTime >= nextFrameTime)
            {
                var frameStart = stopwatch.Elapsed;
                
                // Poll input (this is what we're benchmarking)
                inputDevice.Poll();
                
                var pollTime = stopwatch.Elapsed - frameStart;
                
                // Process input and update display
                var shouldExit = ProcessInput(inputDevice, frameCount);
                if (shouldExit) break;
                
                var frameEnd = stopwatch.Elapsed;
                var totalFrameTime = frameEnd - frameStart;
                
                benchmark.RecordFrame(pollTime, totalFrameTime);
                
                // Display real-time stats every 30 frames (1 second)
                if (frameCount % 30 == 0 && frameCount > 0)
                {
                    DisplayStats(benchmark, frameCount, stopwatch.ElapsedMilliseconds);
                }
                
                frameCount++;
                nextFrameTime = currentTime + FrameTimeMs;
                
                // Stop benchmark after 10 seconds
                if (stopwatch.ElapsedMilliseconds > 10000)
                {
                    break;
                }
            }
            else
            {
                // Sleep for a short time to avoid busy waiting
                Thread.Sleep(1);
            }
        }
        
        // Final results
        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine("FINAL BENCHMARK RESULTS");
        Console.WriteLine(new string('=', 50));
        DisplayFinalStats(benchmark, frameCount, stopwatch.ElapsedMilliseconds);
    }
    
    private static bool ProcessInput(ConsoleInputDevice inputDevice, int frameCount)
    {
        // Check for ESC key to exit
        if (inputDevice.WasPressed(InputKeys.Escape))
        {
            Console.WriteLine("\nBenchmark stopped by user (ESC pressed)");
            return true;
        }
        
        // Display interactive feedback for other keys
        if (inputDevice.WasPressed(InputKeys.Space))
        {
            Console.Write($"[Frame {frameCount}] SPACE ");
        }
        else if (inputDevice.WasPressed(InputKeys.Enter))
        {
            Console.Write($"[Frame {frameCount}] ENTER ");
        }
        else
        {
            // Check for any letter key
            for (char c = 'A'; c <= 'Z'; c++)
            {
                if (inputDevice.WasPressed(InputKeys.GetKey(c)))
                {
                    Console.Write($"[Frame {frameCount}] {c} ");
                    break;
                }
            }
        }
        
        // Display mouse interaction
        if (inputDevice.MouseClicked(Core.Types.MouseButtonType.Left))
        {
            var pos = inputDevice.MousePosition();
            Console.Write($"[Frame {frameCount}] CLICK({pos.X},{pos.Y}) ");
        }
        
        return false;
    }
    
    private static void DisplayStats(FrameBenchmark benchmark, int frameCount, long elapsedMs)
    {
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth - 1)); // Clear line
        Console.SetCursorPosition(0, Console.CursorTop);
        
        var actualFPS = frameCount / (elapsedMs / 1000.0);
        var avgPollTime = benchmark.AveragePollTimeMs;
        var avgFrameTime = benchmark.AverageFrameTimeMs;
        
        Console.Write($"Frame {frameCount:D4} | FPS: {actualFPS:F1} | Poll: {avgPollTime:F3}ms | Frame: {avgFrameTime:F2}ms");
    }
    
    private static void DisplayFinalStats(FrameBenchmark benchmark, int totalFrames, long totalTimeMs)
    {
        var actualFPS = totalFrames / (totalTimeMs / 1000.0);
        
        Console.WriteLine($"Total frames: {totalFrames}");
        Console.WriteLine($"Total time: {totalTimeMs}ms");
        Console.WriteLine($"Actual FPS: {actualFPS:F2} (target: {TargetFPS})");
        Console.WriteLine($"FPS efficiency: {(actualFPS / TargetFPS * 100):F1}%");
        Console.WriteLine();
        
        Console.WriteLine("Poll() Performance:");
        Console.WriteLine($"  Average time: {benchmark.AveragePollTimeMs:F4}ms");
        Console.WriteLine($"  Maximum time: {benchmark.MaxPollTimeMs:F4}ms");
        Console.WriteLine($"  Minimum time: {benchmark.MinPollTimeMs:F4}ms");
        Console.WriteLine($"  Standard deviation: {benchmark.PollTimeStdDevMs:F4}ms");
        Console.WriteLine();
        
        Console.WriteLine("Frame Performance:");
        Console.WriteLine($"  Average time: {benchmark.AverageFrameTimeMs:F4}ms (target: {FrameTimeMs:F2}ms)");
        Console.WriteLine($"  Maximum time: {benchmark.MaxFrameTimeMs:F4}ms");
        Console.WriteLine($"  Frame budget usage: {(benchmark.AverageFrameTimeMs / FrameTimeMs * 100):F1}%");
        Console.WriteLine();
        
        // Performance assessment
        var pollEfficiency = benchmark.AveragePollTimeMs < (FrameTimeMs * 0.1); // Poll should use <10% of frame time
        var frameEfficiency = benchmark.AverageFrameTimeMs < (FrameTimeMs * 0.8); // Frame should use <80% of budget
        var fpsStability = Math.Abs(actualFPS - TargetFPS) < 2.0; // Within 2 FPS of target
        
        Console.WriteLine("Performance Assessment:");
        Console.WriteLine($"  Poll efficiency: {(pollEfficiency ? "✓ PASS" : "✗ FAIL")} (Poll time < 10% frame budget)");
        Console.WriteLine($"  Frame efficiency: {(frameEfficiency ? "✓ PASS" : "✗ FAIL")} (Frame time < 80% frame budget)");
        Console.WriteLine($"  FPS stability: {(fpsStability ? "✓ PASS" : "✗ FAIL")} (Within 2 FPS of target)");
        Console.WriteLine();
        
        var overallRating = (pollEfficiency ? 1 : 0) + (frameEfficiency ? 1 : 0) + (fpsStability ? 1 : 0);
        var rating = overallRating switch
        {
            3 => "EXCELLENT - Ready for production 30 FPS applications",
            2 => "GOOD - Suitable for most 30 FPS applications with minor optimizations",
            1 => "ACCEPTABLE - May work for 30 FPS but needs optimization",
            _ => "POOR - Not suitable for 30 FPS applications, requires significant optimization"
        };
        
        Console.WriteLine($"Overall Rating: {rating}");
    }
}

internal class FrameBenchmark
{
    private readonly List<double> _pollTimes = new();
    private readonly List<double> _frameTimes = new();
    
    public void RecordFrame(TimeSpan pollTime, TimeSpan frameTime)
    {
        _pollTimes.Add(pollTime.TotalMilliseconds);
        _frameTimes.Add(frameTime.TotalMilliseconds);
    }
    
    public double AveragePollTimeMs => _pollTimes.Count > 0 ? _pollTimes.Average() : 0;
    public double MaxPollTimeMs => _pollTimes.Count > 0 ? _pollTimes.Max() : 0;
    public double MinPollTimeMs => _pollTimes.Count > 0 ? _pollTimes.Min() : 0;
    public double PollTimeStdDevMs => CalculateStandardDeviation(_pollTimes);
    
    public double AverageFrameTimeMs => _frameTimes.Count > 0 ? _frameTimes.Average() : 0;
    public double MaxFrameTimeMs => _frameTimes.Count > 0 ? _frameTimes.Max() : 0;
    
    private static double CalculateStandardDeviation(List<double> values)
    {
        if (values.Count == 0) return 0;
        
        var average = values.Average();
        var sumOfSquaresOfDifferences = values.Select(val => (val - average) * (val - average)).Sum();
        return Math.Sqrt(sumOfSquaresOfDifferences / values.Count);
    }
}