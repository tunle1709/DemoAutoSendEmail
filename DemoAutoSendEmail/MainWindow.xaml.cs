using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;

namespace DemoAutoSendEmail
{
    public partial class MainWindow : Window
    {
        private static readonly IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        private static readonly string rootFolder = config["FolderSettings:RootFolder"];

        public MainWindow()
        {
            InitializeComponent();
            DatabaseConfig.InitializeDatabase();
            FolderWatcher.ProcessExistingFolders(rootFolder); // Quét lại các thư mục chưa được gửi khi khởi động
        }

        private void StartWatchingButton_Click(object sender, RoutedEventArgs e)
        {
            FolderWatcher.StartWatching(rootFolder);
            MessageBox.Show("Đang theo dõi thư mục: " + rootFolder);
        }
    }
}