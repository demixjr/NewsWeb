namespace DAL.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
        public List<News> News { get; set; } = new List<News>();
    }
}
