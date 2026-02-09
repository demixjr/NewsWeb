using BLL.DTO;
using BLL.Interfaces;
using DAL;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UI.Services;

[Authorize]
public class EditModel : PageModel
{
    private readonly INewsService _newsService;
    private readonly IRepository<News> _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuthorizationService _authorization;

    [BindProperty]
    public NewsDTO News { get; set; }

    public EditModel(INewsService newsService, IRepository<News> repository, ICurrentUserService currentUser, IAuthorizationService authorization)
    {
        _newsService = newsService;
        _repository = repository;
        _currentUser = currentUser;
        _authorization = authorization;
    }

    public IActionResult OnPost()
    {
        var authResult = _authorization.AuthorizeAsync(User, News, "CanEditNews").Result;
        if (!authResult.Succeeded) return Forbid();

        _newsService.EditNews(_repository, News, _currentUser.UserId.Value);
        return RedirectToPage("Index");
    }
}