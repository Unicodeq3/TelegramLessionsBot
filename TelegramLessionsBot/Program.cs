using System.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using MySql.Data.MySqlClient;
using System.Timers;
using Timer = System.Timers.Timer;

namespace TelegramLessionsBot
{
    class Program
    {
        public static readonly string _botToken = "5716835807:AAF3uxwXYD0Jd84L9N5xZJvqfYHc3b-iwM8";
        public static readonly int _delay = 5;
        
        public static TelegramBotClient _client = new TelegramBotClient(_botToken);
        public static DataBase db = new DataBase();


        static void Main(string[] args)
        {
            Timer timer = new Timer(_delay * 1000);
            timer.Elapsed += TimerOnElapsed;
            timer.Start();
            
            _client.StartReceiving(UpdateHandler, PollingErrorHandler);
            Console.ReadLine();
            Thread.Sleep(-1);
        }

        static async void TimerOnElapsed(object? sender, ElapsedEventArgs e)
        {
            foreach (var u in GetAllUsersFromDataBase())
            {
                if(u.isEnabled)
                    await _client.SendTextMessageAsync(u.chatID, $"Пройшло {_delay}сек");
            }
        }


        private static async Task UpdateHandler(ITelegramBotClient arg1, Update arg2, CancellationToken arg3)
        {
            var msg = arg2.Message;
            if (msg.Text != null)
            {
                switch (msg.Text.ToLower())
                {
                    case var s when s.Contains("/start"):
                        User c = AddToDataBase(msg.Chat.Id);
                        if (c == null)
                            await _client.SendTextMessageAsync(msg.Chat.Id, $"Добрий день, щоб ввімкнути сповіщення про уроки напишіть /sub");
                        else
                            await _client.SendTextMessageAsync(msg.Chat.Id, $"Ви вже жмакали /start, {(c.isEnabled ? "і ви вже підаписанні на сповіщення": "щоб ввімкнути сповіщення про уроки напишіть /sub")}");
                        break;
                    case var s when s.Contains("/sub"):
                        EnableNotifications(msg.Chat.Id);
                        User mUser = GetUserFromDataBase(msg.Chat.Id);
                        if (mUser != null)
                            await _client.SendTextMessageAsync(msg.Chat.Id, $"Статус сповіщень: {(mUser.isEnabled ? "включені 🟢" : "виключені 🔴")}");
                        break;
                }
                
                    
                
            }
        }

        static User AddToDataBase(long chatId)
        {
            User searchUser = GetUserFromDataBase(chatId);
            
            if (searchUser != null)
            {
                return searchUser;
            }
            else
            {
                db.openConnection();
                if (new MySqlCommand($"INSERT INTO `users` (`chatID`, `isAdmin`, `isEnabled`) VALUES ({chatId}, 0, 0);", db._connection).ExecuteNonQuery() == 1)
                {
                    return null;
                }
                    
            }
            db.closeConnection();
            return searchUser;


        }

        static User getUser(DataTable table)
        {
            return new User(Convert.ToInt64(table.Rows[0][0]), Convert.ToBoolean(table.Rows[0][1]),
                Convert.ToBoolean(table.Rows[0][2]));
        }
        static User GetUserFromDataBase(long chatId)
        {
            db.openConnection();
            
            MySqlDataAdapter adapter = new MySqlDataAdapter($"SELECT * FROM `users` WHERE `chatID` = {chatId}", db._connection);
            DataTable table = new DataTable();
            adapter.Fill(table);
            db.closeConnection();
            if (table.Rows.Count > 0)
                return getUser(table);
            else
                return null;
            
        }
        
        static void EnableNotifications(long chatId)
        {
            db.openConnection();

            new MySqlCommand(
                $"UPDATE `users` SET `isEnabled`= NOT `isEnabled` WHERE  `chatID`={chatId} LIMIT 1",
                db.getConnection()).ExecuteNonQuery();
                db.closeConnection();
        }

        static List<User> GetAllUsersFromDataBase()
        {
            List<User> users = new List<User>();
            db.openConnection();
            
            MySqlDataAdapter adapter = new MySqlDataAdapter($"SELECT * FROM `users`", db._connection);
            DataTable table = new DataTable();
            adapter.Fill(table);
            db.closeConnection();
            if (table.Rows.Count > 0)
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    users.Add(new User(Convert.ToInt64(table.Rows[i][0]), Convert.ToBoolean(table.Rows[i][1]),
                        Convert.ToBoolean(table.Rows[i][2])));
                }
            else
                return null;
                return users;
        }
        private static async Task PollingErrorHandler(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            Console.WriteLine(arg2.ToString());
        }

        
    }
}