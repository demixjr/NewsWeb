
namespace BLL.DTO
{
    public class NewsDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public int Views { get; set; }

        public int CategoryId { get; set; }
        public CategoryDTO? Category { get; set; }

        public int AuthorId { get; set; }
        public UserDTO? Author { get; set; }
    }
}
