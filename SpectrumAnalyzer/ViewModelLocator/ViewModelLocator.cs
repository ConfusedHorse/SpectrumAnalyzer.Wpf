using System.ComponentModel;
using SpectrumAnalyzer.Models;

namespace SpectrumAnalyzer.ViewModelLocator
{
    public class ViewModelLocator
    {
        #region Singleton

        private static ViewModelLocator _instance;

        public static ViewModelLocator Instance
        {
            get
            {
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return null;
                return _instance ?? (_instance = new ViewModelLocator());
            }
        }

        #endregion Singleton

        private AnalyzerViewModel _analyzerViewModel;

        public AnalyzerViewModel AnalyzerViewModel => _analyzerViewModel 
            ?? (_analyzerViewModel = 
                new AnalyzerViewModel(
                    Properties.Settings.Default.Bins,
                    Properties.Settings.Default.Rate,
                    Properties.Settings.Default.Normal));
    }
}