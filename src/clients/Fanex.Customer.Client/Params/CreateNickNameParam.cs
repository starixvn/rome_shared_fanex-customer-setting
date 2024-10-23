using Fanex.Customer.Client.Constants;

namespace Fanex.Customer.Client.Params
{
    public class CreateNickNameParam
    {
        public CreateNickNameParam(
           int custId,
           int subAccId,
           string nickName,
           string ip,
           string site,
           int custDBGroupID,
           bool isEnableUpdateNickname,
           string language = Languages.English)
        {
            CustId = custId;
            NickName = nickName;
            Language = language;
            SubAccId = subAccId;
            IP = ip;
            CustDBGroupID = custDBGroupID;
            Site = site;
            IsEnableUpdateNickname = isEnableUpdateNickname;
        }

        public int CustId { get; }
        public string NickName { get; }
        public string Language { get; }
        public int SubAccId { get; }
        public string IP { get; }
        public int CustDBGroupID { get; }
        public string Site { get; }
        public bool IsEnableUpdateNickname { get; }
    }
}
