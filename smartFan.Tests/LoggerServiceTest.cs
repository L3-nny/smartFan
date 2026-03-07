using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using smartFan.Services;
using smartFan.Services.Interfaces;
using smartFan.Repositories.Interfaces;
using System.IO;
using System;

namespace smartFan.Tests
{
    public class LoggerServiceTest : IDisposable
    {
        private readonly string _testLogDirectory;
        private readonly Mock<IErrorLogRepository> _mockRepository;
        private readonly Mock<IHostEnvironment> _mockEnvironment;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly LoggerService _loggerService;

        public LoggerServiceTest()
        {
            _testLogDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testLogDirectory);

            _mockRepository = new Mock<IErrorLogRepository>();
            _mockEnvironment = new Mock<IHostEnvironment>();
            _mockConfig = new Mock<IConfiguration>();

            // Setup mock configuration
            _mockConfig.Setup(c => c["LogSettings:Directory"]).Returns(_testLogDirectory);
            _mockConfig.Setup(c => c["LogSettings:EnableConsole"]).Returns("false");
            _mockConfig.Setup(c => c["LogSettings:Level"]).Returns("Info");
            _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Test");

            _loggerService = new LoggerService(_mockRepository.Object, _mockEnvironment.Object, _mockConfig.Object);
        }

        [Fact]
        public void LogInfo_ShouldWriteMessageToFile()
        {
            // Arrange
            string testMessage = "Test info message";

            // Act
            _loggerService.LogInfo(testMessage);

            // Assert
            string[] logFiles = Directory.GetFiles(_testLogDirectory, "*.txt");
            Assert.NotEmpty(logFiles);
            
            string logContent = File.ReadAllText(logFiles[0]);
            Assert.Contains(testMessage, logContent);
            Assert.Contains("[Info]", logContent);
        }

        [Fact]
        public void LogError_ShouldCallRepository()
        {
            // Arrange
            string testMessage = "Test error message";
            var exception = new Exception("Test exception");

            // Act
            _loggerService.LogError(testMessage, exception);

            // Assert - Verify that repository AddAsync was called
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<smartFan.Models.ErrorLogModel>()), Times.Once);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testLogDirectory))
            {
                Directory.Delete(_testLogDirectory, true);
            }
        }
    }
}