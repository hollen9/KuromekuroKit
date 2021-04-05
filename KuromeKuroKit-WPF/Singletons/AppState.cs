using KuromeKuroKit_WPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KuromeKuroKit_WPF.Singletons
{
    public class AppState
    {
        private readonly string filename;

        public AppState()
        {
            filename = "app.json";
            pathConfigDir = Path.Combine(App.RootFolder, ".config");
            pathAppJson = Path.Combine(pathConfigDir, filename);

            FoundedProfileInfos = new List<ProfileInfo>();
        }

        [Newtonsoft.Json.JsonIgnore]
        public UserProfile UsingProfile
        {
            get => usingProfile;
            set 
            {
                usingProfile = value;
                usingProfileName = usingProfile.ProfileName;
            } 
        }
        public ProfileInfo UsingProfileInfo => new ProfileInfo 
        {
            Filename = System.Uri.EscapeDataString(usingProfileName) + ".cfg",
            Name = usingProfileName
        };

        [Newtonsoft.Json.JsonIgnore]
        public List<ProfileInfo> FoundedProfileInfos { get; set; }

        #region Json
        [Newtonsoft.Json.JsonProperty]
        private string usingProfileName;
        #endregion
        private UserProfile usingProfile;

        private readonly string pathConfigDir;
        private readonly string pathAppJson;

        public void Initialize()
        {
            AppState loadedState = null;

            _ = Directory.CreateDirectory(pathConfigDir);



            if (File.Exists(pathAppJson))
            {
                string jsonBody = File.ReadAllText(pathAppJson);
                try
                {
                    loadedState = Newtonsoft.Json.JsonConvert.DeserializeObject<AppState>(jsonBody);

                }
                catch (Newtonsoft.Json.JsonReaderException jrEx)
                {
                    loadedState = null;
                }
            }

            // 將讀取的應用程式 Fields，Deep Copy 到當前的 Instance。
            if (loadedState != null)
            {
                this.usingProfileName = loadedState.usingProfileName;
            }

            if (usingProfileName != null)
            {
                string encodedSafeProfileName = System.Uri.EscapeDataString(usingProfileName);
                string pathUserProfileJson = Path.Combine(pathConfigDir, encodedSafeProfileName + ".cfg");
                if (File.Exists(pathUserProfileJson))
                {
                    string jsonBody = File.ReadAllText(pathUserProfileJson);
                    try
                    {
                        UsingProfile = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfile>(jsonBody);
                    }
                    catch (Newtonsoft.Json.JsonReaderException jrEx)
                    {
                        UsingProfile = null;
                    }
                }
            }

            if (UsingProfile == null)
            {
                // 找不到使用者設定或是無效，所以採用預設的使用者設定。
                UsingProfile = UserProfile.GetDefaultProfile();
                 _ = SaveUsingUserProfile();
            }

            var uFs = Directory.EnumerateFiles(pathConfigDir, "*", SearchOption.TopDirectoryOnly)
                .Where(x=> x.EndsWith(".cfg"));
            foreach (var uF in uFs)
            {
                string name = Path.GetFileNameWithoutExtension(uF);
                string filename = name + ".cfg";

                FoundedProfileInfos.Add(new ProfileInfo 
                {
                    Filename = filename,
                    Name = System.Uri.UnescapeDataString(name)
                });
            }

        }

        public bool LoadUserProfile(ProfileInfo usingProfileInfo)
        {
            string jsonPath = Path.Combine(pathConfigDir, usingProfileInfo.Filename);
            if (!File.Exists(jsonPath))
            {
                return false;
            }
            try
            {
                string jsonBody = File.ReadAllText(jsonPath);
                UsingProfile = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfile>(jsonBody);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool SaveAppState()
        {
            string appJsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(this);

            try
            {
                File.WriteAllText(pathAppJson, appJsonBody);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SaveUsingUserProfile()
        {
            string userJsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(this.UsingProfile);

            string encodedSafeProfileName = System.Uri.EscapeDataString(usingProfileName);
            string pathUserProfileJson = Path.Combine(pathConfigDir, encodedSafeProfileName + ".cfg");

            try
            {
                File.WriteAllText(pathUserProfileJson, userJsonBody);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RenameUsingUserProfile(string newName)
        {
            if (UsingProfile == null)
            {
                return false;
            }
            try
            {
                File.Move(
                Path.Combine(pathConfigDir, UsingProfileInfo.Filename),
                Path.Combine(pathConfigDir, System.Uri.EscapeDataString(newName) + ".cfg"));
                
                UsingProfile.ProfileName = newName;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
