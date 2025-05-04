using Application;
using Application.Commons;
using Application.IRepositories;
using Application.Services;
using Application.ViewModels.UserDTO;
using AutoMapper;
using Domain.Entities;
using Moq;
using System.Data.Common;

namespace UnitTest
{
    public class RegisterAccountTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserRepo> _mockUserRepo;
        private readonly Mock<ITokenRepo> _mockTokenRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<AppConfiguration> _appConfiguration; 
        private readonly AuthenService _service;

        public RegisterAccountTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserRepo = new Mock<IUserRepo>();
            _mockTokenRepo = new Mock<ITokenRepo>();
            _mockMapper = new Mock<IMapper>();
            _appConfiguration = new Mock<AppConfiguration>();

            _mockUnitOfWork.Setup(u => u.UserRepo).Returns(_mockUserRepo.Object);
            _mockUnitOfWork.Setup(u => u.TokenRepo).Returns(_mockTokenRepo.Object);

            _service = new AuthenService(_appConfiguration.Object,_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task RegisterAsync_EmailAlreadyExists_ReturnsError()
        {
            // Arrange
            var registerDto = new RegisterDTO { Email = "test@example.com", Password = "password" };
            _mockUserRepo.Setup(r => r.GetByEmailAsync(registerDto.Email))
                         .ReturnsAsync(new User { Email = registerDto.Email, IsDeleted = false });

            // Act
            var result = await _service.RegisterAsync(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Email is already existed", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_DbExceptionOccurs_ReturnsDatabaseError()
        {
            // Arrange
            var registerDto = new RegisterDTO { Email = "newuser@example.com", Password = "password123" };

            _mockUserRepo.Setup(r => r.GetByEmailAsync(registerDto.Email)).ReturnsAsync((User)null);
            _mockMapper.Setup(m => m.Map<User>(It.IsAny<RegisterDTO>())).Returns(new User { Email = registerDto.Email });

            // Simulate database error
            var dbException = new Mock<DbException>().Object;
            _mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>()))
                         .ThrowsAsync(dbException);

            // Act
            var result = await _service.RegisterAsync(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Database error occurred.", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_UnexpectedException_ReturnsGenericError()
        {
            // Arrange
            var registerDto = new RegisterDTO { Email = "newuser@example.com", Password = "password123" };

            _mockUserRepo.Setup(r => r.GetByEmailAsync(registerDto.Email)).ThrowsAsync(new Exception("Unexpected"));

            // Act
            var result = await _service.RegisterAsync(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Error", result.Message);
            Assert.Contains("Unexpected", result.ErrorMessages[0]);
        }


    }
}