namespace BitOk.Auth
{
    public class UserSession
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
    }
}