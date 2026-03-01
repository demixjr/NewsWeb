using BLL.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NewsWebsite.Pages;
using System.ComponentModel.DataAnnotations;
using UI.Tests.Helpers;
using Xunit;

namespace UI.Tests.Pages
{
    public class BasePageModelTests
    {
        private TestableBasePageModel CreatePageModel()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new TestSession(); // Використовуємо TestSession
            
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            var pageContext = new PageContext
            {
                HttpContext = httpContext
            };

            var model = new TestableBasePageModel
            {
                PageContext = pageContext,
                TempData = tempData
            };

            return model;
        }

        #region CurrentUser Tests

        [Fact]
        public void IsAuthenticated_NoUser_ReturnsFalse()
        {
            // Arrange
            var model = CreatePageModel();

            // Act & Assert
            Assert.False(model.IsAuthenticated);
        }

        [Fact]
        public void IsAuthenticated_WithUser_ReturnsTrue()
        {
            // Arrange
            var model = CreatePageModel();
            model.SetCurrentUser(new UserDTO { Id = 1, Username = "test", Role = "User" });

            // Act & Assert
            Assert.True(model.IsAuthenticated);
        }

        [Fact]
        public void CurrentUserId_NoUser_ReturnsNull()
        {
            // Arrange
            var model = CreatePageModel();

            // Act & Assert
            Assert.Null(model.CurrentUserId);
        }

        [Fact]
        public void CurrentUserId_WithUser_ReturnsUserId()
        {
            // Arrange
            var model = CreatePageModel();
            model.SetCurrentUser(new UserDTO { Id = 42, Username = "test" });

            // Act & Assert
            Assert.Equal(42, model.CurrentUserId);
        }

        #endregion

        #region Role Tests

        [Fact]
        public void IsAdmin_AdminUser_ReturnsTrue()
        {
            // Arrange
            var model = CreatePageModel();
            model.SetCurrentUser(new UserDTO { Id = 1, Username = "admin", Role = "Admin" });

            // Act & Assert
            Assert.True(model.IsAdmin);
        }

        [Fact]
        public void IsAdmin_RegularUser_ReturnsFalse()
        {
            // Arrange
            var model = CreatePageModel();
            model.SetCurrentUser(new UserDTO { Id = 1, Username = "user", Role = "User" });

            // Act & Assert
            Assert.False(model.IsAdmin);
        }

        [Fact]
        public void IsWriter_WriterUser_ReturnsTrue()
        {
            // Arrange
            var model = CreatePageModel();
            model.SetCurrentUser(new UserDTO { Id = 1, Username = "writer", Role = "Writer" });

            // Act & Assert
            Assert.True(model.IsWriter);
        }

        [Fact]
        public void CanPublish_WriterUser_ReturnsTrue()
        {
            // Arrange
            var model = CreatePageModel();
            model.SetCurrentUser(new UserDTO { Id = 1, Username = "writer", Role = "Writer" });

            // Act & Assert
            Assert.True(model.CanPublish);
        }

        [Fact]
        public void CanEdit_AdminUser_ReturnsTrue()
        {
            // Arrange
            var model = CreatePageModel();
            model.SetCurrentUser(new UserDTO { Id = 1, Username = "admin", Role = "Admin" });

            // Act & Assert
            Assert.True(model.CanEdit);
        }

        [Fact]
        public void CanDelete_AdminUser_ReturnsTrue()
        {
            // Arrange
            var model = CreatePageModel();
            model.SetCurrentUser(new UserDTO { Id = 1, Username = "admin", Role = "Admin" });

            // Act & Assert
            Assert.True(model.CanDelete);
        }

        #endregion

        #region Authorization Methods Tests

        [Fact]
        public void RedirectToLoginIfNotAuthenticated_NotAuthenticated_ReturnsRedirect()
        {
            // Arrange
            var model = CreatePageModel();

            // Act
            var result = model.TestRedirectToLoginIfNotAuthenticated();

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Users/Login", redirectResult.PageName);
        }

        [Fact]
        public void RedirectToLoginIfNotAuthenticated_Authenticated_ReturnsNull()
        {
            // Arrange
            var model = CreatePageModel();
            model.SetCurrentUser(new UserDTO { Id = 1, Username = "user", Role = "User" });

            // Act
            var result = model.TestRedirectToLoginIfNotAuthenticated();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void RequirePublishRole_NoPublishRole_ReturnsRedirectWithError()
        {
            // Arrange
            var model = CreatePageModel();
            model.SetCurrentUser(new UserDTO { Id = 1, Username = "user", Role = "User" });

            // Act
            var result = model.TestRequirePublishRole();

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Index", redirectResult.PageName);
            Assert.True(model.TempData.ContainsKey("Error"));
            Assert.Equal("Потрібна роль Writer або Admin.", model.TempData["Error"]);
        }

        [Fact]
        public void RequirePublishRole_WriterRole_ReturnsNull()
        {
            // Arrange
            var model = CreatePageModel();
            model.SetCurrentUser(new UserDTO { Id = 1, Username = "writer", Role = "Writer" });

            // Act
            var result = model.TestRequirePublishRole();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void RequireAdminRole_NotAdmin_ReturnsRedirectWithError()
        {
            // Arrange
            var model = CreatePageModel();
            model.SetCurrentUser(new UserDTO { Id = 1, Username = "writer", Role = "Writer" });

            // Act
            var result = model.TestRequireAdminRole();

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Index", redirectResult.PageName);
            Assert.True(model.TempData.ContainsKey("Error"));
            Assert.Equal("Потрібна роль Admin.", model.TempData["Error"]);
        }

        [Fact]
        public void RequireAdminRole_AdminRole_ReturnsNull()
        {
            // Arrange
            var model = CreatePageModel();
            model.SetCurrentUser(new UserDTO { Id = 1, Username = "admin", Role = "Admin" });

            // Act
            var result = model.TestRequireAdminRole();

            // Assert
            Assert.Null(result);
        }

        #endregion

        // Testable wrapper для доступу до protected методів
        private class TestableBasePageModel : BasePageModel
        {
            public void SetCurrentUser(UserDTO? user)
            {
                // Використовуємо Session через HttpContext
                if (user != null)
                {
                    HttpContext.Session.SetObject("CurrentUser", user);
                }
                else
                {
                    HttpContext.Session.Remove("CurrentUser");
                }
            }

            public IActionResult TestRedirectToLoginIfNotAuthenticated()
            {
                return RedirectToLoginIfNotAuthenticated();
            }

            public IActionResult TestRequirePublishRole()
            {
                return RequirePublishRole();
            }

            public IActionResult TestRequireAdminRole()
            {
                return RequireAdminRole();
            }
        }
    }
}
