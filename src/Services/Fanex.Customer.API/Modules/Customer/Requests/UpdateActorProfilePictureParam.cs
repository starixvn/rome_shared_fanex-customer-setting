namespace Fanex.Customer.API.Modules.Customer.Requests
{
    public class UpdateActorProfilePictureParam
    {
        public int ActorId { get; set; }

        public string IP { get; set; }

        public bool IsMBCSite { get; set; }
    }
}