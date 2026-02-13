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

        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                if (CategoryId.HasValue)
                {
                    // Асинхронно отримуємо новини по категорії
                    NewsList = await _newsService.GetByCategory(CategoryId.Value);

                    // Асинхронно отримуємо всі категорії і знаходимо потрібну
                    var categories = await _categoryService.GetAllCategories();
                    var cat = categories.FirstOrDefault(c => c.Id == CategoryId.Value);
                    CategoryName = cat?.Name;
                }
                else
                {
                    NewsList = await _newsService.GetSortedByDate(descending: true);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                NewsList = new();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var check = RequireAdminRole();
            if (check != null) return check;

            try
            {
                await _newsService.DeleteNews(id, CurrentUserId!.Value);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage();
        }
    }
}