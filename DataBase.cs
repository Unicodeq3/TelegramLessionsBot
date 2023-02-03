using MySql.Data.MySqlClient;
using Org.BouncyCastle.Pkcs;

namespace TelegramLessionsBot;

public class DataBase
{
    private static readonly string host = "containers-us-west-34.railway.app";
    private static readonly string port = "7167";
    private static readonly string username = "root";
    private static readonly string password = "1K0OcH4a8kUW15O2Ofpq";
    private static readonly string database = "railway";
    
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