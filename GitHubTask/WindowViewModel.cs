using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace GitHubTask
{
    public class WindowViewModel : BaseViewModel
    {
        private ObservableCollection<UserViewModel> gitHubUsers;
        private bool noConnection = false;
        private bool noPreviousData = false;

        /// <summary>
        /// There is only some mocked data
        /// </summary>
        public WindowViewModel()
        {
            gitHubUsers = new ObservableCollection<UserViewModel>()
            {
                new UserViewModel(){ Login = "Krzysiu", NumOfRepos = 4, Avatar = new Uri("Img/images 2.jpg", UriKind.Relative)},
                new UserViewModel(){ Login = "Monisia", NumOfRepos = 2, Avatar = new Uri("Img/images.jpg", UriKind.Relative)},
            };
        }

        private readonly object _lockObject = new object();
        public WindowViewModel(IEnumerable<UserViewModel> data)
        {
            GitHubUsers = new ObservableCollection<UserViewModel>();
            BindingOperations.EnableCollectionSynchronization(GitHubUsers, _lockObject);
            Task.Factory.StartNew(() =>
                {
                    foreach (var item in data)
                    {
                        GitHubUsers.Add(item);
                        OnPropertyChanged("GitHubUsers");
                    }
                });
        }

        public Visibility AlerNoConnectionVisibility
        {
            get
            {
                return NoConnection && !NoPreviousData ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility AlerNoPreviousDataVisibility
        {
            
            get
            {
                return NoConnection && NoPreviousData ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public ObservableCollection<UserViewModel> GitHubUsers
        {
            get { return gitHubUsers; }
            set 
            { 
                gitHubUsers = value;
                OnPropertyChanged("GitHubUsers");
            }
        }

        public bool NoConnection
        {
            get { return noConnection; }
            set 
            { 
                noConnection = value;
                OnPropertyChanged("AlerNoConnectionVisibility");
                OnPropertyChanged("AlerNoPreviousDataVisibility");
            }
        }

        public bool NoPreviousData
        {
            get { return noPreviousData; }
            set 
            { 
                noPreviousData = value;
                OnPropertyChanged("AlerNoPreviousDataVisibility");
                OnPropertyChanged("AlerNoConnectionVisibility");
            }
        }
    }
}
