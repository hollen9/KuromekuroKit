using KuromeKuroKit_WPF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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


        public event EventHandler FoundedProfileInfosChanged;




        public event EventHandler UsingProfileChanged;

        protected virtual void OnUsingProfileChanged()
        {
            UsingProfileChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnFoundedProfileInfosChanged()
        {
            FoundedProfileInfosChanged?.Invoke(this, EventArgs.Empty);
        }

        [Newtonsoft.Json.JsonIgnore]
        public UserProfile UsingProfile
        {
            get => usingProfile;
            set
            {
                if (FoundedProfileInfos.Count >= 1)
                {
                    if (value != null)
                    {
                        int idx = -1;

                        try
                        {
                            idx = FoundedProfileInfos.Select((v, i) => new
                            {
                                o = v,
                                Index = i
                            }).First(x => x.o.Name == value.ProfileName).Index;
                        }
                        catch (InvalidOperationException opEx)
                        {}

                        if (idx > -1)
                        {
                            UsingProfileInfoIndex = idx;
                        }
                        else
                        {
                            throw new Exception("只能設定 FoundedUserInfos 存在的 UsingProfile。");
                        }
                    }
                    else
                    {
                        throw new Exception("Cannot set using profile to null.");
                    }
                }

                usingProfile = value;
                usingProfileName = value.ProfileName;
                OnUsingProfileChanged();
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public ProfileInfo UsingProfileInfo
        {
            get 
            {
                if (usingProfileName == null)
                {
                    return null;
                }

                return new ProfileInfo
                {
                    Filename = System.Uri.EscapeDataString(usingProfileName) + ".cfg",
                    Name = usingProfileName
                };
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public List<ProfileInfo> FoundedProfileInfos { get; set; }

        #region Json
        [Newtonsoft.Json.JsonProperty]
        private string usingProfileName;

        [Newtonsoft.Json.JsonIgnore]
        public int UsingProfileInfoIndex { get; private set; }
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
                    catch(Exception ex)
                    {
                        UsingProfile = null;
                    }
                }
                OnUsingProfileChanged();
            }

            if (UsingProfile == null)
            {
                // 找不到使用者設定或是無效，所以採用預設的使用者設定。
                UsingProfile = UserProfile.GetDefaultProfile();
                 _ = SaveUsingUserProfile();
            }

            FindProfileInfos();

        }

        public void FindProfileInfos()
        {
            FoundedProfileInfos.Clear();

            var uFs = Directory.EnumerateFiles(pathConfigDir, "*", SearchOption.TopDirectoryOnly)
                .Where(x => x.EndsWith(".cfg"));
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

            FoundedProfileInfos = FoundedProfileInfos.OrderBy(x => x.Name).ToList();

            OnFoundedProfileInfosChanged();
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

        public bool DeleteUserProfile(string profileName)
        {
            string jsonPath = Path.Combine(pathConfigDir, GetFilename(profileName));
            if (!File.Exists(jsonPath))
            {
                Debug.WriteLine("Found no profile match the given filename.");
                return false;
            }

            bool isLoadSucceed;

            if (UsingProfileInfo.Name == profileName)
            {
                if (UsingProfileInfoIndex == FoundedProfileInfos.Count - 1)
                {
                    isLoadSucceed = LoadUserProfile(FoundedProfileInfos[0]);
                }
                else
                {
                    isLoadSucceed = LoadUserProfile(FoundedProfileInfos[UsingProfileInfoIndex + 1]);
                }
            }
            else
            {
                isLoadSucceed = true;
            }
            
            if (!isLoadSucceed)
            {
                Debug.WriteLine("It didn't load successfully.");
                return false;
            }

            try
            {
                File.Delete(jsonPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

            try
            {
                FindProfileInfos();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
            return true;
        }

        public string GetFilename(string profileName)
        {
            return System.Uri.EscapeDataString(profileName) + ".cfg";
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

        public bool CreateNewProfile(string newProfileName)
        {
            try
            {
                var oriP = UsingProfile;
                var newP = new UserProfile();
                newP.CopyFrom(oriP);
                newP.ProfileName = newProfileName;
                FoundedProfileInfos.Add(new ProfileInfo { Name = newProfileName, Filename = GetFilename(newProfileName) });
                FoundedProfileInfos = FoundedProfileInfos.OrderBy(x => x.Name).ToList();
                UsingProfile = newP;
                return SaveUsingUserProfile();
            }
            catch (Exception ex)
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
                string newFilePath = Path.Combine(pathConfigDir, System.Uri.EscapeDataString(newName) + ".cfg");

                File.Move(
                Path.Combine(pathConfigDir, UsingProfileInfo.Filename),
                newFilePath);

                var readFromFile = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfile>(File.ReadAllText(newFilePath));
                readFromFile.ProfileName = newName;
                string writeToFile = Newtonsoft.Json.JsonConvert.SerializeObject(readFromFile);
                File.WriteAllText(newFilePath, writeToFile);

                UsingProfile.ProfileName = newName;
                usingProfileName = newName;
                FindProfileInfos();
                OnUsingProfileChanged();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
