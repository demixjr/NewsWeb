using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NewsWebsite.Pages.Categories;
using Xunit;

namespace UI.Tests.Pages.Categories
{
    public class CategoriesIndexModelTests
    {
        private readonly Mock<ICategoryService> _categoryServiceMock;

        public CategoriesIndexModelTests()
        {
            _categoryServiceMock = new Mock<ICategoryService>();
        }

        private NewsWebsite.Pages.Categories.IndexModel CreatePageModel()
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            var pageContext = new PageContext
            {
                HttpContext = httpContext
            };

            return new NewsWebsite.Pages.Categories.IndexModel(_categoryServiceMock.Object)
            {
                PageContext = pageContext,
                TempData = tempData
            };
        }

        [Fact]
        public async Task OnGetAsync_LoadsAllCategories()
        {
            // Arrange
            var expectedCategories = new List<CategoryDTO>
            {
                new CategoryDTO { Id = 1, Name = "Спорт" },
                new CategoryDTO { Id = 2, Name = "Політика" },
                new CategoryDTO { Id = 3, Name = "Технології" }
            };

            _categoryServiceMock.Setup(s => s.GetAllCategories())
                .ReturnsAsync(expectedCategories);

            var pageModel = CreatePageModel();

            // Act
            await pageModel.OnGetAsync();

            // Assert
            Assert.NotNull(pageModel.Categories);
            Assert.Equal(3, pageModel.Categories.Count);
            Assert.Equal("Спорт", pageModel.Categories[0].Name);
            _categoryServiceMock.Verify(s => s.GetAllCategories(), Times.Once);
        }

        [Fact]
        public async Task OnGetAsync_ServiceThrowsException_ReturnsEmptyList()
        {
            // Arrange
            _categoryServiceMock.Setup(s => s.GetAllCategories())
                .ThrowsAsync(new Exception("Database error"));

            var pageModel = CreatePageModel();

            // Act
            await pageModel.OnGetAsync();

            // Assert
            Assert.NotNull(pageModel.Categories);
            Assert.Empty(pageModel.Categories);
        }
    }
}

namespace UI.Tests.Pages.News
{
    public class PopularModelTests
    {
        private readonly Mock<INewsService> _newsServiceMock;

        public PopularModelTests()
        {
            _newsServiceMock = new Mock<INewsService>();
        }

        private NewsWebsite.Pages.News.PopularModel CreatePageModel()
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            var pageContext = new PageContext
            {
                HttpContext = httpContext
            };

            return new NewsWebsite.Pages.News.PopularModel(_newsServiceMock.Object)
            {
                PageContext = pageContext,
                TempData = tempData
            };
        }

        [Fact]
        public async Task OnGetAsync_LoadsPopularNewsWithDefaultMinViews()
        {
            // Arrange
            var popularNews = new List<NewsDTO>
            {
                new NewsDTO { Id = 1, Title = "Популярна 1", Views = 100 },
                new NewsDTO { Id = 2, Title = "Популярна 2", Views = 50 }
            };

            _newsServiceMock.Setup(s => s.GetPopular(10))
                .ReturnsAsync(popularNews);

            var pageModel = CreatePageModel();

            // Act
            await pageModel.OnGetAsync();

            // Assert
            Assert.NotNull(pageModel.NewsList);
            Assert.Equal(2, pageModel.NewsList.Count);
            Assert.Equal(10, pageModel.MinViews); // default
            _newsServiceMock.Verify(s => s.GetPopular(10), Times.Once);
        }

        [Fact]
        public async Task OnGetAsync_CustomMinViews_LoadsNewsWithCustomValue()
        {
            // Arrange
            var popularNews = new List<NewsDTO>
            {
                new NewsDTO { Id = 1, Title = "Дуже популярна", Views = 500 }
            };

            _newsServiceMock.Setup(s => s.GetPopular(100))
                .ReturnsAsync(popularNews);

            var pageModel = CreatePageModel();
            pageModel.MinViews = 100;

            // Act
            await pageModel.OnGetAsync();

            // Assert
            Assert.Single(pageModel.NewsList);
            Assert.Equal(100, pageModel.MinViews);
            _newsServiceMock.Verify(s => s.GetPopular(100), Times.Once);
        }

        [Fact]
        public async Task OnGetAsync_ServiceThrowsException_SetsTempDataError()
        {
            // Arrange
            _newsServiceMock.Setup(s => s.GetPopular(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Service error"));

            var pageModel = CreatePageModel();

            // Act
            await pageModel.OnGetAsync();

            // Assert
            Assert.Empty(pageModel.NewsList);
            Assert.True(pageModel.TempData.ContainsKey("Error"));
            Assert.Equal("Service error", pageModel.TempData["Error"]);
        }
    }
}
