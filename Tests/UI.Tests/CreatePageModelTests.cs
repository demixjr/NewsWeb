using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NewsWebsite.Pages.News;
using UI.Tests.Helpers;
using Xunit;

namespace UI.Tests.Pages.News
{
    public class CreateModelTests
    {
        private readonly Mock<INewsService> _newsServiceMock;
        private readonly Mock<ICategoryService> _categoryServiceMock;

        public CreateModelTests()
        {
            _newsServiceMock = new Mock<INewsService>();
            _categoryServiceMock = new Mock<ICategoryService>();
        }

        private CreateModel CreatePageModel(UserDTO? currentUser = null)
        {
            var model = new CreateModel(_newsServiceMock.Object, _categoryServiceMock.Object);
            
            // Налаштовуємо PageContext з Session
            PageModelTestHelper.SetupPageModel(model, currentUser);

            return model;
        }

        #region OnGetAsync Tests

        [Fact]
        public async Task OnGetAsync_NoPublishRole_ReturnsRedirect()
        {
            // Arrange - користувач без Writer/Admin ролі
            var regularUser = PageModelTestHelper.CreateUser();
            var pageModel = CreatePageModel(regularUser);

            // Act
            var result = await pageModel.OnGetAsync();

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Index", redirectResult.PageName);
            Assert.True(pageModel.TempData.ContainsKey("Error"));
        }

        [Fact]
        public async Task OnGetAsync_WithPublishRole_LoadsCategoriesAndReturnsPage()
        {
            // Arrange
            var categories = new List<CategoryDTO>
            {
                new CategoryDTO { Id = 1, Name = "Спорт" },
                new CategoryDTO { Id = 2, Name = "Політика" }
            };

            _categoryServiceMock.Setup(s => s.GetAllCategories())
                .ReturnsAsync(categories);

            var writer = PageModelTestHelper.CreateWriter();
            var pageModel = CreatePageModel(writer);

            // Act
            var result = await pageModel.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            _categoryServiceMock.Verify(s => s.GetAllCategories(), Times.Once);
            Assert.NotNull(pageModel.CategorySelectList);
        }

        [Fact]
        public async Task OnGetAsync_LoadsCategories_PopulatesSelectList()
        {
            // Arrange
            var categories = new List<CategoryDTO>
            {
                new CategoryDTO { Id = 1, Name = "Спорт" },
                new CategoryDTO { Id = 2, Name = "Технології" }
            };

            _categoryServiceMock.Setup(s => s.GetAllCategories())
                .ReturnsAsync(categories);

            var writer = PageModelTestHelper.CreateWriter();
            var pageModel = CreatePageModel(writer);

            // Act
            await pageModel.OnGetAsync();

            // Assert
            Assert.NotNull(pageModel.CategorySelectList);
            _categoryServiceMock.Verify(s => s.GetAllCategories(), Times.Once);
        }

        #endregion

        #region OnPostAsync Tests

        [Fact]
        public async Task OnPostAsync_InvalidModelState_ReturnsPageWithCategories()
        {
            // Arrange
            var categories = new List<CategoryDTO>
            {
                new CategoryDTO { Id = 1, Name = "Спорт" }
            };

            _categoryServiceMock.Setup(s => s.GetAllCategories())
                .ReturnsAsync(categories);

            var writer = PageModelTestHelper.CreateWriter();
            var pageModel = CreatePageModel(writer);
            pageModel.ModelState.AddModelError("Title", "Required");

            // Act
            var result = await pageModel.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            _categoryServiceMock.Verify(s => s.GetAllCategories(), Times.Once);
            _newsServiceMock.Verify(s => s.AddNews(It.IsAny<NewsDTO>()), Times.Never);
        }

        [Fact]
        public async Task OnPostAsync_ValidInput_CreatesNewsAndRedirects()
        {
            // Arrange
            var writer = PageModelTestHelper.CreateWriter(id: 42);
            var pageModel = CreatePageModel(writer);

            pageModel.Input = new CreateModel.InputModel
            {
                Title = "Тестова новина",
                Description = "Опис новини",
                CategoryId = 1
            };

            _newsServiceMock.Setup(s => s.AddNews(It.IsAny<NewsDTO>()))
                .Returns(Task.FromResult(true));

            // Act
            var result = await pageModel.OnPostAsync();

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Index", redirectResult.PageName);
            Assert.True(pageModel.TempData.ContainsKey("Success"));
            
            _newsServiceMock.Verify(s => s.AddNews(It.Is<NewsDTO>(n =>
                n.Title == "Тестова новина" &&
                n.Description == "Опис новини" &&
                n.CategoryId == 1 &&
                n.AuthorId == 42 // Перевіряємо AuthorId
            )), Times.Once);
        }

        [Fact]
        public async Task OnPostAsync_ServiceThrowsException_SetsTempDataError()
        {
            // Arrange
            var categories = new List<CategoryDTO>
            {
                new CategoryDTO { Id = 1, Name = "Спорт" }
            };

            _categoryServiceMock.Setup(s => s.GetAllCategories())
                .ReturnsAsync(categories);

            var writer = PageModelTestHelper.CreateWriter();
            var pageModel = CreatePageModel(writer);

            pageModel.Input = new CreateModel.InputModel
            {
                Title = "Тестова новина",
                Description = "Опис",
                CategoryId = 1
            };

            _newsServiceMock.Setup(s => s.AddNews(It.IsAny<NewsDTO>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await pageModel.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.True(pageModel.TempData.ContainsKey("Error"));
            Assert.Equal("Database error", pageModel.TempData["Error"]);
            _categoryServiceMock.Verify(s => s.GetAllCategories(), Times.Once);
        }

        [Fact]
        public async Task OnPostAsync_SetsCorrectAuthorId()
        {
            // Arrange
            var writer = PageModelTestHelper.CreateWriter(id: 99);
            var pageModel = CreatePageModel(writer);

            pageModel.Input = new CreateModel.InputModel
            {
                Title = "Новина",
                Description = "Опис",
                CategoryId = 1
            };

            NewsDTO? capturedNews = null;
            _newsServiceMock.Setup(s => s.AddNews(It.IsAny<NewsDTO>()))
                .Callback<NewsDTO>(n => capturedNews = n)
                .Returns(Task.FromResult(true));

            // Act
            await pageModel.OnPostAsync();

            // Assert
            Assert.NotNull(capturedNews);
            Assert.Equal(99, capturedNews.AuthorId);
            Assert.Equal("Новина", capturedNews.Title);
        }

        #endregion
    }
}
