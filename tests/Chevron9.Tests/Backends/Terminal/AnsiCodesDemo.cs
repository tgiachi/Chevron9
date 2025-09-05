using System.Diagnostics;
using Chevron9.Backends.Terminal.Utils;
using Chevron9.Shared.Graphics;

namespace Chevron9.Tests.Backends.Terminal;

/// <summary>
///     Demo program to test ANSI codes in real terminal.
///     This shows actual ANSI escape sequences working in a terminal.
/// </summary>
public static class AnsiCodesDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("=== ANSI Codes Demo ===");
        Console.WriteLine("Testing ANSI escape sequences in real terminal...\n");

        var builder = new AnsiBuilder();

        // Test 1: Simple text
        Console.WriteLine("1. Simple Text:");
        var simpleText = builder.Append("Hello World").Build();
        ExecuteAndDisplay("Simple Text", simpleText);

        // Test 2: Colors
        Console.WriteLine("\n2. Colors:");
        var colorText = builder
            .FgColor(Color.Red).Append("Red ")
            .FgColor(Color.Green).Append("Green ")
            .FgColor(Color.Blue).Append("Blue")
            .Build();
        ExecuteAndDisplay("Colors", colorText);

        // Test 3: Background colors
        Console.WriteLine("\n3. Background Colors:");
        var bgText = builder
            .BgColor(Color.Yellow).FgColor(Color.Black).Append(" Yellow BG ")
            .BgColor(Color.Black).FgColor(Color.White).Append(" Black BG ")
            .Build();
        ExecuteAndDisplay("Background Colors", bgText);

        // Test 4: Cursor movement
        Console.WriteLine("\n4. Cursor Movement:");
        var cursorText = builder
            .CursorPosition(1, 1).Append("Top Left")
            .CursorPosition(3, 5).Append("Positioned")
            .Build();
        ExecuteAndDisplay("Cursor Movement", cursorText);

        // Test 5: Clear screen
        Console.WriteLine("\n5. Clear Screen:");
        var clearText = builder
            .ClearScreen()
            .CursorPosition(1, 1)
            .Append("Screen Cleared!")
            .Build();
        ExecuteAndDisplay("Clear Screen", clearText);

        // Test 6: Complex demo
        Console.WriteLine("\n6. Complex Demo:");
        var complexDemo = builder
            .ClearScreen()
            .CursorPosition(1, 1)
            .FgColor(Color.Cyan).BgColor(Color.Black)
            .Append("╔══════════════════════════════════════╗")
            .CursorPosition(2, 1)
            .Append("║         ANSI Codes Working!         ║")
            .CursorPosition(3, 1)
            .Append("║                                      ║")
            .CursorPosition(4, 1)
            .FgColor(Color.Green)
            .Append("║           ✓ Success!                ║")
            .CursorPosition(5, 1)
            .FgColor(Color.Yellow)
            .Append("║         Terminal supports ANSI       ║")
            .CursorPosition(6, 1)
            .Append("╚══════════════════════════════════════╝")
            .CursorPosition(8, 1)
            .FgColor(Color.White).BgColor(Color.Black)
            .Append("Press Enter to continue...")
            .Build();

        ExecuteAndDisplay("Complex Demo", complexDemo);

        Console.WriteLine("\n=== Demo Complete ===");
        Console.WriteLine("If you saw colored text and positioned elements above,");
        Console.WriteLine("then ANSI codes are working correctly in your terminal!");
    }

    private static void ExecuteAndDisplay(string testName, string ansiSequence)
    {
        try
        {
            Console.WriteLine($"Testing: {testName}");
            Console.WriteLine($"Sequence: {EscapeString(ansiSequence)}");

            // Execute in terminal
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "printf",
                    Arguments = $"\"{ansiSequence.Replace("\"", "\\\"")}\"",
                    RedirectStandardOutput = false, // Let it output to console
                    RedirectStandardError = false,
                    UseShellExecute = false,
                    CreateNoWindow = false
                }
            };

            process.Start();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                Console.WriteLine("✓ Executed successfully");
            }
            else
            {
                Console.WriteLine($"✗ Failed with exit code: {process.ExitCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error: {ex.Message}");
        }
    }

    private static string EscapeString(string input)
    {
        return input
            .Replace("\x1b", "\\e")  // ESC character
            .Replace("[", "\\[")
            .Replace(";", "\\;")
            .Replace("m", "\\m");
    }
}
