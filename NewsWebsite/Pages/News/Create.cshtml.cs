using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewsWebsite.Pages;
using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.Pages.News
{
    public class CreateModel : BasePageModel
    {
        private readonly INewsService _newsService;
        private readonly ICategoryService _categoryService;

        public CreateModel(INewsService newsService, ICategoryService categoryService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public SelectList CategorySelectList { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync()
        {
            var check = RequirePublishRole();
            if (check != null) return check;

            await LoadCategoriesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var check = RequirePublishRole();
            if (check != null) return check;

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return Page();
            }

            try
            {
                await _newsService.AddNews(new NewsDTO
                {
                    Title = Input.Title,
                    Description = Input.Description,
                    CategoryId = Input.CategoryId,
                    AuthorId = CurrentUserId!.Value
                });

                TempData["Success"] = "Статтю опубліковано!";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                await LoadCategoriesAsync();
                return Page();
            }
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await _categoryService.GetAllCategories();
            CategorySelectList = new SelectList(categories, "Id", "Name");
        }

        public class InputModel
        {
            [Required(ErrorMessage = "Обов'язкове поле")]
            [StringLength(200)]
            public string Title { get; set; } = string.Empty;

            [Required(ErrorMessage = "Обов'язкове поле")]
            public string Description { get; set; } = string.Empty;

            [Required(ErrorMessage = "Оберіть категорію")]
            public int CategoryId { get; set; }
        }
    }
}