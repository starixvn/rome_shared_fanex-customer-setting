using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Customer.API.Modules.Customer;
using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Customer.API.Modules.Customer.Requests.CashService;
using Fanex.Customer.API.Shared.Models;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Fanex.Logging;
using Moq;
using Xunit;
using Fanex.Customer.API.Modules.Customer.Criteria.CashService;

namespace Fanex.Customer.API.Modules.Customer.Tests
{
    public class CashCustomerServiceTests
    {
        private readonly Mock<IDynamicRepository> _dynamicRepositoryMock = new();
        private readonly Mock<ISiteDBService> _siteDBServiceMock = new();
        private readonly Mock<IWriteLogSPService> _writeLogSPServiceMock = new();
        private readonly Mock<IWriteLogSPTransferService> _writeLogSPTransferServiceMock = new();
        private readonly Mock<IWriteLogSPCashService> _writeLogSPCashServiceMock = new();
        private readonly Mock<ILogger> _loggerMock = new();

        private CashCustomerService CreateService() => new(
            _dynamicRepositoryMock.Object,
            _siteDBServiceMock.Object,
            _writeLogSPServiceMock.Object,
            _writeLogSPTransferServiceMock.Object,
            _writeLogSPCashServiceMock.Object,
            _loggerMock.Object
        );

        [Fact]
        public async Task UpdateEnableCashService_NullRequest_ReturnsError()
        {
            var service = CreateService();
            var result = await service.UpdateEnableCashService(null);
            Assert.False(result.IsSuccessful);
            Assert.Equal("Request cannot be null.", result.Message);
        }

        [Fact]
        public async Task UpdateEnableCashService_InvalidCustId_ReturnsError()
        {
            var service = CreateService();
            var request = new UpdateCashServiceStatusRequest { CustId = 0, Site = "site", ServiceType = 1 };
            var result = await service.UpdateEnableCashService(request);
            Assert.False(result.IsSuccessful);
            Assert.Equal("Invalid CustId.", result.Message);
        }

        [Fact]
        public async Task UpdateEnableCashService_ValidRequest_Success()
        {
            var request = new UpdateCashServiceStatusRequest
            {
                CustId = 1,
                ServiceType = 1,
                IP = "127.0.0.1",
                Language = "en",
                Site = "site1"
            };
            var siteDBConfig = new SiteDBConfig { SiteDBGroupID = 5 };
            _siteDBServiceMock.Setup(s => s.GetSiteDBConfigByCustId(1)).ReturnsAsync(siteDBConfig);
            _dynamicRepositoryMock.Setup(d => d.FetchAsync<LogDataEntity>(It.IsAny<UpdateCashServiceStatusCommand>())).ReturnsAsync(new List<LogDataEntity>());
            _dynamicRepositoryMock.Setup(d => d.ExecuteAsync(It.IsAny<INonQueryCommand>())).ReturnsAsync(0);

            var service = CreateService();
            var result = await service.UpdateEnableCashService(request);

            Assert.True(result.IsSuccessful);
            Assert.Equal("Success", result.Message);
        }
    }
}
