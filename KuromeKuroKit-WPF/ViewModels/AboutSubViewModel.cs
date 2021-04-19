using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KuromeKuroKit_WPF.ViewModels
{
    public class AboutSubViewModel : BindableBase
    {
        private string html;
        public string Html
        {
            get { return html; }
            set { SetProperty(ref html, value); }
        }
        public AboutSubViewModel()
        {
            //System.Windows.MessageBox.Show("AboutSubViewModel");
            Html = KuromeKuroKit_WPF.Converters.HtmlToXaml.HtmlToXamlConverter.ConvertHtmlToXaml("<b>testBold</b>End!", false);
        }
    }
}
