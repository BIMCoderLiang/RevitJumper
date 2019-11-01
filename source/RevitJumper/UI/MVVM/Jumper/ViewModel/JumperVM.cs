using Autodesk.Revit.DB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RevitJumper.UI.MVVM.Jumper.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace RevitJumper.UI.MVVM.Jumper.ViewModel
{
    public class JumperVM : INotifyPropertyChanged
    {
        private string version { get; set; }
        public ICommand SearchCmd { get; set; }
        public ObservableCollection<DisplayModel> InfoList { get; set; }
        public ObservableCollection<string> TypeNames { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public JumperVM(Document doc, List<ElementId> elemIds, string version)
        {
            InfoList = new ObservableCollection<DisplayModel>();
           
            this.version = version;
            if (elemIds != null && elemIds.Any())
            {
                if (elemIds.Count() == 1)
                {
                    IsVisible = false;
                    var targetId = elemIds.FirstOrDefault();
                    var target = doc.GetElement(targetId);
                    if (target != null)
                    {
                        var searchcontent = target.GetType().Name;
                        if (!string.IsNullOrEmpty(searchcontent))
                        {
                            QueryInput = searchcontent;                           
                        }
                    }
                }
                else
                {
                    IsVisible = true;
                    TypeNames = new ObservableCollection<string>();
                    foreach (var eleId in elemIds)
                    {
                        var elem = doc.GetElement(eleId);
                        if (elem != null)
                        {
                            var typename = elem.GetType().Name;
                            if (!string.IsNullOrEmpty(typename) &&
                                !string.IsNullOrWhiteSpace(typename) &&
                                !TypeNames.Contains(typename))
                            {
                                TypeNames.Add(typename);                              
                            }
                        }
                    }
                    NotifyPropertyChanged("TypeNames");
                    SelectedType = TypeNames.FirstOrDefault();
                }
            }
            SearchCmd = new RelayCommand(ExecuteSearch, CanRun);
        }

        private bool CanRun(object para)
        {
            return true;
        }

        private string _selectedType;
        public string SelectedType
        {
            get
            {
                return _selectedType;
            }
            set
            {
                _selectedType = value;
                NotifyPropertyChanged("SelectedType");
                QueryInput = _selectedType;
            }
        }

        private string _queryInput;
        public string QueryInput
        {
            get
            {
                return _queryInput;
            }
            set
            {
                _queryInput = value;
                NotifyPropertyChanged("QueryInput");
                InfoList.Clear();
                var query = new Query();
                var array = query.GetSearchResult(_queryInput);
                if (array != null)
                {
                    foreach (var info in array)
                    {
                        var relatedkey = info["value"].ToString();
                        var data = info["data"].ToString();
                        JObject datas = (JObject)JsonConvert.DeserializeObject(data);
                        var description = datas["description"].ToString();
                        var url = datas["url"].ToString();

                        var model = new DisplayModel()
                        {
                            RelatedKey = relatedkey,
                            Description = description,
                            Url = url,
                        };
                        InfoList.Add(model);
                    }
                    NotifyPropertyChanged("InfoList");
                    SelectedInfo = InfoList.FirstOrDefault();
                }                              
            }
        }

        private bool _isVisible;
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
                NotifyPropertyChanged("IsVisible");
            }
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


        private void ExecuteSearch(object param)
        {
            var url = SelectedInfo == null ? string.Empty : SelectedInfo.Url;
            var revitdocs = "https://www.revitapidocs.com";
            var finalurl = $"{revitdocs}/{version}/{url}";
            System.Diagnostics.Process.Start(finalurl);
        }
    }

    public class RelayCommand : ICommand
    {
        public Action<object> ExecuteCommand { get; set; }
        public Func<object, bool> CanExecuteCommand { get; set; }

        public RelayCommand(Action<object> execute, Func<object, bool> canexecute)
        {
            ExecuteCommand = execute;
            CanExecuteCommand = canexecute;
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteCommand(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            ExecuteCommand(parameter);
        }
    }
}
