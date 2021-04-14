using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KuromeKuroKit_WPF.Singletons
{
    public class UserProfile
    {
        private string profileName;
        private string cSGOGameDir;
        private string languageCode;

        public UserProfile()
        {
        }

        public event PropertyValueChangedEventHandler PropertyValueChanged;

        public delegate void PropertyValueChangedEventHandler(object sender, PropertyValueChangedEventArgs e);

        private void OnPropertyValueChanged(string propertyName, object oldValue, object newValue)
        {
            PropertyValueChanged?.Invoke(this, new PropertyValueChangedEventArgs(propertyName, oldValue, newValue));
        }

        void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null) 
        {
            if (storage == null && value != null ||
                storage != null && !storage.Equals(value))
            {
                OnPropertyValueChanged(propertyName, storage, value);
            }
            storage = value;
        }

        public string ProfileName
        {
            get { return profileName; }
            set 
            {
                SetProperty(ref profileName, value);
            }
        }

        //public string ProfileName
        //{
        //    get => profileName; set
        //    {
        //        profileName = value;
        //        HasUnsavedChanges = true;
        //    }
        //}
        public string CSGOGameDir
        {
            get => cSGOGameDir; set
            {
                SetProperty(ref cSGOGameDir, value);
            }
        }
        public string LanguageCode
        {
            get => languageCode; set
            {
                SetProperty(ref languageCode, value);
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public bool HasUnsavedChanges { get; set; }

        public static UserProfile GetDefaultProfile()
        {
            var p = new UserProfile
            {
                profileName = "default",
                languageCode = "system"
            };
            return p;
        }

        // public bool IsLanguageDefault => LanguageCode == "system";
        public CultureInfo GetCultureInfo()
        {
            if (LanguageCode == "system")
            {
                return CultureInfo.InstalledUICulture;
            }
            else
            {
                return new CultureInfo(LanguageCode);
            }
        }

        public void CopyFrom(UserProfile oriUP)
        {
            this.profileName = oriUP.profileName;
            this.cSGOGameDir = oriUP.cSGOGameDir;
            this.languageCode = oriUP.languageCode;
        }
    }

    public class PropertyValueChangedEventArgs : EventArgs
    {
        public PropertyValueChangedEventArgs(string propertyName, object oldValue, object newValue)
        {
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }
        public virtual string PropertyName { get; }
        public virtual object OldValue { get; }
        public virtual object NewValue { get; }
    }
}
