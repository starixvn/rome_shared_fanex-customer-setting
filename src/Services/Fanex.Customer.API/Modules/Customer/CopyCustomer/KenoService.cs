using System.Globalization;
using Fanex.RemotingRef;
using Fanex.Customer.API.Shared.Models;

namespace Fanex.Customer.API.Modules.Customer.CopyCustomer;

public interface IKenoService
{
    IEnumerable<Result> UpdateMemberSettings(
        int custId,
        int sourceCustId,
        CopyCustomerRequest request);
}

public class KenoService : IKenoService
{
    private readonly Logging.ILogger logger;
    private readonly KenoConfiguration configuration;
    private readonly IKenoRepository kenoRepository;
    private readonly IRemotingRefService remotingRefService;

    public KenoService(
        Logging.ILogger logger,
        KenoConfiguration configuration,
        IKenoRepository kenoRepository,
        IRemotingRefService remotingRefService)
    {
        this.logger = logger;
        this.configuration = configuration;
        this.kenoRepository = kenoRepository;
        this.remotingRefService = remotingRefService;
    }

    public IEnumerable<Result> UpdateMemberSettings(int custId, int sourceCustId, CopyCustomerRequest request)
    {
        if (configuration.ShowSetting)
        {
            try
            {
                var sourceSettings = GetSourceMemberSettings(sourceCustId, request.ActorId, request.ActorSubAccId);
                sourceSettings.SystemId = request.SystemId;
                sourceSettings.CustomerInformation.CustId = custId;
                sourceSettings.CustomerInformation.Ip = request.IP;

                return UpdateMemberSettings(sourceSettings);
            }
            catch (Exception ex)
            {
                logger.Warn(ex.ToString());
            }
        }

        return Enumerable.Empty<Result>();
    }

    private AgentEditMemberSettingDto GetSourceMemberSettings(int custId, int userId, int subAccountId)
        => GetSettingInCaseOfAgentEditMember(custId, userId, subAccountId);

    private IEnumerable<Result> UpdateMemberSettings(AgentEditMemberSettingDto sourceSettings)
    {
        foreach (var betSetting in sourceSettings.BetSettings)
        {
            betSetting.IsStatusUpdated = true;
            betSetting.IsBetSettingUpdated = true;
        }

        foreach (var positionTaking in sourceSettings.PositionTakings)
        {
            positionTaking.A1 = positionTaking.A1 < positionTaking.A2 ? positionTaking.A1 : positionTaking.A2;
            positionTaking.IsUpdateA1 = true;
        }

        return UpdateMemberCustomerSettingInCaseOfAgentEditMember(sourceSettings, true);
    }

    private AgentEditMemberSettingDto GetSettingInCaseOfAgentEditMember(int custId, int userId, int subAccountId)
    {
        var memberSetting = kenoRepository.GetSettingInCaseOfAgentEditMember(new AgentGetMemberSettingCriteria(custId));
        memberSetting.AdjustInformationWhenLoadSetting(custId, userId, subAccountId);
        UpdateBetTypeNameForPositionTaking(memberSetting);
        UpdateBetTypeNameForBetSetting(memberSetting);

        return memberSetting;
    }

    internal void UpdateBetTypeNameForPositionTaking(AgentEditMemberSettingDto memberSetting)
    {
        if (memberSetting?.PositionTakings != null && memberSetting.PositionTakings.Any())
        {
            foreach (var positionTaking in memberSetting.PositionTakings)
            {
                positionTaking.BetTypeName = GetBettype(positionTaking.BetTypeId);
            }
        }
    }

    internal void UpdateBetTypeNameForBetSetting(AgentEditMemberSettingDto memberSetting)
    {
        if (memberSetting?.BetSettings != null && memberSetting.BetSettings.Any())
        {
            foreach (var betSetting in memberSetting.BetSettings)
            {
                betSetting.BetTypeName = GetBettype(betSetting.BetTypeId);
            }
        }
    }

