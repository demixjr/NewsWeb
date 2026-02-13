using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.Pages.Users
{
    public class LoginModel : BasePageModel
    {
        private readonly IUserService _userService;
        public LoginModel(IUserService userService) => _userService = userService;

        [BindProperty] public InputModel Input { get; set; } = new();

        public IActionResult OnGet()
        {
            if (IsAuthenticated) return RedirectToPage("/Index");
            return Page();
        }

        // Асинхронний Post
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _userService.FindUserByUsername(Input.Username);

            if (user == null)
            {
                TempData["Error"] = "Користувача не знайдено.";
                return Page();
            }

            CurrentUser = user;
            return RedirectToPage("/Index");
        }

        public class InputModel
        {
            [Required(ErrorMessage = "Введіть ім'я користувача")]
            public string Username { get; set; } = string.Empty;
        }
    }
}