using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ovotan.Windows.Controls.TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        int index = 1;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var tab = new TabControlItem() { Header = "eeeee - " + index, Content = new TextBox() { Text = "wwww" } };
            MainTabControl.AddTab(tab);
            index++;
        }
    }
}