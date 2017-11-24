using System;
using System.Windows;

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
            Application.Current.Shutdown();
        }
    }
}
