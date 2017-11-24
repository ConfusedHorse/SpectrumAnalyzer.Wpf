using Ninject.Modules;
using SpectrumAnalyzer.Bass.ViewModel;

namespace SpectrumAnalyzer.ViewModel.Module
{
    public class BassModule : NinjectModule
    {
        public override void Load()
        {
            Bind<AnalyzerViewModel>().ToSelf().InSingletonScope();
        }
    }
}