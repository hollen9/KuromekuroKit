using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KuromeKuroKit_WPF.Singletons
{
    public class UserProfile
    {
        private string profileName;
        private string cSGOGameDir;
        private string languageCode;

        public string ProfileName
        {
            get => profileName; set
            {
                profileName = value;
                isChangesNotSaved = true;
            }
        }
        public string CSGOGameDir
        {
            get => cSGOGameDir; set
            {
                cSGOGameDir = value;
                isChangesNotSaved = true;
            }
        }
        public string LanguageCode
        {
            get => languageCode; set
            {
                languageCode = value;
                isChangesNotSaved = true;
            }
        }

        private bool isChangesNotSaved;

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
