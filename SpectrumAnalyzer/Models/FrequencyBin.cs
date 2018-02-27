namespace SpectrumAnalyzer.Models
{
    public class FrequencyBin : ViewModelBase
    {
        private double _value;
        private int _minFrequency;
        private int _maxFrequency;

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
                RaisePropertyChanged();
            }
        }

        public int MinFrequency
        {
            get => _minFrequency;
            set
            {
                _minFrequency = value;
                RaisePropertyChanged();
            }
        }

        public int MaxFrequency
        {
            get => _maxFrequency;
            set
            {
                _maxFrequency = value;
                RaisePropertyChanged();
            }
        }
    }
}