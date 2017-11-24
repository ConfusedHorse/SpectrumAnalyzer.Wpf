using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SpectrumAnalyzer.Bass.ViewModel;
using SpectrumAnalyzer.Bass.ViewModel.Helpers;

namespace SpectrumAnalyzer.Controls
{
    /// <summary>
    /// Interaktionslogik für AudioLine.xaml
    /// </summary>
    public partial class AudioLine : UserControl
    {
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

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double), typeof(AudioLine), new PropertyMetadata(10d));
        
        public double Value
        {
            get => (double) GetValue(ValueProperty);
            set
            {
                double newValue;
                if (value > Value + SpeedRaising) newValue = Value + SpeedRaising;
                else if (value < Value - SpeedDropping) newValue = Value - SpeedDropping;
                else newValue = value;

                SetValue(ValueProperty, newValue);
                if (newValue >= Maximum) AudioLineRectangle.Height = Height;
                else
                    AudioLineRectangle.Height = newValue <= Minimum
                        ? Minimum / Maximum * Height
                        : newValue / Maximum * Height;

                if (PitchColor)
                    AudioLineRectangle.Fill =
                        new SolidColorBrush(Foreground.Color.Merge(ForegroundPitched.Color, value / Maximum));
            }
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum", typeof(double), typeof(AudioLine), new PropertyMetadata(255d));

        public double Maximum
        {
            get => (double) GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public static readonly DependencyProperty SpeedDroppingProperty = DependencyProperty.Register(
            "SpeedDropping", typeof(double), typeof(AudioLine), new PropertyMetadata(25.5d));

        public double SpeedDropping
        {
            get => (double) GetValue(SpeedDroppingProperty) * AnalyzerViewModel.Hertz;
            set => SetValue(SpeedDroppingProperty, value);
        }

        public static readonly DependencyProperty SpeedRaisingProperty = DependencyProperty.Register(
            "SpeedRaising", typeof(double), typeof(AudioLine), new PropertyMetadata(25.5d));

        public double SpeedRaising
        {
            get => (double) GetValue(SpeedRaisingProperty);
            set => SetValue(SpeedRaisingProperty, value);
        }

        #endregion

        private double Minimum => Width / Height * Maximum;

        public static AudioLine Default => new AudioLine
        {
            Margin = new Thickness(3),
            Width = 3,
            Maximum = AnalyzerViewModel.Normal
        };

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
            Value = Value;
        }
    }
}
