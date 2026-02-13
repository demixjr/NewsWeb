using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NewsWebsite.Filters
{
    public class CategoryNavFilter : IAsyncResultFilter
    {
        private readonly ICategoryService _categoryService;

        public CategoryNavFilter(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Controller is Microsoft.AspNetCore.Mvc.RazorPages.PageModel page)
            {
                try
                {
                    page.ViewData["Categories"] = _categoryService.GetAllCategories();
                }
                catch { /* не ламаємо сторінку */ }
            }
            await next();
        }
    }
}