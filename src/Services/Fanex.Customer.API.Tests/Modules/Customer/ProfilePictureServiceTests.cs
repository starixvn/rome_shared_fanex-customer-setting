using System;
using System.Threading.Tasks;
using Fanex.Customer.API.Modules.Customer;
using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Customer.API.Modules.Customer.Requests;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data.Repository;
using Moq;
using Xunit;

namespace Fanex.Customer.API.Tests.Modules.Customer
{
    public class ProfilePictureServiceTests
    {
        private readonly Mock<IDynamicRepository> _dynamicRepositoryMock;
        private readonly ProfilePictureService _service;

        public ProfilePictureServiceTests()
        {
            _dynamicRepositoryMock = new Mock<IDynamicRepository>();
            _service = new ProfilePictureService(_dynamicRepositoryMock.Object);
        }

        [Fact]
        public async Task GetActorProfilePicture_ReturnsModel_WhenFound()
        {
            var expected = new ActorProfilePictureModel();
            _dynamicRepositoryMock.Setup(r => r.GetAsync<ActorProfilePictureModel>(It.IsAny<ICriteria>()))
                .ReturnsAsync(expected);

            var result = await _service.GetActorProfilePicture(1);

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GetActorProfilePicture_ReturnsNewModel_WhenNull()
        {
            _dynamicRepositoryMock.Setup(r => r.GetAsync<ActorProfilePictureModel>(It.IsAny<ICriteria>()))
                .ReturnsAsync((ActorProfilePictureModel)null);

            var result = await _service.GetActorProfilePicture(1);

            Assert.NotNull(result);
            Assert.IsType<ActorProfilePictureModel>(result);
        }

        [Fact]
        public async Task UpdateActorProfilePicture_ExecutesCommandAndReturnsResult()
        {
            var param = new UpdateActorProfilePictureParam { ActorId = 1, IsMBCSite = true, IP = "127.0.0.1" };
            _dynamicRepositoryMock.Setup(r => r.ExecuteAsync(It.IsAny<INonQueryCommand>()))
                .Returns(Task.FromResult(0));

            var result = await _service.UpdateActorProfilePicture(param);

            Assert.NotNull(result);
        }
    }
}
