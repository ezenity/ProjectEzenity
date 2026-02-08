using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Options;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading;
using Ezenity.Infrastructure.Helpers;
using Ezenity.Core.Services.Common;
using Ezenity.Core.Services.Emails;
using Ezenity.Infrastructure.Services.Accounts;
using Ezenity.DTOs.Models.Accounts;
using Ezenity.Core.Entities.Accounts;
using Ezenity.Core.Entities.Emails;
using Ezenity.Core.Interfaces;
using System.Linq;
using System.Collections.Generic;
using Moq.EntityFrameworkCore;
using Ezenity.Tests.Mocks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ezenity.Tests.AccountTests
{
    [TestClass]
    public class AccountServiceTests
    {
        private Mock<IDataContext> mockContext;
        private MockDbTransaction mockTransaction;
        private Mock<IMapper> mockMapper;
        //private Mock<IOptions<AppSettings>> mockAppSettings;
        private Mock<IAppSettings> mockAppSettings;
        private Mock<IEmailService> mockEmailService;
        private Mock<ILogger<AccountService>> mockLogger;
        private Mock<ITokenHelper> mockTokenHelper;
        private Mock<IAuthService> mockAuthService;
        private Mock<IPasswordService> mockPasswordService;
        private AccountService accountService;

        [TestInitialize]
        public void Setup()
        {
            // Initialize other mocks
            mockMapper = new Mock<IMapper>();
            //mockAppSettings = new Mock<IOptions<AppSettings>>();
            mockAppSettings = new Mock<IAppSettings>();
            mockEmailService = new Mock<IEmailService>();
            mockLogger = new Mock<ILogger<AccountService>>();
            mockTokenHelper = new Mock<ITokenHelper>();
            mockAuthService = new Mock<IAuthService>();
            mockPasswordService = new Mock<IPasswordService>();

            // Mocking the application settings
            mockAppSettings.Setup(m => m.BaseUrl).Returns("http://test.com");
            mockAppSettings.Setup(m => m.Secret).Returns("testSecret");
            mockAppSettings.Setup(m => m.RefreshTokenTTL).Returns(7);
            mockAppSettings.Setup(m => m.EmailFrom).Returns("test@ezenity.com");
            mockAppSettings.Setup(m => m.SmtpHost).Returns("smtp.gmail.com");
            mockAppSettings.Setup(m => m.SmtpPort).Returns(465);
            mockAppSettings.Setup(m => m.SmtpUser).Returns("test@ezenity.com");
            mockAppSettings.Setup(m => m.SmtpPass).Returns("test12345");
            mockAppSettings.Setup(m => m.SmtpEnabledSsl).Returns(true);
            mockAppSettings.Setup(m => m.AccessToken).Returns("testAccessToken");

            // Mock the IMapper to return a non-null Account object
            var account = new Account("Mr.", "Tony", "Mac", "test@ezenity.com")
            {
                Id = 1,
                PasswordHash = "$2a$11$sjVA/GVTJSFf1gEQA85ElODocaeXsJV7u9jLxFvqNxXngsIkOEmF6",
                AcceptTerms = true,
                RoleId = 1,
                Verified = DateTime.UtcNow,
                Created = DateTime.UtcNow
            };
            mockMapper.Setup(m => m.Map<Account>(It.IsAny<RegisterRequest>())).Returns(account);

            // Mocking DbSet for Accounts
            var mockAccountSet = new List<Account>
            {
                new Account("Mr.","Tony", "Mac", "test@ezenity.com")
                {
                    Id = 1,
                    PasswordHash = "$2a$11$sjVA/GVTJSFf1gEQA85ElODocaeXsJV7u9jLxFvqNxXngsIkOEmF6",
                    AcceptTerms = true,
                    RoleId = 1,
                    Verified = DateTime.UtcNow,
                    Created = DateTime.UtcNow
                }
            };

            // Mocking DbSet for Roles
            var mockRoleSet = new List<Role>
            {
                new Role
                {
                    Id = 1,
                    Name = "Admin"
                }
            };

            mockContext = new Mock<IDataContext>();
            mockContext.Setup(c => c.Accounts).ReturnsDbSet(mockAccountSet);
            mockContext.Setup(c => c.Roles).ReturnsDbSet(mockRoleSet);

            mockTransaction = new MockDbTransaction();

            mockContext.Setup(c => c.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((IDbContextTransaction)mockTransaction));

            // Create a MapperConfiguration by adding the profile
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));

            // Use the configuration in the mock setup
            mockMapper.Setup(m => m.ConfigurationProvider).Returns(config);


            // Initialize the service to be tested
            accountService = new AccountService(mockContext.Object, mockMapper.Object, mockAppSettings.Object, mockEmailService.Object, mockLogger.Object, mockTokenHelper.Object, mockAuthService.Object, mockPasswordService.Object);
        }

        [TestMethod]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsAuthenticatedUser()
        {
            // Arrange
            var model = new AuthenticateRequest { Email = "test@ezenity.com", Password = "test12345" };
            var ipAddress = "127.0.0.1";

            // Mock Verify Password
            mockPasswordService.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // Mock the Mapper to return a non-null AuthenticateResponse object
            var authResponse = new AuthenticateResponse
            {
                Id = 1,
                Title = "Mr.",
                FirstName = "Tony",
                LastName = "Mac",
                Email = "test@ezenity.com",
                Role = "Admin",
                Created = DateTime.UtcNow,
                IsVerified = true,
                JwtToken = "jwt_token",
                RefreshToken = "refresh_token"
            };
            mockMapper.Setup(m => m.Map<AuthenticateResponse>(It.IsAny<Account>())).Returns(authResponse);

            // Setup Token Helper Mock
            mockTokenHelper.Setup(x => x.GenerateJwtToken(It.IsAny<int>())).Returns("jwt_token");
            mockTokenHelper.Setup(x => x.GenerateNewRefreshToken(It.IsAny<string>())).Returns(new RefreshToken { Token = "refresh_token" });

            // Mock Update and SaveChangesAsync methods
            mockContext.Setup(c => c.Update(It.IsAny<Account>()));
            mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            Assert.IsNotNull(mockMapper.Object);
            Assert.IsNotNull(mockTokenHelper.Object);
            Assert.IsNotNull(mockContext.Object);

            // Act
            var result = await accountService.AuthenticateAsync(model, ipAddress);

            // Verify that the mocked methods were called
            mockTokenHelper.Verify(x => x.GenerateJwtToken(It.IsAny<int>()), Times.Once);
            mockTokenHelper.Verify(x => x.GenerateNewRefreshToken(It.IsAny<string>()), Times.Once);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("jwt_token", result.JwtToken);
            Assert.AreEqual("refresh_token", result.RefreshToken);
        }

        [TestMethod]
        public async Task RegisterAsync_ValidModel_SendsVerificationEmail()
        {
            // Arrange
            var model = new RegisterRequest {
                Title = "Mr.",
                FirstName = "John",
                LastName = "Doe",
                Email = "test@email.com",
                Password = "password",
                ConfirmPassword = "password",
                AcceptTerms = true
            };

            /*var account = new Account("Mr.", "John", "Doe", "test@email.com");
            mockMapper.Setup(m => m.Map<Account>(It.IsAny<RegisterRequest>())).Returns(account);*/


            var origin = "http://localhost";

            mockEmailService.Setup(x => x.SendEmailAsync(It.IsAny<EmailMessage>())).Verifiable();

            // Act
            await accountService.RegisterAsync(model, origin);

            // Assert
            mockEmailService.Verify(x => x.SendEmailAsync(It.IsAny<EmailMessage>()), Times.Once);

        }
    }
}
