namespace UI.Services
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        bool IsAdmin { get; }
    }
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContext;

        public CurrentUserService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public int? UserId =>
            int.TryParse(_httpContext.HttpContext?.User.FindFirst("Id")?.Value, out var id)
                ? id
                : null;

        public bool IsAdmin =>
            _httpContext.HttpContext?.User.IsInRole("Admin") ?? false;
    }
}
