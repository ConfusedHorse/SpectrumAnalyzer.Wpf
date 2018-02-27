using System;
using System.Collections.Generic;
using CSCore.DSP;

namespace SpectrumAnalyzer.Models
{
    // this code is taken from the CSCore visualization example
    public class SpectrumProvider : FftProvider
    {
        private readonly int _sampleRate;
        private readonly List<object> _contexts = new List<object>();

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

        public bool GetFftData(float[] fftResultBuffer, object context)
        {
            if (_contexts.Contains(context))
                return false;

            _contexts.Add(context);
            GetFftData(fftResultBuffer);
            return true;
        }

        public override void Add(float[] samples, int count)
        {
            base.Add(samples, count);
            if (count > 0)
                _contexts.Clear();
        }

        public override void Add(float left, float right)
        {
            base.Add(left, right);
            _contexts.Clear();
        }
    }
}