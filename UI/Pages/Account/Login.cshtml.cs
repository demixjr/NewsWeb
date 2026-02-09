using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.Interfaces;
using DAL;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace NewsPortal.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IRepository<DAL.Models.User> _userRepository;

        public LoginModel(
            IUserService userService,
            IRepository<DAL.Models.User> userRepository)
        {
            _userService = userService;
            _userRepository = userRepository;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Username is required")]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Display(Name = "Remember me")]
            public bool RememberMe { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                // Find user by username
                var userDTO = _userService.FindUserByUsername(_userRepository, Input.Username);

                if (userDTO != null)
                {
                    // Note: In a real application, you should verify the password hash
                    // For this demo, we'll accept any non-empty password
                    if (!string.IsNullOrEmpty(Input.Password))
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, userDTO.Username),
                            new Claim(ClaimTypes.Role, userDTO.Role ?? "User"),
                            new Claim("UserId", userDTO.Id.ToString()),
                            new Claim("FullName", userDTO.Username)
                        };

                        var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = Input.RememberMe,
                            ExpiresUtc = Input.RememberMe ?
                                DateTimeOffset.UtcNow.AddDays(30) :
                                DateTimeOffset.UtcNow.AddHours(2)
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        // Log login activity
                        await LogLoginAsync(userDTO.Id);

                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid password.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
            }

            return Page();
        }

        private async Task LogLoginAsync(int userId)
        {
            // In a real app, you would log this to a database
            Console.WriteLine($"User {userId} logged in at {DateTime.Now}");
        }

        // AJAX handler for checking username availability (for registration)
        public JsonResult OnGetCheckUsername(string username)
        {
            var user = _userService.FindUserByUsername(_userRepository, username);
            return new JsonResult(new { available = user == null });
        }
    }
}