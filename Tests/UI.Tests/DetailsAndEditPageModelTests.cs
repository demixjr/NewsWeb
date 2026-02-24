using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NewsWebsite.Pages.News;
using Xunit;

namespace UI.Tests.Pages.News
{
    public class DetailsModelTests
    {
        private readonly Mock<INewsService> _newsServiceMock;

        public DetailsModelTests()
        {
            _newsServiceMock = new Mock<INewsService>();
        }

        private DetailsModel CreatePageModel()
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            var pageContext = new PageContext
            {
                HttpContext = httpContext
            };

            return new DetailsModel(_newsServiceMock.Object)
            {
                PageContext = pageContext,
                TempData = tempData
            };
        }

        [Fact]
        public async Task OnGetAsync_ExistingNews_LoadsNewsAndReturnsPage()
        {
            // Arrange
            var expectedNews = new NewsDTO
            {
                Id = 1,
                Title = "Тестова новина",
                Description = "Опис",
                Views = 10
            };

            _newsServiceMock.Setup(s => s.GetById(1))
                .ReturnsAsync(expectedNews);

            var pageModel = CreatePageModel();

            // Act
            var result = await pageModel.OnGetAsync(1);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.NotNull(pageModel.NewsItem);
            Assert.Equal("Тестова новина", pageModel.NewsItem.Title);
            Assert.Equal(10, pageModel.NewsItem.Views);
            _newsServiceMock.Verify(s => s.GetById(1), Times.Once);
        }

        [Fact]
        public async Task OnGetAsync_NonExistentNews_RedirectsToIndex()
        {
            // Arrange
            _newsServiceMock.Setup(s => s.GetById(999))
                .ReturnsAsync((NewsDTO?)null);

            var pageModel = CreatePageModel();

            // Act
            var result = await pageModel.OnGetAsync(999);

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Index", redirectResult.PageName);
            Assert.Null(pageModel.NewsItem);
        }

        [Fact]
        public async Task OnGetAsync_ServiceThrowsException_RedirectsWithError()
        {
            // Arrange
            _newsServiceMock.Setup(s => s.GetById(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Database error"));

            var pageModel = CreatePageModel();

            // Act
            var result = await pageModel.OnGetAsync(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Index", redirectResult.PageName);
            Assert.True(pageModel.TempData.ContainsKey("Error"));
            Assert.Equal("Database error", pageModel.TempData["Error"]);
        }
    }
}
