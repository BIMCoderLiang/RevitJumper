
using RevitJumper.UI.MVVM.Jumper.Model;
using RevitJumper.UI.MVVM.Jumper.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace RevitJumper.UI.MVVM.Jumper.View
{
    /// <summary>
    /// JumperWnd.xaml 的交互逻辑
    /// </summary>
    public partial class JumperWnd : Window
    {
        public JumperWnd(List<DisplayModel> models, string content, string version)
        {
            InitializeComponent();
            DataContext = new JumperVM(models, content, version);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
