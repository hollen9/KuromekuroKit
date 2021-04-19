using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace KuromeKuroKit_WPF.Views
{
    /// <summary>
    /// Interaction logic for SettingsUserControl
    /// </summary>
    public partial class AboutSub : UserControl
    {
        public AboutSub()
        {
            InitializeComponent();

            var fd = XamlReader.Parse(KuromeKuroKit_WPF.Converters.HtmlToXaml.HtmlToXamlConverter.ConvertHtmlToXaml("<b>B<font color=#FF0000>o</font>ld</b>Text", true)) as FlowDocument;
            rtbDis.Document = fd;
        }
    }
}
