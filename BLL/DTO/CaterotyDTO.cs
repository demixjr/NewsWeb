using DAL.Models;

namespace BLL.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<News> News { get; set; } = new List<News>();
    }
}
