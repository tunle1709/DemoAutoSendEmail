using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace DemoAutoSendEmail
{
    public static class EmailService
    {
        private static readonly IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        private static readonly string smtpServer = config["EmailSettings:SmtpServer"];
        private static readonly int smtpPort = int.Parse(config["EmailSettings:Port"]);
        private static readonly bool enableSsl = bool.Parse(config["EmailSettings:EnableSsl"]);
        private static readonly string senderEmail = config["EmailSettings:SenderEmail"];
        private static readonly string senderPassword = config["EmailSettings:SenderPassword"];
        private static readonly string receiverEmail = config["EmailSettings:ReceiverEmail"];

        public static bool SendEmailWithAttachment(string folderName, string attachmentPath)
        {
            try
            {
                using var mail = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = "EHPHP24235/" + folderName + "/in",
                    Body = $"Kính gửi: quý khách công ty SITC\n\nChúng tôi xin kính gửi quý khách hàng hình ảnh của container {folderName}.\n\nTrân trọng cảm ơn.\nNam Đình Vũ Port.",
                    IsBodyHtml = false
                };

                mail.To.Add(receiverEmail);

                if (!File.Exists(attachmentPath))
                {
                    Console.WriteLine($"Attachment không tồn tại: {attachmentPath}");
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
                Console.WriteLine($"Email gửi thất bại: {ex.Message}");
                return false;
            }
        }
    }
}