using System;
using System.Linq;
using System.Windows.Media;
using SpectrumAnalyzer.Controls;
using SpectrumAnalyzer.Singleton;

namespace SpectrumAnalyzer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Closed += OnClosed;
        }

        private static void OnClosed(object sender, EventArgs eventArgs)
        {
            ViewModelLocator.Instance.AnalyzerViewModel.Stop();
        }

        private void BlurryColorPicker_OnColorChanged(object sender, Color color)
        {
            foreach (var audioSpectrum in Spectrum.Children.OfType<AudioSpectrum>())
                audioSpectrum.ForegroundPitched = new SolidColorBrush(color);
            foreach (var audioSpectrum in Reflection.Children.OfType<AudioSpectrum>())
                audioSpectrum.ForegroundPitched = new SolidColorBrush(color);
        }
    }
}
