namespace Fanex.Customer.API.Modules.Customer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateActorProfilePictureCommandBase : NonQueryCommand
    {
        public UpdateActorProfilePictureCommandBase(int actorId, int mode, string ip, bool isMainServer)
        {
            ActorId = actorId;
            IsDefault = true;
            Mode = mode;
            IP = ip;
            IsMainServer = isMainServer;
        }

        [SpParam(SqlParamName = "CustId")]
        public int ActorId { get; }

        public bool IsDefault { get; }

        public bool IsMainServer { get; }

        public int Mode { get; }

        public string IP { get; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey() =>string.Empty;

        public override bool IsValid() => ActorId > 0;
    }
}