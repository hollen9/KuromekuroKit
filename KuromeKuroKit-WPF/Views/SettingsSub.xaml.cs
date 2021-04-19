using KuromeKuroKit_WPF.Models;
using KuromeKuroKit_WPF.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Controls;
using Prism.Ioc;
using System;
using System.Threading.Tasks;

namespace KuromeKuroKit_WPF.Views
{
    /// <summary>
    /// Interaction logic for SettingsUserControl
    /// </summary>
    public partial class SettingsSub : UserControl
    {
        private readonly SettingsSubViewModel viewModel;

        public SettingsSub(Prism.Ioc.IContainerExtension cp)
        {
            cp.RegisterInstance(DialogCoordinator.Instance);

            DataContext = viewModel = new SettingsSubViewModel(cp);
            InitializeComponent();
        }

        private void cbUserProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = cbUserProfile;
            var selected = (ProfileInfo)cb.SelectedItem;
            if (selected != null)
            {
                if (selected.Filename == null)
                {
                    // If user selects "Add new", then set it back to original value.
                    if (e.RemovedItems != null && e.RemovedItems.Count > 0)
                    {
                        cb.SelectedItem = e.RemovedItems[0];
                    }
                    else
                    {
                        cb.SelectedItem = null;
                    }
                    viewModel.NewProfileCommand.Execute(null);
                }
                else
                {
                    Task.Run(async ()=> 
                    {
                        var result = await DialogCoordinator.Instance.ShowMessageAsync(this,
                            title: "_Unsaved change warning!",
                            message: "!!!",
                            style: MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,
                            settings: new MetroDialogSettings 
                            {
                                FirstAuxiliaryButtonText = "_取消",
                                AffirmativeButtonText = "_保存並離開",
                                NegativeButtonText = "_直接離開",
                                DefaultButtonFocus = MessageDialogResult.Affirmative,
                                DialogResultOnCancel = MessageDialogResult.FirstAuxiliary
                            }
                            );
                    });
                }
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Task.Run(async () => await viewModel.InitializeAsync());
        }
    }
}
