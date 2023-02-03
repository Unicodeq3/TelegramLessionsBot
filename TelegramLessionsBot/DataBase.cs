using MySql.Data.MySqlClient;
using Org.BouncyCastle.Pkcs;

namespace TelegramLessionsBot;

public class DataBase
{
    private static readonly string host = "localhost";
    private static readonly string port = "3306";
    private static readonly string username = "root";
    private static readonly string password = "root";
    private static readonly string database = "telegram";
    
    public MySqlConnection _connection = new MySqlConnection($"server={host};port={port};username={username};password={password};database={database}");

    public void openConnection()
    {
        if (_connection.State == System.Data.ConnectionState.Closed)
            _connection.Open();
    }

    public void closeConnection()
    {
        if(_connection.State == System.Data.ConnectionState.Open)
            _connection.Close();
    }

    public MySqlConnection getConnection()
    {
        return _connection;
    }
    
    
}