using System;

namespace NCoverCop
{
    public static class DoubleExtensions
    {
        public static bool IsZero(this double value, double tollerance = 0.000001) => Math.Abs(value) < tollerance;
    }

    public static class ArrayExtensions
    {
        public static T Fetch<T>(this T[] array, int index, T defaultVal = default(T))
        {
            if (index < 0) // negative index values index from the end
            {
                index += array.Length;
            }
            if (index >= 0 && index < array.Length)
            {
                return array[index];
            }
            return defaultVal;
        }
    }
}