using Microsoft.Data.Sqlite;

namespace CodingChallenge.Services;

public interface IUserService
{
    string? GetWelcomeMessage(string username);
}

public class UserService : IUserService
{
    private readonly string _connectionString;
    private static readonly object _initLock = new(); // For thread safety
    private static bool _isInitialized = false;

    public UserService()
    {
        // Create a temporary SQLite database file
        string tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");
        _connectionString = $"Data Source={tempFile};";

        // Ensure the temp file is deleted when the application exits
        AppDomain.CurrentDomain.ProcessExit += (s, e) => File.Delete(tempFile);

        Initialize();
    }

    private void Initialize()
    {
        lock (_initLock)
        {
            if (_isInitialized) return;

            var sqliteConnection = new SqliteConnection(_connectionString);
            sqliteConnection.Open();

            using var command = sqliteConnection.CreateCommand();
            command.CommandText = @"
            CREATE TABLE Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL,
                Name TEXT NOT NULL
            );

            INSERT INTO Users (Username, Name) VALUES ('john_doe', 'John Doe');
            INSERT INTO Users (Username, Name) VALUES ('jane_doe', 'Jane Doe');
            ";
            command.ExecuteNonQuery();

            sqliteConnection.Close();
            _isInitialized = true;
        }
    }

    public string? GetWelcomeMessage(string username)
    {
        // Use SQLite connection to query data
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        string query = "SELECT Name FROM Users WHERE Username = @username";
        using var cmd = conn.CreateCommand();
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@username", username);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            string name = reader["Name"].ToString();
            return $"Hello, {name}";
        }
        else
        {
            return null;
        }
    }
}
