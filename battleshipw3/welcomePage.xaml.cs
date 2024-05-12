using System.Windows;

namespace battleshipw3
{
    /// <summary>
    /// Interaction logic for welcomePage.xaml
    /// </summary>
    public partial class welcomePage : Window
    {
        MainWindow mainWindow;
        public welcomePage()
        {
            InitializeComponent();
            mainWindow = new MainWindow();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ShowDialog();
        }
    }
}
