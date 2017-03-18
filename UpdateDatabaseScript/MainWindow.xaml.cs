using MahApps.Metro.Controls;
using UpdateDatabaseScript.ViewModel;

namespace UpdateDatabaseScript
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
        }
    }
}