    private IList<Result> UpdateMemberCustomerSettingInCaseOfAgentEditMember(AgentEditMemberSettingDto memberSetting, bool cloneSetting = false)
    {
        return UpdateCustomerSetting(memberSetting, cloneSetting, memberSetting.SystemId.Value);
    }

    internal List<Result> UpdateCustomerSetting(AgentEditMemberSettingDto customerSetting, bool cloneSetting, byte systemId)
    {
        var checkUmResult = kenoRepository.CheckUM(customerSetting.CustomerInformation.CustId);
        if (!checkUmResult.IsSuccessful)
        {
            return new List<Result> { checkUmResult };
        }

        customerSetting.AdjustInformationWhenUpdateSetting();
        var results = new List<Result>();
        try
        {
            results.AddRange(UpdateBetSettings(customerSetting, cloneSetting, systemId));
            results.AddRange(UpdatePositionTakings(customerSetting, cloneSetting));
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message, ex);

            results.Add(new Result(100, ex.Message));
        }

        return results;
    }

    internal IList<Result> UpdateBetSettings(AgentEditMemberSettingDto customerSetting, bool cloneSetting, byte systemId)
    {
        var results = new List<Result>();

        foreach (var betSetting in customerSetting.BetSettings.Where(bs => bs.IsDataChanged()))
        {
            results.AddRange(UpdateBetSetting(customerSetting.CustomerInformation, betSetting, cloneSetting, systemId));
        }

        return results;
    }

    internal IList<Result> UpdatePositionTakings(AgentEditMemberSettingDto customerSetting, bool cloneSetting)
    {
        var results = new List<Result>();

        foreach (var positionTaking in customerSetting.PositionTakings.Where(pt => pt.IsDataChanged()))
        {
            results.AddRange(UpdatePositionTaking(customerSetting.CustomerInformation, positionTaking, cloneSetting));
        }

        return results;
    }

    internal IList<Result> UpdatePositionTaking(CustomerInformation customerInformation, AgentEditMemberPositionTaking positionTaking, bool cloneSetting)
    {
        var criteria = new AgentUpdateMemberPositionTakingCriteria(customerInformation, positionTaking, cloneSetting);
        var results = kenoRepository.UpdatePositionTaking(criteria);

        foreach (var result in results)
        {
            result.Message = ReformatErrorMessageForPositionTaking(positionTaking.BetTypeId, result.Message);
        }

        return results;
    }

    public IList<Result> UpdateBetSetting(CustomerInformation customerInformation, BetSetting betSetting, bool cloneSetting, byte systemId)
    {
        var criteria = new AgentUpdateMemberBetSettingCriteria(customerInformation, betSetting, cloneSetting, systemId);
        if (criteria.IsInvalid())
        {
            return new List<Result>();
        }

        var results = kenoRepository.UpdateBetSetting(criteria);

        foreach (var result in results)
        {
            result.Message = ReformatErrorMessageForBetSetting(betSetting.BetTypeId, result.Message);
        }

        return new List<Result>();
    }

    internal string ReformatErrorMessageForBetSetting(int betTypeId, string inputMessage)
    {
        string betTypeName = GetBettype(betTypeId);
        return string.Format(
            "{0} - {1} - {2}",
            Resources.Messages.BetSetting,
            !string.IsNullOrEmpty(betTypeName) ? betTypeName : betTypeId.ToString(),
            inputMessage);
    }

    internal string ReformatErrorMessageForPositionTaking(int betTypeId, string inputMessage)
    {
        string betTypeName = GetBettype(betTypeId);
        return string.Format(
            "{0} - {1} - {2}",
            Resources.Messages.PositionTaking,
            !string.IsNullOrEmpty(betTypeName) ? betTypeName : betTypeId.ToString(),
            inputMessage);
    }

    internal string GetBettype(int betTypeId)
    {
        return remotingRefService.GetBetTypeName(betTypeId.ToString(), CultureInfo.CurrentCulture.Name, string.Empty, string.Empty);
    }
}
