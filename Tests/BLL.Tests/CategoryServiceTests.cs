using AutoMapper;
using BLL;
using BLL.DTO;
using BLL.Services;
using DAL;
using DAL.Models;
using MockQueryable.Moq;
using Moq;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Tests.BLL.Tests
{
    public class CategoryServiceTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepository<Category>> _repositoryMock;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();

            _repositoryMock = new Mock<IRepository<Category>>();
            _categoryService = new CategoryService(_mapper, _repositoryMock.Object);
        }

        [Fact]
        public async Task AddCategory_ValidCategory_ReturnsTrue()
        {
            // Arrange
            var categoryDTO = new CategoryDTO { Name = "Спорт" };

            var categories = new List<Category>().AsQueryable().BuildMock();
            _repositoryMock.Setup(r => r.GetAll()).Returns(categories);
            _repositoryMock.Setup(r => r.Add(It.IsAny<Category>())).Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.SaveChanges()).Returns(Task.CompletedTask);

            // Act
            var result = await _categoryService.AddCategory(categoryDTO);

            // Assert
            Assert.True(result);
            _repositoryMock.Verify(r => r.Add(It.Is<Category>(c => c.Name == "Спорт")), Times.Once);
            _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public async Task AddCategory_DuplicateName_ThrowsValidationException()
        {
            // Arrange
            var categoryDTO = new CategoryDTO { Name = "Спорт" };
            var existingCategory = new Category { Id = 1, Name = "Спорт" };

            var categories = new List<Category> { existingCategory }.AsQueryable().BuildMock();
            _repositoryMock.Setup(r => r.GetAll()).Returns(categories);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _categoryService.AddCategory(categoryDTO)
            );

            Assert.Equal("Така категорія вже існує", exception.Message);
            _repositoryMock.Verify(r => r.Add(It.IsAny<Category>()), Times.Never);
            _repositoryMock.Verify(r => r.SaveChanges(), Times.Never);
        }

        [Fact]
        public async Task AddCategory_NullName_AddsSuccessfully()
        {
            // Arrange
            var categoryDTO = new CategoryDTO { Name = null };

            var categories = new List<Category>().AsQueryable().BuildMock();
            _repositoryMock.Setup(r => r.GetAll()).Returns(categories);
            _repositoryMock.Setup(r => r.Add(It.IsAny<Category>())).Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.SaveChanges()).Returns(Task.CompletedTask);

            // Act
            var result = await _categoryService.AddCategory(categoryDTO);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetAllCategories_ReturnsAllCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Спорт", News = new List<News>() },
                new Category { Id = 2, Name = "Політика", News = new List<News>() },
                new Category { Id = 3, Name = "Технології", News = new List<News>() }
            }.AsQueryable().BuildMock();

            _repositoryMock.Setup(r => r.GetAll()).Returns(categories);

            // Act
            var result = await _categoryService.GetAllCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("Спорт", result[0].Name);
            Assert.Equal("Політика", result[1].Name);
            Assert.Equal("Технології", result[2].Name);
        }

        [Fact]
        public async Task GetAllCategories_EmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            var categories = new List<Category>().AsQueryable().BuildMock();
            _repositoryMock.Setup(r => r.GetAll()).Returns(categories);

            // Act
            var result = await _categoryService.GetAllCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllCategories_CategoriesWithNews_IncludesNews()
        {
            // Arrange
            var news = new List<News>
            {
                new News { Id = 1, Title = "Тестова новина", CategoryId = 1, AuthorId = 1 }
            };

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Спорт", News = news }
            }.AsQueryable().BuildMock();

            _repositoryMock.Setup(r => r.GetAll()).Returns(categories);

            // Act
            var result = await _categoryService.GetAllCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Single(result[0].News);
            Assert.Equal("Тестова новина", result[0].News[0].Title);
        }
    }
}
