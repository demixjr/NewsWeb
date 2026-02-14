using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewsWebsite.Pages;
using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.Pages.News
{
    public class EditModel : BasePageModel
    {
        private readonly INewsService _newsService;
        private readonly ICategoryService _categoryService;

        public EditModel(INewsService newsService, ICategoryService categoryService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
        }

        [BindProperty] public InputModel Input { get; set; } = new();
        public SelectList CategorySelectList { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var check = RequireAdminRole();
            if (check != null) return check;

            var news = await _newsService.GetById(id);
            if (news == null) return RedirectToPage("Index");

            Input = new InputModel
            {
                Id = news.Id,
                Title = news.Title ?? string.Empty,
                Description = news.Description ?? string.Empty,
                CategoryId = news.CategoryId
            };

            await LoadCategoriesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var check = RequireAdminRole();
            if (check != null) return check;

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return Page();
            }

            try
            {
                await _newsService.EditNews(new NewsDTO
                {
                    Id = Input.Id,
                    Title = Input.Title,
                    Description = Input.Description,
                    CategoryId = Input.CategoryId
                });

                TempData["Success"] = "«м≥ни збережено.";
                return RedirectToPage("Details", new { id = Input.Id });
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
            CategorySelectList = new SelectList(categories, "Id", "Name", Input.CategoryId);
        }

        public class InputModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "ќбов'€зкове поле")]
            [StringLength(200)]
            public string Title { get; set; } = string.Empty;

            [Required(ErrorMessage = "ќбов'€зкове поле")]
            public string Description { get; set; } = string.Empty;

            [Required(ErrorMessage = "ќбер≥ть категор≥ю")]
            public int CategoryId { get; set; }
        }
    }
}