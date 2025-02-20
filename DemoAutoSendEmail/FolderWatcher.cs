using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace DemoAutoSendEmail
{
    public static class FolderWatcher
    {
        private static ConcurrentQueue<string> folderQueue = new ConcurrentQueue<string>();
        private static Timer emailTimer;
        private static FileSystemWatcher watcher;

        public static void StartWatching(string rootFolder)
        {
            Console.WriteLine($"Đang theo dõi thư mục: {rootFolder}");

            // Khởi động FileSystemWatcher để theo dõi thư mục liên tục
            watcher = new FileSystemWatcher
            {
                Path = rootFolder,
                NotifyFilter = NotifyFilters.DirectoryName,
                Filter = "*.*"
            };

            watcher.Created += OnFolderCreated;
            watcher.EnableRaisingEvents = true;

            // Khởi động Timer để gửi email mỗi 5 phút
            emailTimer = new Timer(SendEmails, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }

        private static void OnFolderCreated(object sender, FileSystemEventArgs e)
        {
            if (Directory.Exists(e.FullPath))
            {
                folderQueue.Enqueue(e.FullPath);
            }
        }

        private static void SendEmails(object state)
        {
            while (folderQueue.TryDequeue(out var folderPath))
            {
                FolderProcessor.ProcessFolder(folderPath);
            }
        }
    }
}