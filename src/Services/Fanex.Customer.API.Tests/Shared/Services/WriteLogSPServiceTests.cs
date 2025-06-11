using System.Threading.Tasks;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Fanex.Logging;
using Moq;
using Xunit;

namespace Fanex.Customer.API.Shared.Services.Tests
{
    public class WriteLogSPServiceTests
    {
        private readonly Mock<IDynamicRepository> _dynamicRepositoryMock = new();
        private readonly Mock<ILogger> _loggerMock = new();
        private readonly WriteLogSPService _service;

        public WriteLogSPServiceTests()
        {
            _service = new WriteLogSPService(_dynamicRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task WriteLogSPAsync_CallsDbLogFirst_AndFallsBackOnException()
        {
            // Arrange
            _dynamicRepositoryMock.Setup(r => r.ExecuteAsync(It.IsAny<WriteLogSPAtDBLOGCommand>()))
                .ThrowsAsync(new System.Exception("DBLOG failed"));
            _dynamicRepositoryMock.Setup(r => r.ExecuteAsync(It.IsAny<WriteLogSPCommand>()))
                .ReturnsAsync(0);

            // Act
            await _service.WriteLogSPAsync(1, 2, 3, "val1", "val2", 10, 5);

            // Assert
            _dynamicRepositoryMock.Verify(r => r.ExecuteAsync(It.IsAny<WriteLogSPAtDBLOGCommand>()), Times.Once);
            _dynamicRepositoryMock.Verify(r => r.ExecuteAsync(It.IsAny<WriteLogSPCommand>()), Times.Once);
            _loggerMock.Verify(l => l.ErrorAsync(It.Is<string>(s => s.Contains("DBLOG failed")), It.IsAny<System.Exception>()), Times.Once);
        }

        [Fact]
        public async Task WriteLogSPAsync_OnlyCallsDbLog_WhenNoException()
        {
            // Arrange
            _dynamicRepositoryMock.Setup(r => r.ExecuteAsync(It.IsAny<WriteLogSPAtDBLOGCommand>()))
                .ReturnsAsync(0);

            // Act
            await _service.WriteLogSPAsync(1, 2, 3, "val1", "val2", 10, 5);

            // Assert
            _dynamicRepositoryMock.Verify(r => r.ExecuteAsync(It.IsAny<WriteLogSPAtDBLOGCommand>()), Times.Once);
            _dynamicRepositoryMock.Verify(r => r.ExecuteAsync(It.IsAny<WriteLogSPCommand>()), Times.Never);
            _loggerMock.Verify(l => l.ErrorAsync(It.IsAny<string>(), It.IsAny<System.Exception>()), Times.Never);
        }

        [Fact]
        public async Task WriteLogSPAsync_LogsError_WhenBothFail()
        {
            // Arrange
            _dynamicRepositoryMock.Setup(r => r.ExecuteAsync(It.IsAny<WriteLogSPAtDBLOGCommand>()))
                .ThrowsAsync(new System.Exception("DBLOG failed"));
            _dynamicRepositoryMock.Setup(r => r.ExecuteAsync(It.IsAny<WriteLogSPCommand>()))
                .ThrowsAsync(new System.Exception("SP failed"));

            // Act
            await _service.WriteLogSPAsync(1, 2, 3, "val1", "val2", 10, 5);

            // Assert
            _dynamicRepositoryMock.Verify(r => r.ExecuteAsync(It.IsAny<WriteLogSPAtDBLOGCommand>()), Times.Once);
            _dynamicRepositoryMock.Verify(r => r.ExecuteAsync(It.IsAny<WriteLogSPCommand>()), Times.Once);
            _loggerMock.Verify(l => l.ErrorAsync(It.Is<string>(s => s.Contains("DBLOG failed")), It.IsAny<System.Exception>()), Times.Once);
            _loggerMock.Verify(l => l.ErrorAsync(It.Is<string>(s => s.Contains("SP failed")), It.IsAny<System.Exception>()), Times.Once);
        }
    }
}
