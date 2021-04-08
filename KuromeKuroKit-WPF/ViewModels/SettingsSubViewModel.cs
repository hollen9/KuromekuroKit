using KuromeKuroKit_WPF.Models;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using KuromeKuroKit_WPF.Singletons;
using System.Threading.Tasks;

namespace KuromeKuroKit_WPF.ViewModels
{
    public class SettingsSubViewModel : ViewModelBase
    {
        public SettingsSubViewModel(Prism.Ioc.IContainerExtension cp) : base(cp)
        {
            mahDialogCoordinator = cp.Resolve<IDialogCoordinator>();
            appState = cp.Resolve<AppState>();

            ProfileInfos = new ObservableCollection<ProfileInfo>(appState.FoundedProfileInfos);
            ProfileInfos.Insert(0, new ProfileInfo { Filename = null, Name = "Add new..." });


            UsingProfileInfo = appState.UsingProfile == null ? null :
                ProfileInfos.Where(x => x.Filename == appState.UsingProfileInfo.Filename).FirstOrDefault();
            //if (UsingProfileInfo == null && ProfileInfos.Count == 1)
            //{
            //    ProfileInfos.Add(appState.UsingProfileInfo);
            //    UsingProfileInfo = ProfileInfos[1];
            //}

            appState.FoundedProfileInfosChanged += (s, e) =>
            {
                ProfileInfos = new ObservableCollection<ProfileInfo>(appState.FoundedProfileInfos);
                UsingProfileInfo = ProfileInfos[appState.UsingProfileInfoIndex];
                ProfileInfos.Insert(0, new ProfileInfo { Filename = null, Name = "Add new..." });
            };
            appState.UsingProfileChanged += (s, e) =>
            {
                ProfileInfos = new ObservableCollection<ProfileInfo>(appState.FoundedProfileInfos);
                UsingProfileInfo = ProfileInfos[appState.UsingProfileInfoIndex];
            };
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (isInitialized)
            {
                if (!isCreatingProfile && args.PropertyName == "UsingProfileInfo")
                {
                    if (UsingProfileInfo != null && UsingProfileInfo.Filename != null)
                    {
                        if (!appState.LoadUserProfile(UsingProfileInfo))
                        {
                            Task.Run(async () => _ = await mahDialogCoordinator.ShowMessageAsync(this, "_Error", "Cannot load selected profile."));
                        }
                    }
                }
            }
        }

        public string LogMessage
        {
            get { return logMsg; }
            set { SetProperty(ref logMsg, value); }
        }

        private readonly IDialogCoordinator mahDialogCoordinator;
        private readonly AppState appState;

        private bool isCreatingProfile = false;

        public async Task InitializeAsync()
        {


            isInitialized = true;
        }

        public ObservableCollection<ProfileInfo> ProfileInfos { get => profileInfos; set => SetProperty(ref profileInfos, value); }
        public ProfileInfo UsingProfileInfo { get => usingProfileInfo; set => SetProperty(ref usingProfileInfo, value); }

        public string CSGORootFolder { get => csgoRootFolder; set => SetProperty(ref csgoRootFolder, value); }

        //public string PlayerMdlInputPath { get => playerMdlInputPath; set => SetProperty(ref playerMdlInputPath, value); }



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

        public ICommand DeleteProfileCommand => new DelegateCommand<string>(async profileName => 
        {
            if (ProfileInfos.Count >= 2)
            {
                bool result = appState.DeleteUserProfile(profileName);
            }
            else
            {
                MessageBox.Show("無法刪除最後一個 Profile。");
            }
        });

        public ICommand SaveProfileCommand => new DelegateCommand<string>(async profileName =>
        {
            // MessageBox.Show(profileName);
            await mahDialogCoordinator.ShowMessageAsync(this, "Header", profileName);
        });

