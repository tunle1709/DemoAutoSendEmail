using System;
using System.IO;
using System.IO.Compression;

namespace DemoAutoSendEmail
{
    public static class FolderProcessor
    {
        public static void ProcessFolder(string folderPath)
        {
            var folderName = Path.GetFileName(folderPath);
            if (DatabaseConfig.IsFolderSent(folderName))
            {
                Console.WriteLine($"Folder '{folderName}' đã được gửi rồi. Đang bỏ qua...");
                return;
            }

            var images = Directory.GetFiles(folderPath, "*.jpg");
            if (images.Length == 0)
            {
                Console.WriteLine($"Không tìm thấy hình ảnh trong thư mục '{folderName}'. Đang bỏ qua...");
                return;
            }

            var zipPath = Path.Combine(Path.GetDirectoryName(folderPath), folderName + ".zip");

            try
            {
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }

                ZipFile.CreateFromDirectory(folderPath, zipPath);
                Console.WriteLine($"Created ZIP: {zipPath}");

                if (EmailService.SendEmailWithAttachment(folderName, zipPath))
                {
                    DatabaseConfig.MarkFolderAsSent(folderName);
                    Console.WriteLine($"Email đã được gửi thành công cho '{folderName}'!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Xử lý lỗi '{folderName}': {ex.Message}");
            }
            finally
            {
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                    Console.WriteLine($"Xóa ZIP: {zipPath}");
                }
            }
        }
    }
}