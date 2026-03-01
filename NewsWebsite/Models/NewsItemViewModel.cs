using BLL.DTO;

namespace NewsWebsite.Models
{
    public class NewsItemViewModel
    {
        public NewsDTO News { get; set; } = null!;
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}