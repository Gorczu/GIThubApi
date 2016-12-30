using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Xml.Serialization;

namespace GitHubTask
{
    public class GitHubConnector
    {
        private static readonly string GITHUB_URL = "https://api.github.com/users";
        private static readonly string TEMPORARY_FILE = Path.Combine(Path.GetTempPath(), "Smart4Aviation_GITHUB.json");
        private static readonly string TEMPORARY_NUM_OF_REPO_FILE = Path.Combine(Path.GetTempPath(), "Smart4Aviation_NUM_OF_REPO.xml");
        private readonly object saveFileObj = new object();
        private object obj = new object();

        public GitHubConnector(string name, string password)
        {
            this.name = name;
            this.password = password;
        }

        public string name { get; set; }
        public string password { get; set; }
        AuthorizationService AuthorizationService { get; set; }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public IEnumerable<UserViewModel> GetUsersDataByAPI()
        {
            string previousLink = "";
            string currentLink = GITHUB_URL;
            GitHubUserModel[] users = null;
            bool append = false;

            var authInfo = string.Format("{0}:{1}", name, password);
            authInfo = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(authInfo));
            List<UserNoRepositories> userRepoNumbers = new List<UserNoRepositories>();
            while (currentLink != previousLink)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(currentLink);
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "GET";
                request.UserAgent = "Anything";
                request.Timeout = 5000;
                request.Headers["Authorization"] = "Basic " + authInfo;
                
                try
                {
                    using (WebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            JavaScriptSerializer js = new JavaScriptSerializer();
                            var objText = reader.ReadToEnd();
                            users = (GitHubUserModel[])js.Deserialize(objText, typeof(GitHubUserModel[]));
                            //json serialization - now used xml with numbers in json to many not necessary data for offline mode
                            //lock (obj)
                            //{
                            //    using (StreamWriter writer = new StreamWriter(TEMPORARY_FILE, append))
                            //    {
                            //        writer.Write(objText);
                            //    }
                            //}
                        }
                        append = true;
                        previousLink = currentLink;
                        currentLink = response.Headers["Link"]
                                              .Split(';')[0]
                                              .Replace("<", null)
                                              .Replace(">", null);
                    }
                }
                catch (WebException)
                {
                    yield break;
                }
                if (users != null)
                {
                    foreach (var user in users)
                    {
                        bool breakLoop = false;
                        int numberOfRepositories = GetNumberOfUserRepository(authInfo, user, ref breakLoop);
                        if (breakLoop)
                            yield break;

                        //Saving date for offline mode
                        userRepoNumbers.Add(new UserNoRepositories(){ Login= user.Login, NoOfRepositories= numberOfRepositories });
                        //Overriding state one by one
                        SerializeNumOfRepository(userRepoNumbers);
                        CreateImageFileInTemp(user, authInfo);
                        
                        yield return new UserViewModel()
                        {
                            Login = user.Login,
                            NumOfRepos = numberOfRepositories,
                            Avatar = new Uri(user.Avatar_url, UriKind.Absolute),
                        };

                        
                    }
                }

                
            }

            
        }

        private void SerializeNumOfRepository(List<UserNoRepositories> userRepoNumbers)
        {
            try
            {
                lock (saveFileObj)
                {
                    using (TextWriter writer = new StreamWriter(TEMPORARY_NUM_OF_REPO_FILE))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<UserNoRepositories>));
                        serializer.Serialize(writer, userRepoNumbers);
                    }
                }
            }
            catch (Exception ex)
            {
                var res = MessageBox.Show(string.Format("Please close the file {0}", TEMPORARY_NUM_OF_REPO_FILE), "File save interupted", MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                    SerializeNumOfRepository(userRepoNumbers);
                else
                    return;
            }
        }

        public IEnumerable<UserViewModel> GetUsersDataFromLastConnection()
        {

            List<UserNoRepositories> users = new List<UserNoRepositories>(); ;
            try
            {
                using (TextReader reader = new StreamReader(TEMPORARY_NUM_OF_REPO_FILE))
                {
                    //JSon serialier
                    //json serialization - now used xml with numbers in json to many not necessary data for offline mode
                    //JavaScriptSerializer js = new JavaScriptSerializer();
                    //var objectsText = reader.ReadToEnd()
                    //                    .Replace("][", "]~[")
                    //                    .Split('~');

                    XmlSerializer serializer = new XmlSerializer(typeof(List<UserNoRepositories>));
                    users = (List<UserNoRepositories>)serializer.Deserialize(reader);
                }
            }
            catch (FileNotFoundException ex)
            {
                yield break;
            }
            if (users != null)
            {
                foreach (var user in users)
                {
                    yield return new UserViewModel()
                    {
                        Login = user.Login,
                        NumOfRepos = user.NoOfRepositories,
                        Avatar = new Uri(Path.Combine(Path.GetTempPath(), "GitHubAvatars", user.Login + ".jpg"), UriKind.Absolute),
                    };
                }
            }
        }

        private static void CreateImageFileInTemp(GitHubUserModel user, string authInfo)
        {
            HttpWebRequest lxRequest = (HttpWebRequest)WebRequest.Create(user.Avatar_url);
            lxRequest.Method = "GET";
            lxRequest.UserAgent = "Anything";
            lxRequest.Timeout = 5000;
            lxRequest.Headers["Authorization"] = "Basic " + authInfo;
            String lsResponse = string.Empty;
            using (HttpWebResponse lxResponse = (HttpWebResponse)lxRequest.GetResponse())
            {
                using (BinaryReader reader = new BinaryReader(lxResponse.GetResponseStream()))
                {
                    Byte[] lnByte = reader.ReadBytes(1 * 1024 * 1024 * 10);
                    string filePath = Path.Combine(Path.GetTempPath(), "GitHubAvatars", user.Login + ".jpg");
                    Directory.CreateDirectory(Directory.GetParent( filePath).FullName);
                    using (FileStream lxFS = new FileStream(filePath, FileMode.Create))
                    {
                        lxFS.Write(lnByte, 0, lnByte.Length);
                    }
                }
            }
        }

        private static int GetNumberOfUserRepository(string authInfo, GitHubUserModel user, ref bool breakLoop)
        {
            HttpWebRequest repoRequest = (HttpWebRequest)WebRequest.Create(user.Repos_url);
            repoRequest.ContentType = "application/json; charset=utf-8";
            repoRequest.Method = "GET";
            repoRequest.UserAgent = "Anything";
            repoRequest.Timeout = 5000;
            repoRequest.Headers["Authorization"] = "Basic " + authInfo;

            int numberOfRepositories = 0;
            try
            {
                using (WebResponse response = (HttpWebResponse)repoRequest.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var objText = reader.ReadToEnd();
                        dynamic jObj = js.DeserializeObject(objText);
                        numberOfRepositories = jObj.Length;
                    }
                }
            }
            catch (WebException)
            {
                breakLoop = true;
            }
            return numberOfRepositories;
        }
    }
}
