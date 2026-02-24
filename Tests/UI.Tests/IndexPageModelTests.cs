using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NewsWebsite.Pages;
using Xunit;

namespace UI.Tests.Pages
{
    public class IndexModelTests
    {
        private readonly Mock<INewsService> _newsServiceMock;

        public IndexModelTests()
        {
            _newsServiceMock = new Mock<INewsService>();
        }

        private IndexModel CreatePageModel()
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            var pageContext = new PageContext
            {
                HttpContext = httpContext
            };

            return new IndexModel(_newsServiceMock.Object)
            {
                PageContext = pageContext,
                TempData = tempData
            };
        }

        [Fact]
        public async Task OnGetAsync_LoadsNewsSortedByDate()
        {
            // Arrange
            var expectedNews = new List<NewsDTO>
            {
                new NewsDTO { Id = 1, Title = "Новина 1", Date = DateTime.UtcNow },
                new NewsDTO { Id = 2, Title = "Новина 2", Date = DateTime.UtcNow.AddDays(-1) }
            };

            _newsServiceMock.Setup(s => s.GetSortedByDate(true, 1, 20))
                .ReturnsAsync(expectedNews);

            var pageModel = CreatePageModel();

            // Act
            await pageModel.OnGetAsync();

            // Assert
            Assert.NotNull(pageModel.NewsList);
            Assert.Equal(2, pageModel.NewsList.Count);
            Assert.Equal("Новина 1", pageModel.NewsList[0].Title);
            _newsServiceMock.Verify(s => s.GetSortedByDate(true, 1, 20), Times.Once);
        }

        [Fact]
        public async Task OnGetAsync_ServiceThrowsException_SetsTempDataError()
        {
            // Arrange
            _newsServiceMock.Setup(s => s.GetSortedByDate(true, 1, 20))
                .ThrowsAsync(new Exception("Database error"));

            var pageModel = CreatePageModel();

            // Act
            await pageModel.OnGetAsync();

            // Assert
            Assert.Empty(pageModel.NewsList);
            Assert.True(pageModel.TempData.ContainsKey("Error"));
            Assert.Equal("Database error", pageModel.TempData["Error"]);
        }

        [Fact]
        public async Task OnGetAsync_EmptyNewsList_ReturnsEmptyList()
        {
            // Arrange
            _newsServiceMock.Setup(s => s.GetSortedByDate(true, 1, 20))
                .ReturnsAsync(new List<NewsDTO>());

            var pageModel = CreatePageModel();

            // Act
            await pageModel.OnGetAsync();

            // Assert
            Assert.NotNull(pageModel.NewsList);
            Assert.Empty(pageModel.NewsList);
        }
    }
}
