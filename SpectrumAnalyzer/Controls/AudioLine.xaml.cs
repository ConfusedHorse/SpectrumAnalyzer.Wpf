using System;
using System.Windows;
using System.Windows.Media;
using SpectrumAnalyzer.Helpers;
using SpectrumAnalyzer.Models;

namespace SpectrumAnalyzer.Controls
{
    /// <summary>
    /// Interaktionslogik für AudioLine.xaml
    /// </summary>
    public partial class AudioLine
    {
        #region Fields

        private double _currentValue;

        #endregion

        #region Dependency Properties

        public new static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            "Foreground", typeof(SolidColorBrush), typeof(AudioLine), new PropertyMetadata(Brushes.DimGray));

        public new SolidColorBrush Foreground
        {
            get => (SolidColorBrush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        public static readonly DependencyProperty ForegroundPitchedProperty = DependencyProperty.Register(
            "ForegroundPitched", typeof(SolidColorBrush), typeof(AudioLine), new PropertyMetadata(Brushes.DimGray));

        /// <summary>
        /// change line color to this when pitched and <see cref="PitchColor"/> is set to true
        /// </summary>
        public SolidColorBrush ForegroundPitched
        {
            get => (SolidColorBrush) GetValue(ForegroundPitchedProperty);
            set => SetValue(ForegroundPitchedProperty, value);
        }

        public static readonly DependencyProperty PitchColorProperty = DependencyProperty.Register(
            "PitchColor", typeof(bool), typeof(AudioLine), new PropertyMetadata(default(bool)));

        public bool PitchColor
        {
            get => (bool) GetValue(PitchColorProperty);
            set => SetValue(PitchColorProperty, value);
        }

        public static readonly DependencyProperty FrequencyBinProperty = DependencyProperty.Register(
            "FrequencyBin", typeof(FrequencyBin), typeof(AudioLine), new PropertyMetadata(default(FrequencyBin), OnFrequencyBinChanged));

        private static void OnFrequencyBinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is AudioLine al)) return;
            var newDataContext = (FrequencyBin) e.NewValue;
            al.ToolTip = newDataContext.ToString();

            newDataContext.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != nameof(newDataContext.Value)) return;
                UpdateVisuals(al, newDataContext.Value);
            };
        }

        public FrequencyBin FrequencyBin
        {
            get => (FrequencyBin) GetValue(FrequencyBinProperty);
            set => SetValue(FrequencyBinProperty, value);
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Normal", typeof(double), typeof(AudioLine), new PropertyMetadata(255d));

        public double Maximum
        {
            get => (double) GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public static readonly DependencyProperty SpeedDroppingProperty = DependencyProperty.Register(
            "SpeedDropping", typeof(double), typeof(AudioLine), new PropertyMetadata(25.5d));

        public double SpeedDropping
        {
            get => (double) GetValue(SpeedDroppingProperty) * Properties.Settings.Default.Rate;
            set => SetValue(SpeedDroppingProperty, value);
        }

        public static readonly DependencyProperty SpeedRaisingProperty = DependencyProperty.Register(
            "SpeedRaising", typeof(double), typeof(AudioLine), new PropertyMetadata(0d)); 
        //formerly 25.5d (which makes the frequencies appear delayed)

        public double SpeedRaising
        {
            get => (double) GetValue(SpeedRaisingProperty) * Properties.Settings.Default.Rate;
            set => SetValue(SpeedRaisingProperty, value);
        }

        #endregion

        private double Minimum => ActualWidth / ActualHeight * Maximum;

        public AudioLine()
        {
            InitializeComponent();
            Loaded += AdjustRectangle;
            SizeChanged += AdjustRectangle;
        }

        private void AdjustRectangle(object sender, EventArgs _)
        {
            AudioLineRectangle.RadiusX = Width / 2;
            AudioLineRectangle.RadiusY = Width / 2;
            AudioLineRectangle.Width = Width;
            AudioLineRectangle.Fill = Foreground;
            UpdateVisuals(this, FrequencyBin.Value);
        }

        private static void UpdateVisuals(AudioLine al, double value)
        {
            if (al.SpeedRaising > 0 && value > al._currentValue + al.SpeedRaising) al._currentValue = al._currentValue + al.SpeedRaising;
            else if (al.SpeedDropping > 0 && value < al._currentValue - al.SpeedDropping) al._currentValue = al._currentValue - al.SpeedDropping;
            else al._currentValue = value;

            al.AudioLineRectangle.Height = al._currentValue >= al.Maximum
                ? al.ActualHeight
                : (al._currentValue <= al.Minimum
                    ? al.Minimum / al.Maximum * al.ActualHeight
                    : al._currentValue / al.Maximum * al.ActualHeight);

            if (al.PitchColor)
                al.AudioLineRectangle.Fill =
                    new SolidColorBrush(al.Foreground.Color.Merge(al.ForegroundPitched.Color, value / al.Maximum));
        }
    }
}
