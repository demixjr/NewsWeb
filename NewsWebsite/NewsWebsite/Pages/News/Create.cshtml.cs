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

        public IActionResult OnGet()
        {
            var check = RequirePublishRole();
            if (check != null) return check;
            LoadCategories();
            return Page();
        }

        public IActionResult OnPost()
        {
            var check = RequirePublishRole();
            if (check != null) return check;

            if (!ModelState.IsValid) { LoadCategories(); return Page(); }

            try
            {
                _newsService.AddNews(new NewsDTO
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
                LoadCategories();
                return Page();
            }
        }

        private void LoadCategories() =>
            CategorySelectList = new SelectList(_categoryService.GetAllCategories(), "Id", "Name");

        public class InputModel
        {
            [Required(ErrorMessage = "Обов'язкове поле")]
            [StringLength(200)] public string Title { get; set; } = string.Empty;

            [Required(ErrorMessage = "Обов'язкове поле")]
            public string Description { get; set; } = string.Empty;

            [Required(ErrorMessage = "Оберіть категорію")]
            public int CategoryId { get; set; }
        }
    }
}