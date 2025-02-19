using Microsoft.Data.Sqlite;
using System;

namespace DemoAutoSendEmail
{
    public static class DatabaseConfig
    {
        private static readonly string connectionString = @"Data Source=C:\Users\tunda\todo.db";

        public static void InitializeDatabase()
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS SentFolders (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                        FolderName TEXT NOT NULL, 
                        SentDate DATETIME NOT NULL
                    );";
                command.ExecuteNonQuery();

                Console.WriteLine("Database đã được khởi tạo thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khởi tạo database: {ex.Message}");
            }
        }

        public static bool IsFolderSent(string folderName)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM SentFolders WHERE FolderName = @FolderName";
                command.Parameters.AddWithValue("@FolderName", folderName);

                var count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kiểm tra lỗi thư mục: {ex.Message}");
                return false;
            }
        }

        public static void MarkFolderAsSent(string folderName)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO SentFolders (FolderName, SentDate) VALUES (@FolderName, @SentDate)";
                command.Parameters.AddWithValue("@FolderName", folderName);
                command.Parameters.AddWithValue("@SentDate", DateTime.UtcNow); 

                command.ExecuteNonQuery();
                Console.WriteLine($"Đã đánh dấu '{folderName}' đã gửi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi đánh dấu thư mục là đã gửi: {ex.Message}");
            }
        }
    }
}
