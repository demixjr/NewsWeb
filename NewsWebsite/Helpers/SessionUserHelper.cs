using BLL.DTO;
using NewsWebsite.Helpers;

namespace PL.Helpers
{
    /// <summary>
    /// Модуль для роботи з даними користувача в сесії.
    /// Зменшує зв'язність BasePageModel із HttpContext.Session.
    /// </summary>
    public static class SessionUserHelper
    {
        private const string SessionKey = "CurrentUser";

        public static UserDTO? GetCurrentUser(ISession session)
            => session.GetObject<UserDTO>(SessionKey);

        public static void SetCurrentUser(ISession session, UserDTO? user)
            => session.SetObject(SessionKey, user);

        public static void ClearCurrentUser(ISession session)
            => session.Remove(SessionKey);

        public static bool IsAuthenticated(ISession session)
            => GetCurrentUser(session) != null;

        public static bool HasRole(ISession session, string role)
            => GetCurrentUser(session)?.Role == role;
    }
}
