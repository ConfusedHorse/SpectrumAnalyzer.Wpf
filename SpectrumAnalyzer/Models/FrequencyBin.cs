using System;

namespace SpectrumAnalyzer.Models
{
    public class FrequencyBin
    {
        private double _value;

        public event EventHandler<double> ValueChanged;

        public FrequencyBin(int value = 0)
        {
            Value = value;
        }

        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                RaiseValueChanged(value);
            }
        }

        private void RaiseValueChanged(double newValue)
        {
            ValueChanged?.Invoke(this, newValue);
        }
    }
}