using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace KuromeKuroKit_WPF.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(IContainerExtension containerProvider) : base(containerProvider) 
        {
            
        }

        private string title = "KuromeKuro Kit";
        private string logMsg;

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public string LogMessage
        {
            get { return logMsg; }
            set { SetProperty(ref logMsg, value); }
        }

        public string CSGORootFolder { get => csgoRootFolder; set => SetProperty(ref csgoRootFolder, value); }
        //public string PlayerMdlInputPath { get => playerMdlInputPath; set => SetProperty(ref playerMdlInputPath, value); }

        public ICommand NavigateCommand => new DelegateCommand<string>((path)=> 
        {
            if (path != null) 
            {
                regionManager.RequestNavigate(resourceDictionary["mainRegionName"] as string, path);
            }
        });

        public ICommand ShowSettingsViewCommand => new DelegateCommand(()=> 
        {
            mainRegion.Activate(settingsView);
            mainRegion.Deactivate(aboutView);

            // var settingsView = regionManager.Regions["MainRegion"].GetView(typeof(Views.SettingsView).Name);
            // var aboutView = regionManager.Regions["MainRegion"].GetView(typeof(Views.AboutView).Name);

            //regionManager.Regions["MainRegion"].Deactivate(aboutView);
            //regionManager.Regions["MainRegion"].Activate(settingsView);
        });
        public ICommand ShowAboutViewCommand => new DelegateCommand(() =>
        {
            mainRegion.Activate(aboutView);
            mainRegion.Deactivate(settingsView);

            //var settingsView = regionManager.Regions["MainRegion"].GetView(typeof(Views.SettingsView).Name);
            //var aboutView = regionManager.Regions["MainRegion"].GetView(typeof(Views.AboutView).Name);

            //regionManager.Regions["MainRegion"].Activate(aboutView);
            //regionManager.Regions["MainRegion"].Deactivate(settingsView);
        });

        public ICommand BrowseCSGORootCommand => new DelegateCommand(() =>
        {
            do
            {
                var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
                bool? dialogR = dialog.ShowDialog();
                if (!dialogR.HasValue)
                {
                    MessageBox.Show(Strings._Msg_AnErrorOccurred);
                    return;
                }
                if (dialogR.Value)
                {
                    string assumingPath = dialog.SelectedPath;
                    if (!File.Exists(Path.Combine(assumingPath, "csgo.exe")))
                    {
                        MessageBox.Show(Strings._Msg_CSGONotFounded);
                        continue;
                    }
                    CSGORootFolder = assumingPath;
                }
                break;
            } while (true);
        });

        //public ICommand BrowsePlayerMdlCommand => new DelegateCommand(() =>
        //{
        //    do
        //    {
        //        var dialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
        //        dialog.Multiselect = false;
        //        bool? dialogR = dialog.ShowDialog();
        //        if (!dialogR.HasValue)
        //        {
        //            MessageBox.Show(Strings._Msg_AnErrorOccurred);
        //            return;
        //        }
        //        if (dialogR.Value)
        //        {
        //            string assumingPath = dialog.FileName;
        //            PlayerMdlInputPath = assumingPath;
        //        }
        //        break;
        //    } while (true);
        //});

        private bool isDecompiledCompleted;
        private string csgoRootFolder;
        private readonly Views.SettingsView settingsView;
        private readonly Views.AboutView aboutView;
        private readonly IRegion mainRegion;
        //private string playerMdlInputPath;

        //public MainWindowViewModel() : base(null, null)
        //{
        //isDecompiledCompleted = false;

        //var cbApp = new Crowbar.App();
        //Crowbar.Decompiler cbDe = new Crowbar.Decompiler();

        //cbDe.CrowbarApp = cbApp;
        //cbApp.Settings = new Crowbar.AppSettings();
        //cbApp.Settings.SetDefaultDecompileReCreateFilesOptions();
        //cbApp.Settings.DecompileMdlPathFileName = @"J:\Output\DOTNET_TEST_HOMURA\.COMPILED\models\player\custom_player\legacy\kuromekuro\homura\homura.mdl";
        //cbApp.Settings.DecompileOutputFolderOption = Crowbar.AppEnums.DecompileOutputPathOptions.WorkFolder;
        //cbApp.Settings.DecompileOutputFullPath = @"J:\Output\dotnet";

        //cbDe.ProgressChanged += (s, e) => 
        //{
        //    LogMessage = e.ProgressPercentage + "% ..\r\n" + LogMessage;
        //};

        //cbDe.RunWorkerCompleted += (s, e) =>
        //{
        //    LogMessage = "Completed! Result: " + e.Result.ToString() + " #\r\n" + LogMessage;
        //    isDecompiledCompleted = true;
        //};

        //cbDe.Run();

        //Task.Run(() => 
        //{
        //    while (!isDecompiledCompleted) 
        //    {
        //        Task.Delay(1000);
        //    }
        //    MessageBox.Show("Completed!");
        //});
        //}


    }
}
