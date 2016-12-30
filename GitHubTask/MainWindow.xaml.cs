using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GitHubTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WindowViewModel ViewModel { get; set; }
        GitHubConnector GitHubConnector { get; set; }
        AuthorizationService AuthorizationService { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            AuthorizationService = new AuthorizationService();
            var authentication = AuthorizationService.ShowAuthenticationDialog();
            GitHubConnector = new GitHubConnector(authentication.Item1, authentication.Item2);
            IEnumerable<UserViewModel> data = GitHubConnector.GetUsersDataByAPI();
            if (data.Any())
                ViewModel = new WindowViewModel(data);

            else if (!data.Any())
            {
                data = GitHubConnector.GetUsersDataFromLastConnection();
                if (data.Any())
                {
                    ViewModel = new WindowViewModel(data);
                    ViewModel.NoConnection = true;
                }
            }
            if (!data.Any())
            {
                ViewModel = new WindowViewModel();
                ViewModel.NoConnection = true;
                ViewModel.NoPreviousData = true;
            }

            this.DataContext = ViewModel;
        }

        //Small hack ;)
        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Height != 0)
                Grid.Height +=e.NewSize.Height - e.PreviousSize.Height;
        }
    }
}
