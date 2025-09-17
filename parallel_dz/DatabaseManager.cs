using Microsoft.Data.Sqlite;
using System.IO;

public class DatabaseManager
{
    private const string DbFileName = "users_db.db";

    public static string GetDbPath()
    {
        // Просто возвращаем имя файла, так как SqliteConnection
        // создаст его в текущей рабочей директории приложения.
        return DbFileName;
    }

    public static void InitializeDatabase()
    {
        string dbPath = GetDbPath();
        string connectionString = $"Data Source={dbPath};";

        // Проверяем, существует ли файл.
        // Если да, ничего не делаем.
        if (File.Exists(dbPath))
        {
            Console.WriteLine("База данных уже существует. Пропускаем инициализацию.");
            return;
        }

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            Console.WriteLine("Создание и инициализация базы данных...");

            // Создание таблицы Users
            var createTableCommand = connection.CreateCommand();
            createTableCommand.CommandText = @"
                CREATE TABLE Users (
                    UserId INTEGER PRIMARY KEY,
                    FullName TEXT NOT NULL,
                    Username TEXT NOT NULL
                );
            ";
            createTableCommand.ExecuteNonQuery();

            // Вставка тестовых данных
            InsertUser(connection, "Иван Иванов", "ivan.ivanov");
            InsertUser(connection, "Мария Петрова", "maria.petrova");
            InsertUser(connection, "Алексей Смирнов", "alexey.smirnov");
            Console.WriteLine("База данных инициализирована тестовыми данными.");
        }
    }

    private static void InsertUser(SqliteConnection connection, string fullName, string username)
    {
        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Users (FullName, Username) VALUES (@fullName, @username);";
        command.Parameters.AddWithValue("@fullName", fullName);
        command.Parameters.AddWithValue("@username", username);
        command.ExecuteNonQuery();
    }
}