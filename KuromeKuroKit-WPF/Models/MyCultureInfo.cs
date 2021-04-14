using KuromeKuroKit_WPF.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KuromeKuroKit_WPF.Models
{
    public class MyCultureInfo
    {
        public MyCultureInfo()
        {
            IsFakeOption = true;
        }

        public MyCultureInfo(string code)
        {
            if (code != "system")
            {
                NowCultureInfo = CultureInfo.GetCultureInfo(code);
            }
            else
            {
                NowCultureInfo = CultureInfo.InstalledUICulture;
            }
        }

        public CultureInfo NowCultureInfo { get; }
        public string DisplayName => IsFakeOption == true ? Strings._Word_Default : NowCultureInfo.DisplayName;
        public string NativeName => IsFakeOption == true ? string.Empty : (NowCultureInfo.NativeName == NowCultureInfo.DisplayName ? string.Empty : NowCultureInfo.NativeName);
        public bool IsFakeOption { get; } = false;
    }
}
