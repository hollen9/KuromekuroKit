using KuromeKuroKit_WPF.Modules;
using KuromeKuroKit_WPF.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace KuromeKuroKit_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ResourceDictionary>(()=> this.Resources);
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // base.ConfigureModuleCatalog(moduleCatalog);

            moduleCatalog.AddModule<MainViewModule>();
        }
    }
}
