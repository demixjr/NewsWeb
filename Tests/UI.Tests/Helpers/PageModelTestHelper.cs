using BLL.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using UI.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Routing;

namespace UI.Tests.Helpers
{
    /// <summary>
    /// Хелпер для створення PageModel з налаштованою Session
    /// </summary>
    public static class PageModelTestHelper
    {
        /// <summary>
        /// Створює PageContext з TestSession та TempData
        /// </summary>
        public static (PageContext pageContext, TempDataDictionary tempData, TestSession session) CreatePageContext(UserDTO? currentUser = null)
        {
            var httpContext = new DefaultHttpContext();
            var session = new TestSession();
            httpContext.Session = session;

            // Якщо передано користувача - додаємо в Session
            if (currentUser != null)
            {
                session.SetObject("CurrentUser", currentUser);
            }

            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            var pageContext = new PageContext
            {
                HttpContext = httpContext
            };

            return (pageContext, tempData, session);
        }

        /// <summary>
        /// Налаштовує PageModel з Session та TempData
        /// </summary>

public static void SetupPageModel(PageModel pageModel, UserDTO? currentUser = null)
    {
        var (pageContext, tempData, _) = CreatePageContext(currentUser);

            pageContext.ActionDescriptor = new CompiledPageActionDescriptor();

            pageModel.PageContext = pageContext;
        pageModel.TempData = tempData;
        pageModel.MetadataProvider = new EmptyModelMetadataProvider();
        pageModel.ModelState.Clear();
    }

    /// <summary>
    /// Створює Writer користувача для тестів
    /// </summary>
    public static UserDTO CreateWriter(int id = 1, string username = "writer")
        {
            return new UserDTO
            {
                Id = id,
                Username = username,
                Role = "Writer"
            };
        }

        /// <summary>
        /// Створює Admin користувача для тестів
        /// </summary>
        public static UserDTO CreateAdmin(int id = 1, string username = "admin")
        {
            return new UserDTO
            {
                Id = id,
                Username = username,
                Role = "Admin"
            };
        }

        /// <summary>
        /// Створює звичайного User для тестів
        /// </summary>
        public static UserDTO CreateUser(int id = 1, string username = "user")
        {
            return new UserDTO
            {
                Id = id,
                Username = username,
                Role = "User"
            };
        }
    }
}
