using Microsoft.AspNetCore.Mvc;
using PL.Helpers;

namespace NewsWebsite.Pages.Users
{
    public class LogoutModel : BasePageModel
    {
        public IActionResult OnGet()
        {
            SessionUserHelper.ClearCurrentUser(HttpContext.Session);
            return RedirectToPage("/Index");
        }
    }
}