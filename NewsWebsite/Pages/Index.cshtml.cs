using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using PL.Helpers;

namespace NewsWebsite.Pages
{
    public class IndexModel : BasePageModel
    {
        private readonly INewsService _newsService;

        public IndexModel(INewsService newsService) => _newsService = newsService;

        public List<NewsDTO> NewsList { get; set; } = new();

        public async Task OnGetAsync()
        {
            try { NewsList = await _newsService.GetSortedByDate(descending: true); }
            catch (Exception ex) { FlashMessageHelper.SetError(TempData, ex); }
        }

        public IActionResult OnPostDelete(int id)
        {
            var check = RequireAdminRole();
            if (check != null) return check;

            try { _newsService.DeleteNews(id); }
            catch (Exception ex) { FlashMessageHelper.SetError(TempData, ex); }

            return RedirectToPage();
        }
    }
}