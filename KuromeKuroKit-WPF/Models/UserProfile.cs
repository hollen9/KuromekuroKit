using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KuromeKuroKit_WPF.Singletons
{
    public class UserProfile
    {
        public string ProfileName { get; set; }
        public string CSGOGameDir { get; set; }
        public string LanguageCode { get; set; }

        public static UserProfile GetDefaultProfile()
        {
            var p = new UserProfile 
            {
                ProfileName = "default",
                LanguageCode = "system"
            };
            return p;
        }

        public void CopyFrom(UserProfile oriUP)
        {
            this.ProfileName = oriUP.ProfileName;
            this.CSGOGameDir = oriUP.CSGOGameDir;
            this.LanguageCode = oriUP.LanguageCode;
        }
    }
}
