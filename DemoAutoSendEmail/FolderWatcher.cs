using System;
using System.IO;
using System.Threading;

namespace DemoAutoSendEmail
{
    public static class FolderWatcher
    {
        private static FileSystemWatcher watcher;
        private static Timer timer;
        private static string currentFolder;

        public static void StartWatching(string rootFolder)
        {
            watcher = new FileSystemWatcher
            {
                Path = rootFolder,
                NotifyFilter = NotifyFilters.DirectoryName,
                Filter = "*.*"
            };

            watcher.Created += OnFolderCreated;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine($"Đang theo dõi thư mục: {rootFolder}");
        }

        private static void OnFolderCreated(object sender, FileSystemEventArgs e)
        {
            if (!Directory.Exists(e.FullPath))
                return;

            currentFolder = e.FullPath;
            Console.WriteLine($"Thư mục mới được tạo: {currentFolder}");

            timer?.Dispose();
            timer = new Timer(ProcessFolder, null, TimeSpan.FromMinutes(5), Timeout.InfiniteTimeSpan);
        }

        private static void ProcessFolder(object state)
        {
            FolderProcessor.ProcessFolder(currentFolder);
        }

        public static void ProcessExistingFolders(string rootFolder)
        {
            foreach (var folder in Directory.GetDirectories(rootFolder))
            {
                FolderProcessor.ProcessFolder(folder);
            }
        }
    }
}