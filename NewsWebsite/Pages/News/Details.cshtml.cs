using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Pages;

namespace NewsWebsite.Pages.News
{
    public class DetailsModel : BasePageModel
    {
        private readonly INewsService _newsService;

        public DetailsModel(INewsService newsService)
        {
            _newsService = newsService;
        }

        public NewsDTO? NewsItem { get; set; }

        // Асинхронний Get
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                NewsItem = await _newsService.GetById(id);
                if (NewsItem == null)
                    return RedirectToPage("Index");

                return Page();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToPage("Index");
            }
        }

        // Асинхронний Post для видалення
        public async Task<IActionResult> OnPostAsync(int id)
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
                return RedirectToPage("Details", new { id });
            }

            TempData["Success"] = "Статтю видалено.";
            return RedirectToPage("Index");
        }
    }
}