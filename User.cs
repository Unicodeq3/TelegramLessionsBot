namespace TelegramLessionsBot;

public class User
{
    public long chatID { get; set; }
    public bool isAdmin { get; set; }
    public bool isEnabled { get; set; }

    public User(long id, bool admin, bool enabled)
    {
        chatID = id;
        isAdmin = admin;
        isEnabled = enabled;
    }
}