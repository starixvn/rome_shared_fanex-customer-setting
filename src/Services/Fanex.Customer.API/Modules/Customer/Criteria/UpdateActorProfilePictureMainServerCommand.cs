namespace Fanex.Customer.API.Modules.Customer.Criteria
{
    public class UpdateActorProfilePictureMainServerCommand : UpdateActorProfilePictureCommandBase
    {
        public UpdateActorProfilePictureMainServerCommand(int actorId, int mode, string ip)
            : base(actorId, mode, ip, true)
        {
        }

        public override string GetSettingKey() => "Age_CustomerUpdProfilePicture";
    }
}