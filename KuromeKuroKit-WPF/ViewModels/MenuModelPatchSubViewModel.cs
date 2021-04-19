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
using System.Globalization;
using KuromeKuroKit_WPF.Properties;
using WPFLocalizeExtension.Engine;

namespace KuromeKuroKit_WPF.ViewModels
{
    public class MenuModelPatchSubViewModel : ViewModelBase
    {
        public MenuModelPatchSubViewModel(Prism.Ioc.IContainerExtension cp) : base(cp)
        {
            mahDialogCoordinator = cp.Resolve<IDialogCoordinator>();
            appState = cp.Resolve<AppState>();
        }

        private readonly IDialogCoordinator mahDialogCoordinator;
        private readonly AppState appState;
    }
}
