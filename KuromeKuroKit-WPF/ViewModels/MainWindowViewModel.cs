using Prism.Mvvm;
using System.Threading.Tasks;
using System.Windows;

namespace KuromeKuroKit_WPF.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        private string _logMsg;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string LogMessage
        {
            get { return _logMsg; }
            set { SetProperty(ref _logMsg, value); }
        }

        private bool isDecompiledCompleted;

        public MainWindowViewModel()
        {
            isDecompiledCompleted = false;

            var cbApp = new Crowbar.App();
            Crowbar.Decompiler cbDe = new Crowbar.Decompiler();

            cbDe.CrowbarApp = cbApp;
            cbApp.Settings = new Crowbar.AppSettings();
            cbApp.Settings.SetDefaultDecompileReCreateFilesOptions();
            cbApp.Settings.DecompileMdlPathFileName = @"J:\Output\DOTNET_TEST_HOMURA\.COMPILED\models\player\custom_player\legacy\kuromekuro\homura\homura.mdl";
            cbApp.Settings.DecompileOutputFolderOption = Crowbar.AppEnums.DecompileOutputPathOptions.WorkFolder;
            cbApp.Settings.DecompileOutputFullPath = @"J:\Output\dotnet";

            cbDe.ProgressChanged += (s, e) => 
            {
                LogMessage = e.ProgressPercentage + "% ..\r\n" + LogMessage;
            };

            cbDe.RunWorkerCompleted += (s, e) =>
            {
                LogMessage = "Completed! Result: " + e.Result.ToString() + " #\r\n" + LogMessage;
                isDecompiledCompleted = true;
            };

            cbDe.Run();

            Task.Run(() => 
            {
                while (!isDecompiledCompleted) 
                {
                    Task.Delay(1000);
                }
                MessageBox.Show("Completed!");
            });
        }

        
    }
}
