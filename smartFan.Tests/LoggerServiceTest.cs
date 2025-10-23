using Xunit;
using TempControl.Services;
using System.IO;

namespace smartFan.Tests
{
    public class LoggerServiceTest : IDisposable
    {
        private readonly string _testLogFilePath;
        private readonly Logger _logger;

        public LoggerServiceTest()
        {
            _testLogFilePath = Path.GetTempFileName();
            _logger = new Logger(_testLogFilePath);
        }

        [Fact]
        public void Log_ShouldWriteMessageToFile()
        {
            // Arrange
            string testMessage = "Test log message";

            // Act
            _logger.Log(testMessage);

            // Assert
            string logContent = File.ReadAllText(_testLogFilePath);
            Assert.Contains(testMessage, logContent);
        }

        [Fact]
        public void Log_ShouldIncludeTimestamp()
        {
            // Arrange
            string testMessage = "Test with timestamp";

            // Act
            _logger.Log(testMessage);

            // Assert
            string logContent = File.ReadAllText(_testLogFilePath);
            Assert.Contains(DateTime.Now.ToString("yyyy-MM-dd"), logContent);
        }

        public void Dispose()
        {
            if (File.Exists(_testLogFilePath))
            {
                File.Delete(_testLogFilePath);
            }
        }
    }
}