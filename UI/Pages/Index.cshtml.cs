using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.Interfaces;
using DAL;
using BLL.DTO;
using System.Security.Claims;

namespace NewsPortal.Pages
{
    public class IndexModel : PageModel
    {
        private readonly INewsService _newsService;
        private readonly IRepository<DAL.Models.News> _repository;
        private readonly ICategoryService _categoryService;
        private readonly IRepository<DAL.Models.Category> _categoryRepository;

        public List<NewsDTO> NewsList { get; set; } = new List<NewsDTO>();
        public List<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
        public int TotalNewsCount { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        public IndexModel(
            INewsService newsService,
            IRepository<DAL.Models.News> repository,
            ICategoryService categoryService,
            IRepository<DAL.Models.Category> categoryRepository)
        {
            _newsService = newsService;
            _repository = repository;
            _categoryService = categoryService;
            _categoryRepository = categoryRepository;
        }

        public IActionResult OnGet()
        {
            LoadData();
            return Page();
        }

        // AJAX handler for categories dropdown
        public async Task<JsonResult> OnGetCategoriesAsync()
        {
            var categories = _categoryService.GetAllCategories(_categoryRepository);
            return new JsonResult(categories);
        }

        // AJAX handler for news count
        public JsonResult OnGetNewsCount()
        {
            var count = _repository.GetAll().Count();
            return new JsonResult(new { count });
        }

        // AJAX handler for filtering news
        public async Task<PartialViewResult> OnGetFilterAsync(int? categoryId)
        {
            IEnumerable<NewsDTO> news;

            if (categoryId.HasValue && categoryId > 0)
            {
                news = _newsService.GetByCategory(_repository, categoryId.Value);
            }
            else
            {
                news = _newsService.GetAll(_repository);
            }

            return Partial("_NewsListPartial", news);
        }

        // AJAX handler for sorting news
        public async Task<PartialViewResult> OnGetSortAsync(string sortBy)
        {
            List<NewsDTO> news;

            switch (sortBy?.ToLower())
            {
                case "date_desc":
                    news = _newsService.GetSortedByDate(_repository, true);
                    break;
                case "date_asc":
                    news = _newsService.GetSortedByDate(_repository, false);
                    break;
                case "popular":
                    news = _newsService.GetPopular(_repository, 0);
                    break;
                case "views_desc":
                    news = _newsService.GetAll(_repository)
                        .OrderByDescending(n => n.Views)
                        .ToList();
                    break;
                default:
                    news = _newsService.GetSortedByDate(_repository, true);
                    break;
            }

            return Partial("_NewsListPartial", news);
        }

        // AJAX handler for loading more news (pagination)
        public async Task<JsonResult> OnGetLoadMoreAsync(int page = 1, int pageSize = 6)
        {
            var allNews = _newsService.GetSortedByDate(_repository, true);
            var pagedNews = allNews
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var hasMore = allNews.Count > (page * pageSize);

            // Render partial view to string
            var html = await this.RenderPartialViewToStringAsync("_NewsListPartial", pagedNews);

            return new JsonResult(new
            {
                html,
                hasMore,
                currentPage = page
            });
        }

        private void LoadData()
        {
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                // Search functionality
                var allNews = _newsService.GetAll(_repository);
                NewsList = allNews
                    .Where(n => n.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                               n.Description.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(n => n.Date)
                    .ToList();
            }
            else if (CategoryId.HasValue && CategoryId > 0)
            {
                NewsList = _newsService.GetByCategory(_repository, CategoryId.Value);
            }
            else
            {
                NewsList = _newsService.GetSortedByDate(_repository, true);
            }

            Categories = _categoryService.GetAllCategories(_categoryRepository);
            TotalNewsCount = _repository.GetAll().Count();
        }
    }
}