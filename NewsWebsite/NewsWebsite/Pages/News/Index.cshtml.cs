using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Pages;

namespace NewsWebsite.Pages.News
{
    public class IndexModel : BasePageModel
    {
        private readonly INewsService _newsService;
        private readonly ICategoryService _categoryService;

        public IndexModel(INewsService newsService, ICategoryService categoryService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
        }

        public List<NewsDTO> NewsList { get; set; } = new();
        public string? CategoryName { get; set; }

        [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        public void OnGet()
        {
            try
            {
                if (CategoryId.HasValue)
                {
                    NewsList = _newsService.GetByCategory(CategoryId.Value);
                    var cat = _categoryService.GetAllCategories()
                                .FirstOrDefault(c => c.Id == CategoryId.Value);
                    CategoryName = cat?.Name;
                }
                else
                {
                    NewsList = _newsService.GetSortedByDate(descending: true);
                }
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
        }

        public IActionResult OnPostDelete(int id)
        {
            var check = RequireAdminRole();
            if (check != null) return check;

            try { _newsService.DeleteNews(id, CurrentUserId!.Value); }
            catch (Exception ex) { TempData["Error"] = ex.Message; }

            return RedirectToPage();
        }
    }
}