        public ICommand RenameProfileCommand => new DelegateCommand(async () =>
        {
            do
            {
                var mahInputResult = await mahDialogCoordinator.ShowInputAsync(this,
                    "_Rename user profile",
                    "_Input a new name for it.",
                    settings: new MetroDialogSettings
                    {
                        DefaultText = UsingProfileInfo.Name
                    });
                if (mahInputResult == null)
                {
                    // Cancel.
                    return;
                }
                string newProfileName = mahInputResult.Trim();

                if (newProfileName == string.Empty)
                {
                    _ = await mahDialogCoordinator.ShowMessageAsync(this, "_Error", "Cannot left blank!");
                    continue;
                }
                else
                {
                    if (!appState.RenameUsingUserProfile(newProfileName))
                    {
                        //Failed ranaming using profile, so
                        _ = await mahDialogCoordinator.ShowMessageAsync(this, "_Error", "Renaming failed.");
                        return;
                    }
                    //Succeed ranaming then...

                    //UsingProfileInfo.Filename = appState.UsingProfileInfo.Filename;
                    //UsingProfileInfo.Name = appState.UsingProfileInfo.Name;
                    // RaisePropertyChanged("UsingProfileInfo");

                    //int idx = ProfileInfos.Select((v,i)=> new
                    //{
                    //    o = v,
                    //    Index = i
                    //}).First(x=>x.o.Name == UsingProfileInfo.Name).Index;
                    //ProfileInfos[idx].Name = UsingProfileInfo.Name;
                    //ProfileInfos[idx].Filename = UsingProfileInfo.Filename;


                }

                break;
            } while (true);
        });

        public ICommand NewProfileCommand => new DelegateCommand(async () =>
        {
            isCreatingProfile = true;
            //if (appState.UsingProfile != null && appState.UsingProfile.IsChangesNotSaved)
            //{
            //    var mahPromptResult = await mahDialogCoordinator.ShowMessageAsync(this,
            //        "_Unsaved changes",
            //        "有為儲存的變更，在創建之前要儲存嗎?", MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,
            //        new MetroDialogSettings 
            //        {
            //            FirstAuxiliaryButtonText = "_取消",
            //            AffirmativeButtonText = "_儲存並創建",
            //            NegativeButtonText = "_刪除舊的創建新的",
            //            DefaultButtonFocus = MessageDialogResult.Affirmative,
            //            DialogResultOnCancel = MessageDialogResult.FirstAuxiliary
            //        });
            //}

            do
            {
                var mahInputResult = await mahDialogCoordinator.ShowInputAsync(this, "_New user profile", "_Input a name for it.");
                if (mahInputResult == null)
                {
                    // Cancel.
                    return;
                }
                string newProfileName = mahInputResult.Trim();

                if (newProfileName == string.Empty)
                {
                    _ = await mahDialogCoordinator.ShowMessageAsync(this, "_Error", "Cannot left blank!");
                    continue;
                }
                else
                {
                    if (ProfileInfos.Any(x => x.Name == newProfileName))
                    {
                        _ = await mahDialogCoordinator.ShowMessageAsync(this, "_Error", "Provided name aleady exists!");
                        continue;
                    }

                    _ = appState.CreateNewProfile(newProfileName);

                    //var newPI = new ProfileInfo { Name = newProfileName, Filename = newProfileName };

                    //ProfileInfos.Add(newPI);
                    //UsingProfileInfo = newPI;
                    //var oriUP = appState.UsingProfile;
                    //var newUP = UserProfile.GetDefaultProfile();
                    //newUP.CopyFrom(oriUP);
                    //newUP.ProfileName = newPI.Name;
                    //appState.UsingProfile = newUP;
                }

                break;
            } while (true);

            isCreatingProfile = false;
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
        private ObservableCollection<ProfileInfo> profileInfos;
        private string logMsg;
        private ProfileInfo usingProfileInfo;
        private bool isInitialized;
        private readonly Views.SettingsSub settingsView;
        private readonly Views.AboutSub aboutView;
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
