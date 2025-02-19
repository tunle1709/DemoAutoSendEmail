using System.Windows;

namespace DemoAutoSendEmail
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DatabaseConfig.InitializeDatabase();
        }

        private void CheckAndSendEmailsButton_Click(object sender, RoutedEventArgs e)
        {
            FolderScanner.CheckAndSendEmails();
        }
    }
}