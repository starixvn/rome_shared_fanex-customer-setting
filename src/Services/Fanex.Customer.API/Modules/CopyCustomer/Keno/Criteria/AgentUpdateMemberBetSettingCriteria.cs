using Fanex.Customer.API.Modules.CopyCustomer.Keno.Models;
using Fanex.Customer.API.Modules.CopyCustomer.Models;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CopyCustomer.Keno.Criteria;

public class AgentUpdateMemberBetSettingCriteria : CriteriaBase
{
    private const int sportType = 202;

    public AgentUpdateMemberBetSettingCriteria(CustomerInformation customerInformation, BetSetting betSetting, bool cloneSetting, byte systemId)
    {
        IsSC = cloneSetting;
        CustId = customerInformation.CustId;
        SportType = sportType;
        BetType = betSetting.BetTypeId;
        UpdateStatus = betSetting.IsStatusUpdated;
        Disable = !betSetting.IsEnabled;
        UpdateMinMaxBet = betSetting.IsBetSettingUpdated;
        MinBet = betSetting.MinBet;
        MaxBet = betSetting.MaxBet;
        MaxPerMatch = betSetting.MaxPerMatch;
        UserId = customerInformation.UserId;
        SubAccId = customerInformation.SubAccId;
        Ip = customerInformation.Ip;
        Language = customerInformation.Language;
        SystemId = systemId;
    }

    public bool IsSC { get; } = false;

    public int CustId { get; }

    public int SportType { get; }

    public int BetType { get; }

    public bool UpdateStatus { get; }

    public bool Disable { get; }

    public bool UpdateMinMaxBet { get; }

    public double MinBet { get; }

    public double MaxBet { get; }

    public double MaxPerMatch { get; }

    public int UserId { get; }

    public int SubAccId { get; }

    public string Ip { get; }

    public string Language { get; }

    public int SystemId { get; }

    public string ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "Acc_CustomerUpdMemberKeno_BetSetting";

    public override bool IsValid() => CustId > 0 && SportType > 0 && BetType > 0;
}