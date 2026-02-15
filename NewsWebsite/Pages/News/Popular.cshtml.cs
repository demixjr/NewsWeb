// Popular.cshtml.cs
using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace NewsWebsite.Pages.News
{
    public class PopularModel : BasePageModel
    {
        private readonly INewsService _newsService;

        public PopularModel(INewsService newsService)
        {
            _newsService = newsService;
        }

        public List<NewsDTO> NewsList { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int MinViews { get; set; } = 10;

        public async Task OnGetAsync()
        {
            try
            {
                NewsList = await _newsService.GetPopular(MinViews);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var check = RequireAdminRole();
            if (check != null)
                return check;

            try
            {
                await _newsService.DeleteNews(id);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage();
        }
    }
}