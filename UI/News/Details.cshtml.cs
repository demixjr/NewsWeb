using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.Interfaces;
using DAL;
using BLL.DTO;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Pages.News
{
    public class DetailsModel : PageModel
    {
        private readonly INewsService _newsService;
        private readonly IRepository<DAL.Models.News> _newsRepository;
        private readonly IRepository<DAL.Models.User> _userRepository;
        private readonly IRepository<DAL.Models.Category> _categoryRepository;

        public NewsDTO News { get; set; }
        public List<NewsDTO> RelatedNews { get; set; } = new List<NewsDTO>();
        public bool IsAuthor { get; set; }
        public bool IsAdmin { get; set; }
        public bool CanEdit { get; set; }
        public List<CommentDTO> Comments { get; set; } = new List<CommentDTO>();

        [BindProperty]
        public CommentInput CommentInput { get; set; }

        public DetailsModel(
            INewsService newsService,
            IRepository<DAL.Models.News> newsRepository,
            IRepository<DAL.Models.User> userRepository,
            IRepository<DAL.Models.Category> categoryRepository)
        {
            _newsService = newsService;
            _newsRepository = newsRepository;
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
        }

        public IActionResult OnGet(int id)
        {
            News = _newsService.GetById(_newsRepository, id);

            if (News == null)
            {
                return NotFound();
            }

            LoadRelatedNews();
            CheckPermissions();
            LoadComments();

            return Page();
        }

        // AJAX handler for incrementing views
        public JsonResult OnPostIncrementViews(int id)
        {
            var news = _newsService.GetById(_newsRepository, id);
            if (news != null)
            {
                // Note: In a real app, you would increment in database
                return new JsonResult(new { success = true, views = news.Views });
            }
            return new JsonResult(new { success = false });
        }

        // AJAX handler for toggling favorite
        [Authorize]
        public JsonResult OnPostToggleFavorite(int id)
        {
            var userId = int.Parse(User.FindFirstValue("UserId"));

            // Note: In a real app, you would have a favorites table
            return new JsonResult(new
            {
                success = true,
                isFavorite = false, // Mock data
                favoriteCount = 12 // Mock data
            });
        }

        // AJAX handler for adding comment
        [Authorize]
        public async Task<JsonResult> OnPostAddCommentAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, errors = ModelState.Values });
            }

            var userId = int.Parse(User.FindFirstValue("UserId"));
            var username = User.Identity.Name;

            // Note: In a real app, you would save to database
            var comment = new CommentDTO
            {
                Id = new Random().Next(1000, 9999),
                Content = CommentInput.Content,
                CreatedAt = DateTime.Now,
                UserId = userId,
                UserName = username,
                NewsId = id
            };

            return new JsonResult(new
            {
                success = true,
                comment = comment,
                userName = username,
                content = comment.Content
            });
        }

        // AJAX handler for deleting comment
        [Authorize]
        public JsonResult OnPostDeleteComment(int commentId)
        {
            // Note: In a real app, you would delete from database
            return new JsonResult(new { success = true });
        }

        // AJAX handler for loading more comments
        public JsonResult OnGetLoadComments(int id, int page = 1)
        {
            // Note: In a real app, you would load from database with pagination
            var mockComments = new List<CommentDTO>();

            return new JsonResult(new
            {
                comments = mockComments,
                hasMore = false,
                page = page
            });
        }

        private void LoadRelatedNews()
        {
            var allNews = _newsService.GetByCategory(_newsRepository, News.CategoryId)
                .Where(n => n.Id != News.Id)
                .OrderByDescending(n => n.Date)
                .Take(3)
                .ToList();

            RelatedNews = allNews;
        }

        private void CheckPermissions()
        {
            if (User.Identity.IsAuthenticated)
            {
                var currentUserId = int.Parse(User.FindFirstValue("UserId"));
                var userRole = User.FindFirstValue(ClaimTypes.Role);

                IsAuthor = News.AuthorId == currentUserId;
                IsAdmin = userRole == "Admin";
                CanEdit = IsAuthor || IsAdmin;
            }
        }

        private void LoadComments()
        {
            // Note: In a real app, load from database
            // Mock comments for demonstration
            Comments = new List<CommentDTO>
            {
                new CommentDTO
                {
                    Id = 1,
                    Content = "Great article! Very informative.",
                    CreatedAt = DateTime.Now.AddHours(-2),
                    UserName = "user1",
                    UserId = 3
                },
                new CommentDTO
                {
                    Id = 2,
                    Content = "Thanks for sharing this news.",
                    CreatedAt = DateTime.Now.AddHours(-1),
                    UserName = "author1",
                    UserId = 2
                }
            };
        }
    }

    public class CommentInput
    {
        [Required(ErrorMessage = "Comment is required")]
        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters")]
        public string Content { get; set; }
    }

    public class CommentDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int NewsId { get; set; }
    }
}