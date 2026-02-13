using Microsoft.AspNetCore.Mvc;

namespace NewsWebsite.Pages.Users
{
    public class LogoutModel : BasePageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Index");
        }
    }
}