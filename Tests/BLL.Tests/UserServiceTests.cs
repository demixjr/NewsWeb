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
    public class UserServiceTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepository<User>> _repositoryMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();

            _repositoryMock = new Mock<IRepository<User>>();
            _userService = new UserService(_mapper, _repositoryMock.Object);
        }

        [Fact]
        public async Task AddUser_ValidUser_ReturnsTrue()
        {
            // Arrange
            var userDTO = new UserDTO 
            { 
                Username = "testuser", 
                Password = "password123",
                Role = "User"
            };

            var users = new List<User>().AsQueryable().BuildMock();
            _repositoryMock.Setup(r => r.GetAll()).Returns(users);
            _repositoryMock.Setup(r => r.Add(It.IsAny<User>())).Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.SaveChanges()).Returns(Task.CompletedTask);

            // Act
            var result = await _userService.AddUser(userDTO);

            // Assert
            Assert.True(result);
            _repositoryMock.Verify(r => r.Add(It.Is<User>(u => 
                u.Username == "testuser" && 
                !string.IsNullOrEmpty(u.PasswordHash)
            )), Times.Once);
            _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public async Task AddUser_DuplicateUsername_ThrowsValidationException()
        {
            // Arrange
            var userDTO = new UserDTO 
            { 
                Username = "existinguser", 
                Password = "password123" 
            };

            var existingUser = new User 
            { 
                Id = 1, 
                Username = "existinguser",
                PasswordHash = "hashedpassword"
            };

            var users = new List<User> { existingUser }.AsQueryable().BuildMock();
            _repositoryMock.Setup(r => r.GetAll()).Returns(users);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _userService.AddUser(userDTO)
            );

            Assert.Equal("Такий користувач вже існує", exception.Message);
            _repositoryMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task AddUser_PasswordIsHashed()
        {
            // Arrange
            var userDTO = new UserDTO 
            { 
                Username = "testuser", 
                Password = "plainpassword" 
            };

            var users = new List<User>().AsQueryable().BuildMock();
            _repositoryMock.Setup(r => r.GetAll()).Returns(users);
            _repositoryMock.Setup(r => r.SaveChanges()).Returns(Task.CompletedTask);

            User? capturedUser = null;
            _repositoryMock.Setup(r => r.Add(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .Returns(Task.CompletedTask);

            // Act
            await _userService.AddUser(userDTO);

            // Assert
            Assert.NotNull(capturedUser);
            Assert.NotEqual("plainpassword", capturedUser.PasswordHash);
            Assert.True(BCrypt.Net.BCrypt.Verify("plainpassword", capturedUser.PasswordHash));
        }

        [Fact]
        public async Task Authenticate_ValidCredentials_ReturnsUserDTO()
        {
            // Arrange
            var password = "password123";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 4);

            var user = new User 
            { 
                Id = 1, 
                Username = "testuser",
                PasswordHash = passwordHash,
                Role = "User"
            };

            var users = new List<User> { user }.AsQueryable().BuildMock();
            _repositoryMock.Setup(r => r.GetAll()).Returns(users);

            // Act
            var result = await _userService.Authenticate("testuser", password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
            Assert.Equal("User", result.Role);
        }

        [Fact]
        public async Task Authenticate_InvalidPassword_ReturnsNull()
        {
            // Arrange
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword", workFactor: 4);

            var user = new User 
            { 
                Id = 1, 
                Username = "testuser",
                PasswordHash = passwordHash
            };

            var users = new List<User> { user }.AsQueryable().BuildMock();
            _repositoryMock.Setup(r => r.GetAll()).Returns(users);

            // Act
            var result = await _userService.Authenticate("testuser", "wrongpassword");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Authenticate_NonExistentUser_ReturnsNull()
        {
            // Arrange
            var users = new List<User>().AsQueryable().BuildMock();
            _repositoryMock.Setup(r => r.GetAll()).Returns(users);

            // Act
            var result = await _userService.Authenticate("nonexistent", "password");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindUserByUsername_ExistingUser_ReturnsUserDTO()
        {
            // Arrange
            var user = new User 
            { 
                Id = 1, 
                Username = "testuser",
                Role = "User"
            };

            var users = new List<User> { user }.AsQueryable().BuildMock();
            _repositoryMock.Setup(r => r.GetAll()).Returns(users);

            // Act
            var result = await _userService.FindUserByUsername("testuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task FindUserByUsername_NonExistentUser_ReturnsNull()
        {
            // Arrange
            var users = new List<User>().AsQueryable().BuildMock();
            _repositoryMock.Setup(r => r.GetAll()).Returns(users);

            // Act
            var result = await _userService.FindUserByUsername("nonexistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindUserByUsername_NullUsername_ReturnsNull()
        {
            // Arrange
            var users = new List<User>().AsQueryable().BuildMock();
            _repositoryMock.Setup(r => r.GetAll()).Returns(users);

            // Act
            var result = await _userService.FindUserByUsername(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteUser_ExistingUser_ReturnsTrue()
        {
            // Arrange
            var userDTO = new UserDTO 
            { 
                Username = "testuser" 
            };

            var user = new User 
            { 
                Id = 1, 
                Username = "testuser" 
            };

            _repositoryMock.Setup(r => r.Find(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
                .ReturnsAsync(user);
            _repositoryMock.Setup(r => r.Remove(user)).Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.SaveChanges()).Returns(Task.CompletedTask);

            // Act
            var result = await _userService.DeleteUser(userDTO);

            // Assert
            Assert.True(result);
            _repositoryMock.Verify(r => r.Remove(user), Times.Once);
            _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_NonExistentUser_ThrowsException()
        {
            // Arrange
            var userDTO = new UserDTO 
            { 
                Username = "nonexistent" 
            };

            _repositoryMock.Setup(r => r.Find(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _userService.DeleteUser(userDTO)
            );

            Assert.Equal("Неможливо видалити користувача, оскільки його не знайдено в базі даних", 
                exception.Message);
            _repositoryMock.Verify(r => r.Remove(It.IsAny<User>()), Times.Never);
        }
    }
}
