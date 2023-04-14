using System.Data;
using System.Globalization;
using Fanex.Customer.API.Extensions;
using Fanex.Customer.API.Modules.CopyCustomer.Keno.Criteria;
using Fanex.Customer.API.Modules.CopyCustomer.Keno.Models;
using Fanex.Customer.API.Modules.CopyCustomer.Models;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.RemotingRef;

namespace Fanex.Customer.API.Modules.CopyCustomer.Keno;

public interface IKenoRepository
{
    AgentEditMemberSettingDto GetSettingInCaseOfAgentEditMember(AgentGetMemberSettingCriteria criteria);

    IList<Result> UpdateBetSetting(AgentUpdateMemberBetSettingCriteria criteria);

    IList<Result> UpdatePositionTaking(AgentUpdateMemberPositionTakingCriteria criteria);

    Result CheckUM(int custId);
}

public class KenoRepository : IKenoRepository
{
    private readonly KenoConfiguration kenoConfiguration;
    private readonly IMessageHelper messageHelper;
    private readonly IRemotingRefService remotingRefService;

    public KenoRepository(KenoConfiguration kenoConfiguration, IMessageHelper messageHelper, IRemotingRefService remotingRefService)
    {
        this.kenoConfiguration = kenoConfiguration;
        this.messageHelper = messageHelper;
        this.remotingRefService = remotingRefService;
    }

    public AgentEditMemberSettingDto GetSettingInCaseOfAgentEditMember(AgentGetMemberSettingCriteria criteria)
    {
        var memberSetting = new AgentEditMemberSettingDto();

        if (criteria.IsValid())
        {
            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            using var reader = db.ExecuteReader(criteria);
            if (reader != null)
            {
                memberSetting.CustomerInformation = ReadCustomerInformationData(reader, UserLevel.Member.Value, ActionType.AgentEditMember);
                memberSetting.BetSettings = reader.NextResult() ? ReadBetLimitData(reader) : null;
                memberSetting.PositionTakings = reader.NextResult() ? ReadMemberPositionTakingInCaseOfAgentEditMember(reader) : null;
            }
        }

        return memberSetting;
    }

    public IList<Result> UpdateBetSetting(AgentUpdateMemberBetSettingCriteria criteria)
    {
        return ExecuteUpdateSetting(criteria);
    }

    public IList<Result> UpdatePositionTaking(AgentUpdateMemberPositionTakingCriteria criteria)
    {
        return ExecuteUpdateSetting(criteria);
    }

    internal static CustomerInformation ReadCustomerInformationData(IDataReader dataReader, byte role, ActionType actionType)
    {
        CustomerInformation customerInformation = null;
        while (dataReader.Read())
        {
            customerInformation = new CustomerInformation
            {
                CustId = dataReader.HasColumn("custid") ? dataReader.ToInt("custid") : 0,
                Username = dataReader.HasColumn("username") ? dataReader.ToString("username") : string.Empty,
                FirstName = dataReader.HasColumn("firstname") ? dataReader.ToString("firstname") : string.Empty,
                LastName = dataReader.HasColumn("lastname") ? dataReader.ToString("lastname") : string.Empty,
                Phone = dataReader.HasColumn("phone") ? dataReader.ToString("phone") : string.Empty,
                MobilePhone = dataReader.HasColumn("mobilephone") ? dataReader.ToString("mobilephone") : string.Empty,
                Fax = dataReader.HasColumn("fax") ? dataReader.ToString("fax") : string.Empty,
                IsDisableAutoMPT = dataReader.HasColumn("DisableAutoMPT") ? dataReader.ToBool("DisableAutoMPT") : false,
                RoleId = role,
                ActionType = (int)actionType
            };
        }

        return customerInformation;
    }

    internal IList<BetSetting> ReadBetLimitData(IDataReader dataReader)
    {
        var betSettings = new List<BetSetting>();
        while (dataReader.Read())
        {
            var bettingSetting = new BetSetting
            {
                BetTypeId = dataReader.HasColumn("IBCBetTypeID") ? dataReader.ToInt("IBCBetTypeID") : 0,
                MinBet = dataReader.HasColumn("MinBet") ? dataReader.ToDouble("MinBet") : 0,
                MaxBet = dataReader.HasColumn("MaxBet") ? dataReader.ToDouble("MaxBet") : 0,
                MaxPerMatch = dataReader.HasColumn("MaxPerGameType") ? dataReader.ToDouble("MaxPerGameType") : 0,
                IsEnabled = dataReader.HasColumn("IsDisabled") ? !dataReader.ToBool("IsDisabled") : false,
                Upline = new BetSetting()
                {
                    MinBet = dataReader.HasColumn("MinBet_Min") ? dataReader.ToDouble("MinBet_Min") : 0,
                    MaxBet = dataReader.HasColumn("MaxBet_Max") ? dataReader.ToDouble("MaxBet_Max") : 0,
                    MaxPerMatch = dataReader.HasColumn("MaxPerMatch_Max") ? dataReader.ToDouble("MaxPerMatch_Max") : 0,
                    IsEnabled = dataReader.HasColumn("DisableUplinesKeno") ? !dataReader.ToBool("DisableUplinesKeno") : true
                }
            };

            if (!kenoConfiguration.ExcludeBetTypes.Contains(bettingSetting.BetTypeId))
            {
                betSettings.Add(bettingSetting);
            }
        }

        return betSettings;
    }

    internal IList<AgentEditMemberPositionTaking> ReadMemberPositionTakingInCaseOfAgentEditMember(IDataReader dataReader)
    {
        var positionTakings = new List<AgentEditMemberPositionTaking>();
        while (dataReader.Read())
        {
            var bettingSetting = new AgentEditMemberPositionTaking
            {
                BetTypeId = dataReader.ToInt("BetType"),
                A1 = dataReader.ToDouble("a1"),
                N2 = dataReader.ToDouble("a1_min"),
                A2 = dataReader.ToDouble("a1_max"),
                Comm = dataReader.ToDouble("playerdiscount"),
                AComm = dataReader.ToDouble("playerdiscount_max")
            };

            if (!kenoConfiguration.ExcludeSettingBetTypes.Contains(bettingSetting.BetTypeId)
                && !kenoConfiguration.ExcludeBetTypes.Contains(bettingSetting.BetTypeId))
            {
                positionTakings.Add(bettingSetting);
            }
        }

        return positionTakings;
    }

    internal virtual IList<Result> ExecuteUpdateSetting(ICriteria criteria)
    {
        using var objDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        objDb.ExecuteNonQuery(criteria);
        var outputParams = objDb.GetOutputParams();
        messageHelper.SetErrorCodeAndErrorMessage(Convert.ToString(outputParams["errcode"]), Convert.ToString(outputParams["errmsg"]));
        return messageHelper.GetErrors();
    }

    public Result CheckUM(int custId)
    {
        var criteria = new CheckUMCriteria(custId, ProductIds.RNGKeno, "E");

        if (!criteria.IsValid())
        {
            return new Result(500, $"Invalid request custId: {custId}");
        }

        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        db.ExecuteNonQuery(criteria);
        var outputParams = db.GetOutputParams();
        var errMsg = outputParams["errmsg"].ToString();
        var formatedErrMsg = string.Format(errMsg,
            remotingRefService.GetResourceDataByRefIdAndResourceId(
                "Product",
                nameof(ProductIds.RNGKeno),
                CultureInfo.CurrentCulture.Name,
                nameof(ProductIds.RNGKeno)));

        return new Result(Convert.ToInt32(outputParams["errcode"]), formatedErrMsg);
    }
}