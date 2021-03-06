using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KuromeKuroKit_WPF.Modules
{
    public class MainSubModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Views.AboutSub>();
            containerRegistry.RegisterForNavigation<Views.SettingsSub>();
            containerRegistry.RegisterForNavigation<Views.MenuModelPatchSub>();
        }
    }
}
