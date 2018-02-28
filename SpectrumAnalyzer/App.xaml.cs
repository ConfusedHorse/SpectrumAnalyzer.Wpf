using System.Windows.Threading;
using BlurryControls.DialogFactory;

namespace SpectrumAnalyzer
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App
    {
        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            BlurryMessageBox.Show(e.Exception.Message);
        }
    }
}
