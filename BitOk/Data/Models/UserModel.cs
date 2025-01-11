namespace BitOk.Data.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }

        public string RoleString => Role.ToString();

        public static Role FromString(string role) => (Role)Enum.Parse(typeof(Role), role, true);
    }

    public enum Role
    {
        Admin,
        Client
    }
}
