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
using Ezenity.Infrastructure.Services.Accounts;
using Ezenity.DTOs.Models.Accounts;
using Ezenity.Core.Entities.Accounts;
using Ezenity.Core.Entities.Emails;

namespace Ezenity.Tests.AccountTests
{
    [TestClass]
    public class AccountServiceTests
    {
        private Mock<DataContext> mockContext;
        private Mock<IMapper> mockMapper;
        private Mock<IOptions<AppSettings>> mockAppSettings;
        private Mock<IEmailService> mockEmailService;
        private Mock<ILogger<AccountService>> mockLogger;
        private Mock<TokenHelper> mockTokenHelper;
        private Mock<IAuthService> mockAuthService;
        private AccountService accountService;

        [TestInitialize]
        public void Setup()
        {
            mockContext = new Mock<DataContext>();
            mockMapper = new Mock<IMapper>();
            mockAppSettings = new Mock<IOptions<AppSettings>>();
            mockEmailService = new Mock<IEmailService>();
            mockLogger = new Mock<ILogger<AccountService>>();
            mockTokenHelper = new Mock<TokenHelper>();
            mockAuthService = new Mock<IAuthService>();

            accountService = new AccountService(mockContext.Object, mockMapper.Object, mockAppSettings.Object, mockEmailService.Object, mockLogger.Object, mockTokenHelper.Object, mockAuthService.Object);
        }

        [TestMethod]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsAuthenticatedUser()
        {
            // Arrange
            var model = new AuthenticateRequest { Email = "test@email.com", Password = "password" };
            var ipAddress = "127.0.0.1";

            // Create mockAccount with cosntructor
            var mockAccount = new Account("Mr.", "Test", "User", "test@email.com") { Id = 1, PasswordHash = "hashed_password" };

            mockContext.Setup(x => x.Accounts.SingleOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(mockAccount);

            mockTokenHelper.Setup(x => x.GenerateJwtToken(It.IsAny<int>())).Returns("jwt_token");
            mockTokenHelper.Setup(x => x.GenerateNewRefreshToken(It.IsAny<string>())).Returns(new RefreshToken { Token = "refresh_token" });

            // Act
            var result = await accountService.AuthenticateAsync(model, ipAddress);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("jwt_token", result.JwtToken);
            Assert.AreEqual("refresh_token", result.RefreshToken);

        }

        [TestMethod]
        public async Task RegisterAsync_ValidModel_SendsVerificationEmail()
        {
            // Arrange
            var model = new RegisterRequest { Email = "test@email.com", Password = "password", FirstName = "John", LastName = "Doe" };
            var origin = "http://localhost";

            mockEmailService.Setup(x => x.SendEmailAsync(It.IsAny<EmailMessage>())).Verifiable();

            // Act
            await accountService.RegisterAsync(model, origin);

            // Assert
            mockEmailService.Verify(x => x.SendEmailAsync(It.IsAny<EmailMessage>()), Times.Once);

        }

        [TestMethod]
        public async Task GetByIdAsync_ValidId_ReturnsAccount()
        {
            // Arrange
            int id = 1;

            // Create mockAccount and mockAccountResposne with constructors
            var mockAccount = new Account("Mr.", "Test", "User", "test@email.com") { Id = id };
            var mockAccountResponse = new AccountResponse("Mr.", "Test", "User", "test@email.com") { Id = id };

            mockContext.Setup(x => x.Accounts.FindAsync(It.IsAny<int>())).ReturnsAsync(mockAccount);
            mockMapper.Setup(x => x.Map<AccountResponse>(It.IsAny<Account>())).Returns(mockAccountResponse);

            // Act
            var result = await accountService.GetByIdAsync(id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.Id);

        }
    }
}
