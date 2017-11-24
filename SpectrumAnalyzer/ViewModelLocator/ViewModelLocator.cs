using System.ComponentModel;
using Ninject;
using SpectrumAnalyzer.Bass.ViewModel;
using SpectrumAnalyzer.Wpf.DependencyInjection;

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

        public AnalyzerViewModel AnalyzerViewModel => NinjectKernel.StandardKernel.Get<AnalyzerViewModel>();

        public static void Cleanup()
        {

        }
    }
}