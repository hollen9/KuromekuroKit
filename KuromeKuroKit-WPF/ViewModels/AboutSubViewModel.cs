using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KuromeKuroKit_WPF.ViewModels
{
    public class AboutSubViewModel : BindableBase
    {
        public AboutSubViewModel()
        {
            System.Windows.MessageBox.Show("AboutSubViewModel");
        }
    }
}
