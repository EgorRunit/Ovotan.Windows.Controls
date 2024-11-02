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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainHeaders.Background = new SolidColorBrush(Colors.Blue);
        }
        static int _index = 1;
        void _addHeader(object sender, RoutedEventArgs e)
        {
            MainHeaders.AddHeader(new TabHeader() { Header = "Header - " + _index });
            _index++;
        }
    }
}