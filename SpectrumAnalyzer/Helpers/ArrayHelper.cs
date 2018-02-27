using System;

namespace SpectrumAnalyzer.Helpers
{
    public static class ArrayHelper
    {
        public static T[] SubArray<T>(this T[] data, int from, int to)
        {
            var length = to - from;
            var result = new T[length];
            Array.Copy(data, from, result, 0, length);
            return result;
        }
    }
}