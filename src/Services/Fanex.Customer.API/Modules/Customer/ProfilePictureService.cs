using Fanex.Customer.API.Modules.Customer.Criteria;
using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Customer.API.Modules.Customer.Requests;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer
{
    public interface IProfilePictureService
    {
        Task<ActorProfilePictureModel> GetActorProfilePicture(int custId);

        Task<Result> UpdateActorProfilePicture(UpdateActorProfilePictureParam param);
    }

    public class ProfilePictureService : IProfilePictureService
    {
        private readonly IDynamicRepository dynamicRepository;

        public ProfilePictureService(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<ActorProfilePictureModel> GetActorProfilePicture(int custId)
        {
            var model = await dynamicRepository.GetAsync<ActorProfilePictureModel>(new SelectActorProfilePictureCriteria(custId));
            return model ?? new ActorProfilePictureModel();
        }

        public async Task<Result> UpdateActorProfilePicture(UpdateActorProfilePictureParam param)
        {
            var command = new UpdateActorProfilePictureMainServerCommand(
                param.ActorId, param.IsMBCSite ? 1 : 2, param.IP);

            await dynamicRepository.ExecuteAsync(command);

            return Result.Create(command.ErrMsg, command.ErrCode);
        }
    }
}