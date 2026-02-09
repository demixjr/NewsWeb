namespace DAL.Models
{
    public class News
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public int Views { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int AuthorId { get; set; }
        public User? Author { get; set; }
    }
}
