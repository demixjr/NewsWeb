using Microsoft.AspNetCore.Authorization;
using BLL.DTO;

public class CanEditNewsRequirement : IAuthorizationRequirement { }

public class NewsAuthorizationHandler
    : AuthorizationHandler<CanEditNewsRequirement, NewsDTO>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanEditNewsRequirement requirement,
        NewsDTO news)
    {
        if (context.User.IsInRole("Admin"))
            context.Succeed(requirement);

        var userId = context.User.FindFirst("Id")?.Value;
        if (userId != null && news.AuthorId.ToString() == userId)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}