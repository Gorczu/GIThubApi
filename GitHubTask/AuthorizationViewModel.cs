using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GitHubTask
{
    public class AuthorizationViewModel : BaseViewModel
    {
        private string login;
        public string Login
        {
            get { return login; }
            set 
            { 
                login = value;
                OnPropertyChanged("Login");
            }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set 
            { 
                password = value;
                OnPropertyChanged("Password");

            }
        }

        public ICommand OkCommand
        {
            get
            {
                return new DelegateCommand(OkExecute);
            }
        }

        public Action CloseWindow;
        private void OkExecute(object obj)
        {
            if (CloseWindow != null)
                CloseWindow();
        }

    }
}
