using Prism.Regions;
using System.Windows;
using Prism.DryIoc;
using Prism.Ioc;
using System;

namespace KuromeKuroKit_WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IRegionManager regionManager;
        private readonly IContainerExtension container; //IContainerProvider
        
        //private readonly SettingsView settingsView;
        //private readonly AboutView aboutView;
        //private readonly IRegion mainRegion;

        public MainWindow(IRegionManager regionManager, Prism.Ioc.IContainerExtension container)
        {
            InitializeComponent();

            if (regionManager == null) 
            {

            }
            if (container == null)
            {

            }

            this.regionManager = regionManager;
            this.container = container;

            


            
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

            var mainRegion = regionManager.Regions["MainRegion"];
            mainRegion.RequestNavigate("AboutView");
            
        }
    }
}
