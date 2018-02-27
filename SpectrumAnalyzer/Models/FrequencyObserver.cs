namespace SpectrumAnalyzer.Models
{
    public class FrequencyObserver : FrequencyBin
    {
        private double _threshhold = 0.35;
        private string _title;
        private bool _beatDetected;

        public double Threshhold
        {
            get => _threshhold;
            set
            {
                _threshhold = value;
                RaisePropertyChanged();
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                RaisePropertyChanged();
            }
        }

        public bool BeatDetected
        {
            get => _beatDetected;
            set
            {
                _beatDetected = value;
                RaisePropertyChanged();
            }
        }
    }
}