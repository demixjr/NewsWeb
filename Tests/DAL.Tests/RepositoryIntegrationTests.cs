using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.DAL.Tests
{
    public class RepositoryIntegrationTests : IDisposable
    {
        private readonly NewsDbContext _context;
        private readonly Repository<News> _newsRepository;
        private readonly Repository<Category> _categoryRepository;
        private readonly Repository<User> _userRepository;

        public RepositoryIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<NewsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new NewsDbContext(options);
            _newsRepository = new Repository<News>(_context);
            _categoryRepository = new Repository<Category>(_context);
            _userRepository = new Repository<User>(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region Add Tests

        [Fact]
        public async Task Add_ValidEntity_AddsToDatabase()
        {
            // Arrange
            var category = new Category { Name = "Спорт" };

            // Act
            await _categoryRepository.Add(category);
            await _categoryRepository.SaveChanges();

            // Assert
            var result = await _context.Categories.FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal("Спорт", result.Name);
        }

        [Fact]
        public async Task Add_MultipleEntities_AddsAllToDatabase()
        {
            // Arrange
            var category1 = new Category { Name = "Спорт" };
            var category2 = new Category { Name = "Політика" };

            // Act
            await _categoryRepository.Add(category1);
            await _categoryRepository.Add(category2);
            await _categoryRepository.SaveChanges();

            // Assert
            var count = await _context.Categories.CountAsync();
            Assert.Equal(2, count);
        }

        #endregion

        #region Get Tests

        [Fact]
        public async Task Get_ExistingId_ReturnsEntity()
        {
            // Arrange
            var category = new Category { Name = "Спорт" };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.Get(category.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Спорт", result.Name);
        }

        [Fact]
        public async Task Get_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await _categoryRepository.Get(999);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ReturnsAllEntities()
        {
            // Arrange
            _context.Categories.AddRange(
                new Category { Name = "Спорт" },
                new Category { Name = "Політика" },
                new Category { Name = "Технології" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.GetAll().ToListAsync();

            // Assert
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetAll_EmptyDatabase_ReturnsEmptyList()
        {
            // Act
            var result = await _categoryRepository.GetAll().ToListAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAll_AsNoTracking_DoesNotTrackEntities()
        {
            // Arrange
            var category = new Category { Name = "Спорт" };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.GetAll().FirstOrDefaultAsync();

            // Assert
            Assert.NotNull(result);
            var entry = _context.Entry(result);
            Assert.Equal(EntityState.Detached, entry.State);
        }

        #endregion

        #region Find Tests

        [Fact]
        public async Task Find_MatchingPredicate_ReturnsEntity()
        {
            // Arrange
            var category = new Category { Name = "Спорт" };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.Find(c => c.Name == "Спорт");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Спорт", result.Name);
        }

        [Fact]
        public async Task Find_NoMatch_ReturnsNull()
        {
            // Arrange
            var category = new Category { Name = "Спорт" };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.Find(c => c.Name == "Політика");

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region FindAll Tests

        [Fact]
        public async Task FindAll_MatchingPredicate_ReturnsMatchingEntities()
        {
            // Arrange
            var user = new User { Id = 1, Username = "author1" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _context.News.AddRange(
                new News { Title = "Спортивна новина 1", CategoryId = 1, AuthorId = 1, Views = 100 },
                new News { Title = "Спортивна новина 2", CategoryId = 1, AuthorId = 1, Views = 200 },
                new News { Title = "Політична новина", CategoryId = 2, AuthorId = 1, Views = 50 }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _newsRepository.FindAll(n => n.CategoryId == 1).ToListAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, n => Assert.Equal(1, n.CategoryId));
        }

        [Fact]
        public async Task FindAll_NoMatches_ReturnsEmptyList()
        {
            // Act
            var result = await _newsRepository.FindAll(n => n.CategoryId == 999).ToListAsync();

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_ExistingEntity_UpdatesInDatabase()
        {
            // Arrange
            var category = new Category { Name = "Спорт" };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Act
            category.Name = "Оновлений Спорт";
            await _categoryRepository.Update(category);
            await _categoryRepository.SaveChanges();

            // Assert
            var updated = await _context.Categories.FindAsync(category.Id);
            Assert.NotNull(updated);
            Assert.Equal("Оновлений Спорт", updated.Name);
        }

        [Fact]
        public async Task Update_MultipleProperties_UpdatesAll()
        {
            // Arrange
            var user = new User { Username = "olduser", Role = "User" };
            var category = new Category { Name = "Спорт" };
            _context.Users.Add(user);
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var news = new News 
            { 
                Title = "Стара назва",
                Description = "Старий опис",
                CategoryId = category.Id,
                AuthorId = user.Id,
                Views = 10,
                Date = DateTime.UtcNow
            };
            _context.News.Add(news);
            await _context.SaveChangesAsync();

            // Act
            news.Title = "Нова назва";
            news.Description = "Новий опис";
            news.Views = 20;
            await _newsRepository.Update(news);
            await _newsRepository.SaveChanges();

            // Assert
            var updated = await _context.News.FindAsync(news.Id);
            Assert.NotNull(updated);
            Assert.Equal("Нова назва", updated.Title);
            Assert.Equal("Новий опис", updated.Description);
            Assert.Equal(20, updated.Views);
        }

        #endregion

        #region Remove Tests

        [Fact]
        public async Task Remove_ExistingEntity_RemovesFromDatabase()
        {
            // Arrange
            var category = new Category { Name = "Спорт" };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            var categoryId = category.Id;

            // Act
            await _categoryRepository.Remove(category);
            await _categoryRepository.SaveChanges();

            // Assert
            var result = await _context.Categories.FindAsync(categoryId);
            Assert.Null(result);
        }

        [Fact]
        public async Task Remove_MultipleEntities_RemovesAll()
        {
            // Arrange
            var category1 = new Category { Name = "Спорт" };
            var category2 = new Category { Name = "Політика" };
            _context.Categories.AddRange(category1, category2);
            await _context.SaveChangesAsync();

            // Act
            await _categoryRepository.Remove(category1);
            await _categoryRepository.Remove(category2);
            await _categoryRepository.SaveChanges();

            // Assert
            var count = await _context.Categories.CountAsync();
            Assert.Equal(0, count);
        }

        #endregion

        #region SaveChanges Tests

        [Fact]
        public async Task SaveChanges_WithoutChanges_DoesNotThrow()
        {
            // Act
            var exception = await Record.ExceptionAsync(() => _categoryRepository.SaveChanges());

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task SaveChanges_CommitsAllChanges()
        {
            // Arrange
            var category1 = new Category { Name = "Спорт" };
            var category2 = new Category { Name = "Політика" };

            // Act
            await _categoryRepository.Add(category1);
            await _categoryRepository.Add(category2);
            await _categoryRepository.SaveChanges();

            // Assert
            var count = await _context.Categories.CountAsync();
            Assert.Equal(2, count);
        }

        #endregion

        #region Complex Scenarios

        [Fact]
        public async Task ComplexScenario_CRUD_Operations()
        {
            // Arrange & Add
            var category = new Category { Name = "Спорт" };
            await _categoryRepository.Add(category);
            await _categoryRepository.SaveChanges();

            // Read
            var retrieved = await _categoryRepository.Get(category.Id);
            Assert.NotNull(retrieved);
            Assert.Equal("Спорт", retrieved.Name);

            // Update
            retrieved.Name = "Оновлений Спорт";
            await _categoryRepository.Update(retrieved);
            await _categoryRepository.SaveChanges();

            var updated = await _categoryRepository.Get(category.Id);
            Assert.Equal("Оновлений Спорт", updated?.Name);

            // Delete
            await _categoryRepository.Remove(updated!);
            await _categoryRepository.SaveChanges();

            var deleted = await _categoryRepository.Get(category.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task RelatedEntities_Navigation_PropertiesWork()
        {
            // Arrange
            var user = new User { Username = "author1", Role = "Admin" };
            var category = new Category { Name = "Спорт" };
            
            _context.Users.Add(user);
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var news = new News 
            { 
                Title = "Тестова новина",
                Description = "Опис",
                CategoryId = category.Id,
                AuthorId = user.Id,
                Date = DateTime.UtcNow,
                Views = 0
            };

            await _newsRepository.Add(news);
            await _newsRepository.SaveChanges();

            // Act
            var retrievedNews = await _context.News
                .Include(n => n.Category)
                .Include(n => n.Author)
                .FirstOrDefaultAsync(n => n.Id == news.Id);

            // Assert
            Assert.NotNull(retrievedNews);
            Assert.NotNull(retrievedNews.Category);
            Assert.NotNull(retrievedNews.Author);
            Assert.Equal("Спорт", retrievedNews.Category.Name);
            Assert.Equal("author1", retrievedNews.Author.Username);
        }

        #endregion
    }
}
