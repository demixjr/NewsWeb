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
            if (IsAuthenticated) return RedirectToPage("/Index");
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();

            try
            {
                _userService.AddUser(new UserDTO { Username = Input.Username, Role = Input.Role });
                TempData["Success"] = "Реєстрація успішна! Тепер увійдіть.";
                return RedirectToPage("Login");
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; return Page(); }
        }

        public class InputModel
        {
            [Required(ErrorMessage = "Обов'язкове поле")]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Від 3 до 50 символів")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Оберіть роль")]
            public string Role { get; set; } = string.Empty;
        }
    }
}