using BLL.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewsWebsite.Helpers;
using PL.Helpers;

namespace NewsWebsite.Pages
{
    /// <summary>
    /// Базова сторінка. Делегує роботу з сесією до SessionUserHelper,
    /// перевірку ролей — до RolePolicy.
    /// </summary>
    public class BasePageModel : PageModel
    {
        protected UserDTO? CurrentUser
        {
            get => SessionUserHelper.GetCurrentUser(HttpContext.Session);
            set => SessionUserHelper.SetCurrentUser(HttpContext.Session, value);
        }

        public int? CurrentUserId => CurrentUser?.Id;
        public bool IsAuthenticated => CurrentUser != null;
        public bool IsAdmin => RolePolicy.IsAdmin(CurrentUser?.Role);
        public bool IsWriter => RolePolicy.IsWriter(CurrentUser?.Role);
        public bool CanPublish => RolePolicy.CanPublish(CurrentUser?.Role);
        public bool CanEdit => RolePolicy.CanEdit(CurrentUser?.Role);
        public bool CanDelete => RolePolicy.CanDelete(CurrentUser?.Role);

        protected IActionResult? RedirectToLoginIfNotAuthenticated()
            => IsAuthenticated ? null : RedirectToPage("/Users/Login");

        protected IActionResult? RequirePublishRole()
            => GuardRole(CanPublish, "Потрібна роль Writer або Admin.");

        protected IActionResult? RequireAdminRole()
            => GuardRole(IsAdmin, "Потрібна роль Admin.");

        private IActionResult? GuardRole(bool allowed, string errorMessage)
        {
            if (allowed) return null;
            FlashMessageHelper.SetError(TempData, errorMessage);
            return RedirectToPage("/Index");
        }
    }
}