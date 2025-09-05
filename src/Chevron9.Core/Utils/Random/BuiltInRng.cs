using System.Runtime.CompilerServices;

namespace Chevron9.Core.Utils.Random;

public static class BuiltInRng
{
    public static System.Random Generator { get; private set; } = new();

    public static void Reset()
    {
        Generator = new System.Random();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Next()
    {
        return Generator.Next();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Next(int maxValue)
    {
        return Generator.Next(maxValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Next(int minValue, int count)
    {
        return minValue + Generator.Next(count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Next(long maxValue)
    {
        return Generator.NextInt64(maxValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Next(long minValue, long count)
    {
        return minValue + Generator.NextInt64(count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long NextLong()
    {
        return Generator.NextInt64();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double NextDouble()
    {
        return Generator.NextDouble();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NextBytes(Span<byte> buffer)
    {
        Generator.NextBytes(buffer);
    }
}
