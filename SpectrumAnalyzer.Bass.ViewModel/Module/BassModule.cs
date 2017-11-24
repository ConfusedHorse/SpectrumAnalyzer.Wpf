using Ninject.Modules;

namespace SpectrumAnalyzer.Bass.ViewModel.Module
{
    public class BassModule : NinjectModule
    {
        public override void Load()
        {
            Bind<AnalyzerViewModel>().ToSelf().InSingletonScope();
        }
    }
}