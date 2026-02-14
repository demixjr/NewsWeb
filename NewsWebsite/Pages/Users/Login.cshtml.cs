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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _userService.Authenticate(Input.Username, Input.Password);

            if (user == null)
            {
                TempData["Error"] = "Невірне ім'я користувача або пароль.";
                return Page();
            }

            CurrentUser = user;
            return RedirectToPage("/Index");
        }

        public class InputModel
        {
            [Required(ErrorMessage = "Введіть ім'я користувача")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Введіть пароль")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }
    }
}