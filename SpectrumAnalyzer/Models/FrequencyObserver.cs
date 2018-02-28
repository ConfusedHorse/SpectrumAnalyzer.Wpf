namespace SpectrumAnalyzer.Models
{
    public class FrequencyObserver : FrequencyBin
    {
        #region Fields

        private double _averageFactor = 1.2;
        private double _averageEnergy;
        private double _averageEnergyPercentage = 30;
        private double _averageEnergyAdjustment = 0.001;
        private string _title;
        private bool _beatDetected;

        #endregion Fields

        #region Properties

        public double AverageFactor
        {
            get => _averageFactor;
            set
            {
                _averageFactor = value;
                RaisePropertyChanged();
            }
        }

        public double AverageEnergy
        {
            get => _averageEnergy;
            set
            {
                _averageEnergy = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(AverageEnergyThreshold));
            }
        }

        public double AverageEnergyPercentage
        {
            get => _averageEnergyPercentage;
            set
            {
                _averageEnergyPercentage = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(AverageEnergyThreshold));
            }
        }

        public double AverageEnergyAdjustment
        {
            get => _averageEnergyAdjustment;
            set
            {
                _averageEnergyAdjustment = value;
                RaisePropertyChanged();
            }
        }

        public double AverageEnergyThreshold => AverageEnergy * AverageEnergyPercentage * 0.01;

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

        #endregion Properties

        #region Public Methods

        public void AdjustAverage(float cur)
        {
            AverageEnergy = _averageEnergy * (1 - _averageEnergyAdjustment) + cur * _averageEnergyAdjustment;
        }

        #endregion Public Methods
    }
}