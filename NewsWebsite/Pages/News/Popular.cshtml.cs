// Popular.cshtml.cs
using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using PL.Helpers;
namespace NewsWebsite.Pages.News
{
    public class PopularModel : BasePageModel
    {
        private readonly INewsService _newsService;

        public PopularModel(INewsService newsService) => _newsService = newsService;

        public List<NewsDTO> NewsList { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int MinViews { get; set; } = 10;

        public async Task OnGetAsync()
        {
            try   { NewsList = await _newsService.GetPopular(MinViews); }
            catch (Exception ex) { FlashMessageHelper.SetError(TempData, ex); }
        }

        public Task<IActionResult> OnPostDeleteAsync(int id) => HandleDeleteNewsAsync(_newsService, id);
    }
}