using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.Pages.Users
{
    public class RegisterModel : BasePageModel
    {
        private readonly IUserService _userService;
        public RegisterModel(IUserService userService) => _userService = userService;

        [BindProperty] public InputModel Input { get; set; } = new();

        public IActionResult OnGet()
        {
            if (!IsAuthenticated)
            {
                return RedirectToPage("/Users/Login");
            }

            if (!IsAdmin)
            {
                TempData["Error"] = "Тільки адміністратор може реєструвати нових авторів.";
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid) return Page();

            try
            {
                await _userService.AddUser(new UserDTO
                {
                    Username = Input.Username,
                    Role = Input.Role,
                    Password = Input.Password 
                });

                TempData["Success"] = $"Користувача {Input.Username} успішно зареєстровано!";
                return RedirectToPage("Login");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return Page();
            }
        }

        public class InputModel
        {
            [Required(ErrorMessage = "Обов'язкове поле")]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Від 3 до 50 символів")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Оберіть роль")]
            public string Role { get; set; } = string.Empty;

            [Required(ErrorMessage = "Введіть пароль")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль має бути не менше 6 символів")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Паролі не збігаються")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }
    }
}