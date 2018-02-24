using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace SpectrumAnalyzer.Controls
{
    /// <summary>
    ///     Interaktionslogik für AudioSpectrum.xaml
    /// </summary>
    public partial class AudioSpectrum
    {
        public AudioSpectrum()
        {
            InitializeComponent();
            SizeChanged += AdjustLines;
        }

        private void AdjustLines(object sender, SizeChangedEventArgs _)
        {
            var items = Spectrum.Items.OfType<AudioLine>();
            var margin = items.FirstOrDefault()?.Margin;
            var offset = margin?.Top + margin?.Bottom ?? 0;
            foreach (var spectrumItem in Spectrum.Items.OfType<AudioLine>())
                spectrumItem.Height = ActualHeight - offset;
        }

        #region Dependency Properties

        public static readonly DependencyProperty SpeedDroppingProperty = DependencyProperty.Register(
            "SpeedDropping", typeof(double), typeof(AudioSpectrum), new PropertyMetadata(25.5d));

        public double SpeedDropping
        {
            get => (double) GetValue(SpeedDroppingProperty);
            set => SetValue(SpeedDroppingProperty, value);
        }

        public static readonly DependencyProperty SpeedRaisingProperty = DependencyProperty.Register(
            "SpeedRaising", typeof(double), typeof(AudioSpectrum), new PropertyMetadata(25.5d));

        public double SpeedRaising
        {
            get => (double) GetValue(SpeedRaisingProperty);
            set => SetValue(SpeedRaisingProperty, value);
        }

        public new static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            "Foreground", typeof(SolidColorBrush), typeof(AudioSpectrum), new PropertyMetadata(Brushes.DimGray));

        public new SolidColorBrush Foreground
        {
            get => (SolidColorBrush) GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        public static readonly DependencyProperty ForegroundPitchedProperty = DependencyProperty.Register(
            "ForegroundPitched", typeof(SolidColorBrush), typeof(AudioSpectrum), new PropertyMetadata(Brushes.DarkRed));
        
        public SolidColorBrush ForegroundPitched
        {
            get => (SolidColorBrush)GetValue(ForegroundPitchedProperty);
            set => SetValue(ForegroundPitchedProperty, value);
        }

        public static readonly DependencyProperty PitchColorProperty = DependencyProperty.Register(
            "PitchColor", typeof(bool), typeof(AudioSpectrum), new PropertyMetadata(default(bool)));

        public bool PitchColor
        {
            get => (bool)GetValue(PitchColorProperty);
            set => SetValue(PitchColorProperty, value);
        }

        #endregion
    }
}