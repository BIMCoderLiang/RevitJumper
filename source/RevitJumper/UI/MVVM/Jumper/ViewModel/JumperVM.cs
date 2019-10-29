using RevitJumper.UI.MVVM.Jumper.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace RevitJumper.UI.MVVM.Jumper.ViewModel
{
    public class JumperVM : INotifyPropertyChanged
    {
        private string version { get; set; }
        public ObservableCollection<DisplayModel> InfoList { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public JumperVM(List<DisplayModel> list, string version)
        {
            InfoList = new ObservableCollection<DisplayModel>(list);
            this.version = version;
        }

        private DisplayModel _selectedInfo;
        public DisplayModel SelectedInfo
        {
            get
            {
                return _selectedInfo;
            }
            set
            {
                _selectedInfo = value;
                NotifyPropertyChanged("SelectedInfo");
            }
        }

        private ICommand _searchCmd;
        public ICommand SearchCmd
        {
            get { return _searchCmd ?? (_searchCmd = new GoSearch(SelectedInfo.Url, version)); }
            set
            {
                _searchCmd = value;
            }
        }
    }

    public class GoSearch : ICommand
    {
        private string url;
        private string version;
        public GoSearch(string url,string version)
        {
            this.url = url;
            this.version = version;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var revitdocs = "https://www.revitapidocs.com";
            var finalurl = $"{revitdocs}/{version}/{url}";
            System.Diagnostics.Process.Start(finalurl);
        }
    }
}
