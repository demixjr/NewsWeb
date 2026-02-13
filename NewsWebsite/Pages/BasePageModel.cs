using BLL.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewsWebsite.Helpers;

namespace NewsWebsite.Pages
{
    public class BasePageModel : PageModel
    {
        protected UserDTO? CurrentUser
        {
            get => HttpContext.Session.GetObject<UserDTO>("CurrentUser");
            set => HttpContext.Session.SetObject("CurrentUser", value);
        }

        public int? CurrentUserId => CurrentUser?.Id;
        public bool IsAuthenticated => CurrentUser != null;
        public bool IsAdmin => CurrentUser?.Role == "Admin";
        public bool IsWriter => CurrentUser?.Role == "Writer";
        public bool CanPublish => IsAdmin || IsWriter;
        public bool CanEdit => IsAdmin;
        public bool CanDelete => IsAdmin;

        protected IActionResult RedirectToLoginIfNotAuthenticated()
        {
            if (!IsAuthenticated)
                return RedirectToPage("/Users/Login");
            return null!;
        }

        protected IActionResult RequirePublishRole()
        {
            if (!CanPublish)
            {
                TempData["Error"] = "Потрібна роль Writer або Admin.";
                return RedirectToPage("/Index");
            }
            return null!;
        }

        protected IActionResult RequireAdminRole()
        {
            if (!IsAdmin)
            {
                TempData["Error"] = "Потрібна роль Admin.";
                return RedirectToPage("/Index");
            }
            return null!;
        }
    }
}