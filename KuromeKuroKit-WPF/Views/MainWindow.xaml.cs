using Prism.Regions;
using System.Windows;
using Prism.DryIoc;
using Prism.Ioc;
using System;
using KuromeKuroKit_WPF.ViewModels;
using System.Threading.Tasks;
using KuromeKuroKit_WPF.Singletons;
using MahApps.Metro.Controls;
using System.ComponentModel;
using MahApps.Metro.Controls.Dialogs;

namespace KuromeKuroKit_WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly IRegionManager regionManager;
        private readonly ResourceDictionary resource;
        // private readonly AppState preference;
        private readonly IDialogCoordinator mahDialogCoordinator;
        private readonly MainWindowViewModel viewModel;
        
        //private readonly SettingsView settingsView;
        //private readonly AboutView aboutView;
        //private readonly IRegion mainRegion;

        public MainWindow(Prism.Ioc.IContainerExtension container)
        {
            // this.DataContext = new MainWindowViewModel(container);
            this.regionManager = container.Resolve<IRegionManager>();
            this.resource = container.Resolve<ResourceDictionary>();
            // this.preference = container.Resolve<AppState>();

            mahDialogCoordinator = DialogCoordinator.Instance;
            container.RegisterInstance(mahDialogCoordinator);

            DataContext = viewModel = new MainWindowViewModel(container);

            InitializeComponent();




            //mainRegion.Activate(mainRegion.GetView("AboutView"));
            //mainRegion.Add(aboutView);
            //mainRegion.Add(settingsView);

            // string mainRegion = "MainRegion";
            // regionManager.RegisterViewWithRegion(mainRegion, typeof(SettingsView));
            // regionManager.RegisterViewWithRegion(mainRegion, typeof(AboutView));

        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            var mainRegion = regionManager.Regions[this.resource["mainRegionName"] as string];
            mainRegion.RequestNavigate(Helpers.TypeHelper.CachedNameOf<SettingsSub>());
        }

        // ViewModel Ctor -> Windows_Init -> Windows_Loaded
        private void Window_Initialized(object sender, EventArgs e)
        {
            Task.Run(async ()=> await viewModel.InitializeAsync());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            viewModel.ExitCommand.Execute(null);
        }
    }
}
