using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubTask
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var pc = PropertyChanged;
            if(pc != null)
            {
                pc(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
