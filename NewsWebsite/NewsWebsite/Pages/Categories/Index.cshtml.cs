using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.Pages.Categories
{
    public class IndexModel : BasePageModel
    {
        private readonly ICategoryService _categoryService;

        public IndexModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public List<CategoryDTO> Categories { get; set; } = new();
        [BindProperty] public InputModel Input { get; set; } = new();

        public void OnGet() => Load();

        public IActionResult OnPost()
        {
            var check = RequireAdminRole();
            if (check != null) return check;

            if (!ModelState.IsValid) { Load(); return Page(); }

            try
            {
                _categoryService.AddCategory(new CategoryDTO { Name = Input.Name });
                TempData["Success"] = "Категорію створено.";
                return RedirectToPage();
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; Load(); return Page(); }
        }

        private void Load()
        {
            try { Categories = _categoryService.GetAllCategories(); }
            catch { Categories = new(); }
        }

        public class InputModel
        {
            [Required(ErrorMessage = "Назва обов'язкова")]
            [StringLength(100)]
            public string Name { get; set; } = string.Empty;
        }
    }
}