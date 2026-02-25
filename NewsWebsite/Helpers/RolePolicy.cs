namespace PL.Helpers
{
    /// <summary>
    /// Модуль, що централізує всі правила доступу.
    /// Прибирає дублювання логіки ролей з BasePageModel і сторінок.
    /// </summary>
    public static class RolePolicy
    {
        public const string Admin = "Admin";
        public const string Writer = "Writer";

        public static bool CanPublish(string? role) => role == Writer;
        public static bool CanEdit(string? role) => role == Admin;
        public static bool CanDelete(string? role) => role == Admin;
        public static bool IsAdmin(string? role) => role == Admin;
        public static bool IsWriter(string? role) => role == Writer;
    }
}
