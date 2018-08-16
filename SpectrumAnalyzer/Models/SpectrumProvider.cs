using System;
using CSCore.DSP;

namespace SpectrumAnalyzer.Models
{
    public class SpectrumProvider : FftProvider
    {
        private readonly int _sampleRate;

        public SpectrumProvider(int channels, int sampleRate, FftSize fftSize)
            : base(channels, fftSize)
        {
            if (sampleRate <= 0)
                throw new ArgumentOutOfRangeException(nameof(sampleRate));
            _sampleRate = sampleRate;
        }

        public int GetFftBandIndex(float frequency)
        {
            var fftSize = (int)FftSize;
            var f = _sampleRate / 2.0;
            // ReSharper disable once PossibleLossOfFraction
            return (int)(frequency / f * (fftSize / 2));
        }

        public int GetFrequency(int fftBandIndex)
        {
            var fftSize = (int)FftSize;
            var f = _sampleRate / 2.0;
            // ReSharper disable once PossibleLossOfFraction
            return (int)(fftBandIndex * f / (fftSize / 2));
        }
    }
}