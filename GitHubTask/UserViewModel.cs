using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubTask
{
    public class UserViewModel : BaseViewModel
    {
        private Uri avatar;
        private string login;
        private int numOfRepos;

        public Uri Avatar
        {
            get { return avatar; }
            set 
            {
                avatar = value;
                OnPropertyChanged("Avatar");
            }
        }

        public string Login
        {
            get { return login; }
            set 
            { 
                login = value;
                OnPropertyChanged("Login");
            }
        }
        public int NumOfRepos
        {
            get { return numOfRepos; }
            set 
            { 
                numOfRepos = value;
                OnPropertyChanged("NumOfRepos");
            }
        }
    }
}
