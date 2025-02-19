using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace DemoAutoSendEmail
{
    public static class FolderScanner
    {
        private static readonly IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        private static readonly string rootFolder = @"D:\imageContainer\";
        private static readonly string smtpServer = config["EmailSettings:SmtpServer"];
        private static readonly int smtpPort = int.Parse(config["EmailSettings:Port"]);
        private static readonly bool enableSsl = bool.Parse(config["EmailSettings:EnableSsl"]);
        private static readonly string senderEmail = config["EmailSettings:SenderEmail"];
        private static readonly string senderPassword = config["EmailSettings:SenderPassword"];
        private static readonly string receiverEmail = config["EmailSettings:ReceiverEmail"];

        public static void CheckAndSendEmails()
        {
            if (!Directory.Exists(rootFolder))
            {
                Console.WriteLine($"Root folder '{rootFolder}' does not exist!");
                return;
            }

            foreach (var folder in Directory.GetDirectories(rootFolder))
            {
                var folderName = Path.GetFileName(folder);
                if (DatabaseConfig.IsFolderSent(folderName))
                {
                    Console.WriteLine($"Folder '{folderName}' was already sent. Skipping...");
                    continue;
                }

                var images = Directory.GetFiles(folder, "*.jpg");
                if (images.Length == 0)
                {
                    Console.WriteLine($"No images found in folder '{folderName}'. Skipping...");
                    continue;
                }

                var zipPath = Path.Combine(rootFolder, folderName + ".zip");

                try
                {
                    if (File.Exists(zipPath))
                    {
                        File.Delete(zipPath);
                    }

                    ZipFile.CreateFromDirectory(folder, zipPath);
                    Console.WriteLine($"Created ZIP: {zipPath}");

                    if (SendEmailWithAttachment(folderName, zipPath))
                    {
                        DatabaseConfig.MarkFolderAsSent(folderName);
                        Console.WriteLine($"Email sent successfully for '{folderName}'!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing '{folderName}': {ex.Message}");
                }
                finally
                {
                    if (File.Exists(zipPath))
                    {
                        File.Delete(zipPath);
                        Console.WriteLine($"Deleted ZIP: {zipPath}");
                    }
                }
            }
        }

        private static bool SendEmailWithAttachment(string folderName, string attachmentPath)
        {
            try
            {
                using var mail = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = "EHPHP24235/" + folderName + "/in",
                    Body = $"Kính gửi: quý khách công ty SITC\n\nChúng tôi xin kính gửi quý khách hàng hỉnh ảnh của container {folderName}.\n\nTrân trọng cảm ơn.\nNam Đình Vũ Port.",
                    IsBodyHtml = false
                };

                mail.To.Add(receiverEmail);

                if (!File.Exists(attachmentPath))
                {
                    Console.WriteLine($"Attachment not found: {attachmentPath}");
                    return false;
                }

                mail.Attachments.Add(new Attachment(attachmentPath));

                using var smtpClient = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = enableSsl
                };

                smtpClient.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }
    }
}