using KuromeKuroKit_WPF.Properties;
using KuromeKuroKit_WPF.Singletons;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPFLocalizeExtension.Engine;

namespace KuromeKuroKit_WPF.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(IContainerExtension containerProvider) : base(containerProvider)
        {
            //System.ComponentModel.PropertyChangedEventHandler pp = (pps, ppe) =>
            //{
            //    if (ppe.PropertyName == "HasUnsavedChanges")
            //    {
            //        UpdateWindowsTitle();
            //    }
            //};
            UserProfile.PropertyValueChangedEventHandler pp = (pps, ppe) =>
            {
                if (ppe.PropertyName == "HasUnsavedChanges")
                {
                    return;
                }
                appState.UsingProfile.HasUnsavedChanges = true;
                UpdateWindowsTitle();
            };

            mahDialogCoordinator = containerProvider.Resolve<IDialogCoordinator>();
            appState = containerProvider.Resolve<AppState>();
            // Strings.Culture = CultureInfo.GetCultureInfo("zh-TW");// CultureInfo.CurrentUICulture;

            
            appState.UsingProfileChanged += (s, e) =>
            {
                if (appState.UsingProfile != null)
                {
                    var ci = appState.UsingProfile.GetCultureInfo();
                    LocalizeDictionary.Instance.Culture = ci;
                    UpdateWindowsTitle();
                }

                if (appState.UsingProfile != null)
                {
                    // appState.UsingProfile.PropertyChanged += pp;
                    appState.UsingProfile.PropertyValueChanged += pp;
                }
            };

            appState.UsingProfileChanging += (s, e) =>
            {
                if (appState.UsingProfile != null)
                {
                    // appState.UsingProfile.PropertyChanged -= pp;
                }
            };

            appState.UnsavedChangesAppeared += (s, e) => 
            {
                if (appState.IsInitialized)
                {
                    UpdateWindowsTitle();
                }
            };
        }

        private void UpdateWindowsTitle()
        {
            if (!appState.IsInitialized)
            {
                return;
            }


            Title = 
                (appState.HasUnsavedChanges ? "*" : string.Empty) +
                Strings._MainWindowsTitle + " - " +
                (appState.UsingProfile.HasUnsavedChanges ? "*" : string.Empty) +
                appState.UsingProfile.ProfileName;

        }

        private string title = "KuromeKuro Kit";
        private readonly IDialogCoordinator mahDialogCoordinator;
        private readonly AppState appState;

        public string Title { get => title; set => SetProperty(ref title, value); }
        public ICommand NavigateCommand => new DelegateCommand<string>((path) =>
        {
            if (path != null)
            {
                regionManager.RequestNavigate(resourceDictionary["mainRegionName"] as string, path);
            }
        });

        public ICommand ExitCommand => new DelegateCommand(async () =>
        {
            try
            {
                var mahMsgPromptResult = await mahDialogCoordinator.ShowMessageAsync(this, "State saving",
                "_您正要關閉應用程式，請問你想要保存設定嗎?", MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,
                new MetroDialogSettings
                {
                    FirstAuxiliaryButtonText = "_取消",
                    AffirmativeButtonText = "_保存並離開",
                    NegativeButtonText = "_直接離開",
                    DefaultButtonFocus = MessageDialogResult.Affirmative,
                    DialogResultOnCancel = MessageDialogResult.FirstAuxiliary
                });

                switch (mahMsgPromptResult)
                {
                    case MessageDialogResult.Negative:
                        Application.Current.Shutdown(); 
                        return;
                    default:
                    case MessageDialogResult.Affirmative:
                        if (!appState.SaveAppState())
                        {
                            var isOk = await mahDialogCoordinator.ShowMessageAsync(this, "State saving",
                                "_app.json 儲存失敗，應用程式將關閉但不儲存。", MessageDialogStyle.AffirmativeAndNegative
                                , settings: new MetroDialogSettings()
                                {
                                    AnimateHide = false,
                                    ColorScheme = MetroDialogColorScheme.Inverted
                                });
                            if (isOk == MessageDialogResult.Negative)
                            {
                                return;
                            }
                        }
                        if (!appState.SaveUsingUserProfile())
                        {
                            var isOk = await mahDialogCoordinator.ShowMessageAsync(this, "State saving",
                                "_使用者設定儲存失敗，應用程式將關閉但不儲存。", settings:
                                new MetroDialogSettings()
                                {
                                    ColorScheme = MetroDialogColorScheme.Inverted
                                });
                            if (isOk == MessageDialogResult.Negative)
                            {
                                return;
                            }
                        }
                        Application.Current.Shutdown();
                        return;
                    case MessageDialogResult.FirstAuxiliary:
                        return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ExitCommand Error", ex.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        });

        public async Task InitializeAsync()
        {
            appState.Initialize();
        }
    }
}
