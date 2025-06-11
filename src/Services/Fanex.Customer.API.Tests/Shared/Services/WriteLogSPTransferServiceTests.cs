using System.Threading.Tasks;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Fanex.Logging;
using Moq;
using Xunit;

namespace Fanex.Customer.API.Shared.Services.Tests
{
    public class WriteLogSPTransferServiceTests
    {
        private readonly Mock<IDynamicRepository> _dynamicRepositoryMock = new();
        private readonly Mock<ILogger> _loggerMock = new();
        private readonly WriteLogSPTransferService _service;

        public WriteLogSPTransferServiceTests()
        {
            _service = new WriteLogSPTransferService(_dynamicRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task WriteLogSPTransferAsync_CallsDbLogFirst_AndFallsBackOnException()
        {
            // Arrange
            _dynamicRepositoryMock.Setup(r => r.ExecuteAsync(It.IsAny<WriteLogSPTransferAtDBLOGCommand>()))
                .ThrowsAsync(new System.Exception("DBLOG failed"));
            _dynamicRepositoryMock.Setup(r => r.ExecuteAsync(It.IsAny<WriteLogSPTransferCommand>()))
                .ReturnsAsync(0);

            // Act
            await _service.WriteLogSPTransferAsync("logdata", 5);

            // Assert
            _dynamicRepositoryMock.Verify(r => r.ExecuteAsync(It.IsAny<WriteLogSPTransferAtDBLOGCommand>()), Times.Once);
            _dynamicRepositoryMock.Verify(r => r.ExecuteAsync(It.IsAny<WriteLogSPTransferCommand>()), Times.Once);
            _loggerMock.Verify(l => l.ErrorAsync(It.Is<string>(s => s.Contains("DBLOG failed"))), Times.Once);
        }

        [Fact]
        public async Task WriteLogSPTransferAsync_OnlyCallsDbLog_WhenNoException()
        {
            // Arrange
            _dynamicRepositoryMock.Setup(r => r.ExecuteAsync(It.IsAny<WriteLogSPTransferAtDBLOGCommand>()))
                .ReturnsAsync(0);

            // Act
            await _service.WriteLogSPTransferAsync("logdata", 5);

            // Assert
            _dynamicRepositoryMock.Verify(r => r.ExecuteAsync(It.IsAny<WriteLogSPTransferAtDBLOGCommand>()), Times.Once);
            _dynamicRepositoryMock.Verify(r => r.ExecuteAsync(It.IsAny<WriteLogSPTransferCommand>()), Times.Never);
            _loggerMock.Verify(l => l.ErrorAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task WriteLogSPTransferAsync_LogsError_WhenBothFail()
        {
            // Arrange
            _dynamicRepositoryMock.Setup(r => r.ExecuteAsync(It.IsAny<WriteLogSPTransferAtDBLOGCommand>()))
                .ThrowsAsync(new System.Exception("DBLOG failed"));
            _dynamicRepositoryMock.Setup(r => r.ExecuteAsync(It.IsAny<WriteLogSPTransferCommand>()))
                .ThrowsAsync(new System.Exception("SP failed"));

            // Act
            await _service.WriteLogSPTransferAsync("logdata", 5);

            // Assert
            _dynamicRepositoryMock.Verify(r => r.ExecuteAsync(It.IsAny<WriteLogSPTransferAtDBLOGCommand>()), Times.Once);
            _dynamicRepositoryMock.Verify(r => r.ExecuteAsync(It.IsAny<WriteLogSPTransferCommand>()), Times.Once);
            _loggerMock.Verify(l => l.ErrorAsync(It.Is<string>(s => s.Contains("DBLOG failed"))), Times.Once);
            _loggerMock.Verify(l => l.ErrorAsync(It.Is<string>(s => s.Contains("SP failed"))), Times.Once);
        }
    }
}
