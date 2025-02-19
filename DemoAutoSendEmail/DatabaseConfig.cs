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

                Console.WriteLine("Database initialized successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");
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
                Console.WriteLine($"Error checking folder: {ex.Message}");
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
                Console.WriteLine($"Marked '{folderName}' as sent.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking folder as sent: {ex.Message}");
            }
        }
    }
}
