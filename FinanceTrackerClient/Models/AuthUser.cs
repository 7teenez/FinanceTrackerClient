namespace FinanceTrackerClient.Models
{
    public class AuthUser
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; } //збережемо як хеш
    }
}