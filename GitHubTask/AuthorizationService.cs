using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubTask
{
    public class AuthorizationService
    {
        public AuthorizationService()
        {
            ViewModel = new AuthorizationViewModel();
            View = new AuthorizationWindow();
            ViewModel.CloseWindow += View.Close;
            View.DataContext = ViewModel;
        }

        public Tuple<string, string> ShowAuthenticationDialog()
        {
            View.ShowDialog();
            return new Tuple<string, string>(ViewModel.Login, View.Password.Password);
        }

        private AuthorizationViewModel ViewModel { get; set; }

        private AuthorizationWindow View { get; set; }
    }
}
