using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.CopyCustomer;

public class AgentUpdateMemberPositionTakingCriteria : CriteriaBase
{
    public const int sportType = 202;

    public AgentUpdateMemberPositionTakingCriteria(CustomerInformation customerInformation, AgentEditMemberPositionTaking memberPositionTaking, bool cloneSetting)
    {
        CustId = customerInformation.CustId;
        SportType = sportType;
        BetType = memberPositionTaking.BetTypeId;
        UpdatePT = memberPositionTaking.IsUpdateA1;
        A1 = memberPositionTaking.A1;
        UpdateComm = false;
        Discount = 0;
        SubAccId = customerInformation.SubAccId;
        UserId = customerInformation.UserId;
        Ip = customerInformation.Ip;
        Language = customerInformation.Language;
        IsSearchAndCopy = cloneSetting;
    }

    public int CustId { get; }

    public int SportType { get; }

    public int BetType { get; }

    [SpParam(Ignored = true)]
    public bool UpdatePT { get; }

    public double A1 { get; }

    [SpParam(Ignored = true)]
    public double Discount { get; }

    [SpParam(Ignored = true)]
    public bool UpdateComm { get; }

    public int SubAccId { get; }

    public int UserId { get; }

    public string Ip { get; }

    public string Language { get; }

    public bool IsSearchAndCopy { get; }

    public string ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "Acc_CustomerUpdMemberKeno_Positiontaking";

    public override bool IsValid() => CustId > 0 && SportType > 0 && BetType > 0;
}