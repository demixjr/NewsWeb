
namespace BLL.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<NewsDTO> News { get; set; } = new List<NewsDTO>();
    }
}
