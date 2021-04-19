using KuromeKuroKit_WPF.Models;
using KuromeKuroKit_WPF.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Controls;
using Prism.Ioc;
using System;
using System.Threading.Tasks;
using KuromeKuroKit_WPF.Properties;

namespace KuromeKuroKit_WPF.Views
{
    /// <summary>
    /// Interaction logic for GamePatchUserControl
    /// </summary>
    public partial class MenuModelPatchSub : UserControl
    {
        private readonly MenuModelPatchSubViewModel viewModel;

        public MenuModelPatchSub(Prism.Ioc.IContainerExtension cp)
        {
            cp.RegisterInstance(DialogCoordinator.Instance);

            DataContext = viewModel = new MenuModelPatchSubViewModel(cp);
            InitializeComponent();

            
        }

        protected override void OnInitialized(EventArgs e)
        {
            string html = Strings.MenuModelPatchDisclaimerHtml;
            string xaml = KuromeKuroKit_WPF.Converters.HtmlToXaml.HtmlToXamlConverter.ConvertHtmlToXaml(html, true);
            var fd = System.Windows.Markup.XamlReader.Parse(xaml) as System.Windows.Documents.FlowDocument;
            rtbDisclaimer.Document = fd;

            base.OnInitialized(e);
        }
    }
}
