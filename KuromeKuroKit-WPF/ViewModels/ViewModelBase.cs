using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KuromeKuroKit_WPF.ViewModels
{
    public abstract class ViewModelBase : BindableBase
    {
        protected readonly IRegionManager regionManager;
        protected readonly IContainerExtension container;
        protected readonly ResourceDictionary resourceDictionary;

        public ViewModelBase(IContainerExtension containerProvider) 
        {
            this.container = containerProvider;
            this.regionManager = containerProvider.Resolve<IRegionManager>();
            this.resourceDictionary = containerProvider.Resolve<ResourceDictionary>();


        }

    }
}
