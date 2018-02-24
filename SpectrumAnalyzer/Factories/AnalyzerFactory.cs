using System.Collections.Generic;
using SpectrumAnalyzer.Models;

namespace SpectrumAnalyzer.Factories
{
    public static class AnalyzerFactory
    {
        public static FrequencyBin Create(int value = 0)
        {
            return new FrequencyBin(value);
        }
        public static IEnumerable<FrequencyBin> CreateMany(int count, int value = 0)
        {
            for (var i = 0; i < count; i++) yield return Create(value);
        }
    }
}