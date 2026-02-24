using AutoMapper;
using BLL;
using BLL.DTO;
using BLL.Services;
using DAL;
using DAL.Models;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace Tests.BLL.Tests
{
    public class NewsServiceQueryTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepository<News>> _repositoryMock;
        private readonly NewsService _newsService;

        public NewsServiceQueryTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();

            _repositoryMock = new Mock<IRepository<News>>();
            _newsService = new NewsService(_mapper, _repositoryMock.Object);
        }

        #region GetByCategory Tests

        [Fact]
        public async Task GetByCategory_ReturnsNewsInCategory()
        {
            // Arrange
            var categoryId = 1;
            var category = new Category { Id = categoryId, Name = "Спорт" };
            var author = new User { Id = 1, Username = "author1" };

            var newsList = new List<News>
            {
                new News 
                { 
                    Id = 1, 
                    Title = "Спортивна новина 1",
                    CategoryId = categoryId,
                    Category = category,
                    AuthorId = 1,
                    Author = author
                },
                new News 
                { 
                    Id = 2, 
                    Title = "Спортивна новина 2",
                    CategoryId = categoryId,
                    Category = category,
                    AuthorId = 1,
                    Author = author
                }
            }.AsQueryable().BuildMock();

            _repositoryMock.Setup(r => r.GetAll()).Returns(newsList);

            // Act
            var result = await _newsService.GetByCategory(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, n => Assert.Equal(categoryId, n.CategoryId));
        }

        [Fact]
        public async Task GetByCategory_NoNewsInCategory_ReturnsEmptyList()
        {
            // Arrange
            var categoryId = 999;
            var newsList = new List<News>().AsQueryable().BuildMock();

            _repositoryMock.Setup(r => r.GetAll()).Returns(newsList);

            // Act
            var result = await _newsService.GetByCategory(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region GetSortedByDate Tests

        [Fact]
        public async Task GetSortedByDate_Descending_ReturnsSortedNews()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Test" };
            var author = new User { Id = 1, Username = "author" };

            var newsList = new List<News>
            {
                new News 
                { 
                    Id = 1, 
                    Title = "Стара новина",
                    Date = DateTime.UtcNow.AddDays(-5),
                    CategoryId = 1,
                    Category = category,
                    AuthorId = 1,
                    Author = author
                },
                new News 
                { 
                    Id = 2, 
                    Title = "Нова новина",
                    Date = DateTime.UtcNow.AddDays(-1),
                    CategoryId = 1,
                    Category = category,
                    AuthorId = 1,
                    Author = author
                },
                new News 
                { 
                    Id = 3, 
                    Title = "Середня новина",
                    Date = DateTime.UtcNow.AddDays(-3),
                    CategoryId = 1,
                    Category = category,
                    AuthorId = 1,
                    Author = author
                }
            }.AsQueryable().BuildMock();

            _repositoryMock.Setup(r => r.GetAll()).Returns(newsList);

            // Act
            var result = await _newsService.GetSortedByDate(descending: true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            // Проверяем что самая новая новина первая
            Assert.Equal("Нова новина", result[0].Title);
            Assert.Equal("Стара новина", result[2].Title);
        }

        [Fact]
        public async Task GetSortedByDate_Ascending_ReturnsSortedNews()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Test" };
            var author = new User { Id = 1, Username = "author" };

            var newsList = new List<News>
            {
                new News 
                { 
                    Id = 1, 
                    Title = "Стара новина",
                    Date = DateTime.UtcNow.AddDays(-5),
                    CategoryId = 1,
                    Category = category,
                    AuthorId = 1,
                    Author = author
                },
                new News 
                { 
                    Id = 2, 
                    Title = "Нова новина",
                    Date = DateTime.UtcNow.AddDays(-1),
                    CategoryId = 1,
                    Category = category,
                    AuthorId = 1,
                    Author = author
                }
            }.AsQueryable().BuildMock();

            _repositoryMock.Setup(r => r.GetAll()).Returns(newsList);

            // Act
            var result = await _newsService.GetSortedByDate(descending: false);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Стара новина", result[0].Title);
            Assert.Equal("Нова новина", result[1].Title);
        }

        [Fact]
        public async Task GetSortedByDate_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Test" };
            var author = new User { Id = 1, Username = "author" };

            var newsList = new List<News>();
            for (int i = 1; i <= 50; i++)
            {
                newsList.Add(new News 
                { 
                    Id = i,
                    Title = $"Новина {i}",
                    Date = DateTime.UtcNow.AddDays(-i),
                    CategoryId = 1,
                    Category = category,
                    AuthorId = 1,
                    Author = author
                });
            }

            var mockNews = newsList.AsQueryable().BuildMock();
            _repositoryMock.Setup(r => r.GetAll()).Returns(mockNews);

            // Act
            var result = await _newsService.GetSortedByDate(descending: true, page: 2, pageSize: 20);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(20, result.Count);
            // Перевіряємо що це друга сторінка (новини 21-40)
            Assert.Equal("Новина 21", result[0].Title);
        }

        #endregion

        #region GetPopular Tests

        [Fact]
        public async Task GetPopular_ReturnsNewsAboveMinViews()
        {
            // Arrange
            var minViews = 100;
            var newsList = new List<News>
            {
                new News { Id = 1, Title = "Дуже популярна", Views = 500 },
                new News { Id = 2, Title = "Популярна", Views = 150 },
                new News { Id = 3, Title = "Непопулярна", Views = 50 },
                new News { Id = 4, Title = "Середня", Views = 100 }
            }.AsQueryable().BuildMock();

            _repositoryMock.Setup(r => r.GetAll()).Returns(newsList);

            // Act
            var result = await _newsService.GetPopular(minViews);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.All(result, n => Assert.True(n.Views >= minViews));
        }

        [Fact]
        public async Task GetPopular_OrdersByViewsDescending()
        {
            // Arrange
            var newsList = new List<News>
            {
                new News { Id = 1, Title = "Новина 1", Views = 100 },
                new News { Id = 2, Title = "Новина 2", Views = 500 },
                new News { Id = 3, Title = "Новина 3", Views = 300 }
            }.AsQueryable().BuildMock();

            _repositoryMock.Setup(r => r.GetAll()).Returns(newsList);

            // Act
            var result = await _newsService.GetPopular(100);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal(500, result[0].Views);
            Assert.Equal(300, result[1].Views);
            Assert.Equal(100, result[2].Views);
        }

        [Fact]
        public async Task GetPopular_NoNewsAboveMinViews_ReturnsEmptyList()
        {
            // Arrange
            var minViews = 1000;
            var newsList = new List<News>
            {
                new News { Id = 1, Views = 100 },
                new News { Id = 2, Views = 200 }
            }.AsQueryable().BuildMock();

            _repositoryMock.Setup(r => r.GetAll()).Returns(newsList);

            // Act
            var result = await _newsService.GetPopular(minViews);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetPopular_ZeroMinViews_ReturnsAllNews()
        {
            // Arrange
            var newsList = new List<News>
            {
                new News { Id = 1, Views = 0 },
                new News { Id = 2, Views = 100 },
                new News { Id = 3, Views = 50 }
            }.AsQueryable().BuildMock();

            _repositoryMock.Setup(r => r.GetAll()).Returns(newsList);

            // Act
            var result = await _newsService.GetPopular(0);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
        }

        #endregion
    }
}
