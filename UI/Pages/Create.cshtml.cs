// Pages/News/Create.cshtml.cs
using BLL.DTO;
using BLL.Interfaces;
using BLL.Services;
using DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace NewsPortal.Pages.News
{
    [Authorize(Policy = "AuthorOrAdmin")]
    public class CreateModel : PageModel
    {
        private readonly INewsService _newsService;
        private readonly IRepository<DAL.Models.News> _newsRepository;
        private readonly IRepository<DAL.Models.Category> _categoryRepository;
        private readonly IRepository<DAL.Models.User> _userRepository;

        [BindProperty]
        public NewsDTO News { get; set; }

        public List<CategoryDTO> Categories { get; set; }

        public CreateModel(
            INewsService newsService,
            IRepository<DAL.Models.News> newsRepository,
            IRepository<DAL.Models.Category> categoryRepository,
            IRepository<DAL.Models.User> userRepository)
        {
            _newsService = newsService;
            _newsRepository = newsRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
        }

        public void OnGet()
        {
            var categoryService = new CategoryService(null); // Need to fix DI
            Categories = categoryService.GetAllCategories(_categoryRepository);
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                OnGet();
                return Page();
            }

            var userId = int.Parse(User.FindFirstValue("UserId"));
            News.AuthorId = userId;

            _newsService.AddNews(_newsRepository, News);
            (_newsRepository as DAL.Repository<DAL.Models.News>)?.SaveChanges();

            TempData["SuccessMessage"] = "News created successfully!";
            return RedirectToPage("/Index");
        }
    }
}