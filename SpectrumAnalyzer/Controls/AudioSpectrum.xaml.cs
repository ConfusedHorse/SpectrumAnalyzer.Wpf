using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SpectrumAnalyzer.Bass.ViewModel;

namespace SpectrumAnalyzer.Controls
{
    /// <summary>
    ///     Interaktionslogik für AudioSpectrum.xaml
    /// </summary>
    public partial class AudioSpectrum : UserControl
    {
        private readonly int _lines;

        public AudioSpectrum()
        {
            InitializeComponent();
            _lines = AnalyzerViewModel.Lines;
            Loaded += OnLoaded;
            SizeChanged += AdjustLines;
        }

        private void AdjustLines(object sender, SizeChangedEventArgs _)
        {
            foreach (var spectrumItem in Spectrum.Items.OfType<AudioLine>())
                spectrumItem.Height = ActualHeight - 6; // AudioLine.Default.Margin == 6
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Data.CollectionChanged += VisualizeSpectrumData;
            for (var i = 0; i < _lines; i++) Spectrum.Items.Add(CreateAudioLine());
            AdjustLines(this, null);
        }

        private AudioLine CreateAudioLine()
        {
            var newLine = AudioLine.Default;
            newLine.SpeedDropping = SpeedDropping;
            newLine.SpeedRaising = SpeedRaising;
            newLine.Foreground = Foreground;
            newLine.ForegroundPitched = ForegroundPitched;
            newLine.PitchColor = PitchColor;
            newLine.Height = Height;
            return newLine;
        }

        private void VisualizeSpectrumData(object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (Data.Count != _lines) return;
            var i = 0; // assuming observable collection is sorted
            foreach (var audioLine in Spectrum.Items.OfType<AudioLine>()) audioLine.Value = Data[i++];
        }

        #region Dependency Properties

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data", typeof(ObservableCollection<int>), typeof(AudioSpectrum),
            new PropertyMetadata(default(ObservableCollection<int>)));

        public ObservableCollection<int> Data
        {
            get => (ObservableCollection<int>) GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        public static readonly DependencyProperty SpeedDroppingProperty = DependencyProperty.Register(
            "SpeedDropping", typeof(double), typeof(AudioSpectrum), new PropertyMetadata(25.5d));

        public double SpeedDropping
        {
            get => (double) GetValue(SpeedDroppingProperty);
            set
            {
                SetValue(SpeedDroppingProperty, value);
                foreach (var spectrumItem in Spectrum.Items.OfType<AudioLine>()) spectrumItem.SpeedDropping = value;
            }
        }

        public static readonly DependencyProperty SpeedRaisingProperty = DependencyProperty.Register(
            "SpeedRaising", typeof(double), typeof(AudioSpectrum), new PropertyMetadata(25.5d));

        public double SpeedRaising
        {
            get => (double) GetValue(SpeedRaisingProperty);
            set
            {
                SetValue(SpeedRaisingProperty, value);
                foreach (var spectrumItem in Spectrum.Items.OfType<AudioLine>()) spectrumItem.SpeedRaising = value;
            }
        }

        public new static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            "Foreground", typeof(SolidColorBrush), typeof(AudioSpectrum), new PropertyMetadata(Brushes.DimGray));

        public new SolidColorBrush Foreground
        {
            get => (SolidColorBrush) GetValue(ForegroundProperty);
            set
            {
                SetValue(ForegroundProperty, value);
                foreach (var spectrumItem in Spectrum.Items.OfType<AudioLine>()) spectrumItem.Foreground = value;
            }
        }

        public static readonly DependencyProperty ForegroundPitchedProperty = DependencyProperty.Register(
            "ForegroundPitched", typeof(SolidColorBrush), typeof(AudioSpectrum), new PropertyMetadata(Brushes.DarkRed));

        /// <summary>
        /// change line color to this when pitched and <see cref="PitchColor"/> is set to true
        /// </summary>
        public SolidColorBrush ForegroundPitched
        {
            get => (SolidColorBrush)GetValue(ForegroundPitchedProperty);
            set
            {
                SetValue(ForegroundPitchedProperty, value);
                foreach (var spectrumItem in Spectrum.Items.OfType<AudioLine>()) spectrumItem.ForegroundPitched = value;
            }
        }

        public static readonly DependencyProperty PitchColorProperty = DependencyProperty.Register(
            "PitchColor", typeof(bool), typeof(AudioSpectrum), new PropertyMetadata(default(bool)));

        public bool PitchColor
        {
            get => (bool)GetValue(PitchColorProperty);
            set
            {
                SetValue(PitchColorProperty, value);
                foreach (var spectrumItem in Spectrum.Items.OfType<AudioLine>()) spectrumItem.PitchColor = value;
            }
        }

        #endregion
    }
}