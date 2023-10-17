using Fanex.Customer.API.Shared.Services;

namespace Fanex.Customer.API.Modules.Message.Models;

public class SBCommissionViolationSubMessage : SubMessage
{
    private const int SBGroupId = 1;

    private const int SBCommGroupType = 150;

    public override string ToString(ICacheRefService cacheRefService, string language)
        => cacheRefService.GetSportsbookCommSMAName(SBGroupId, SBCommGroupType, SportType, BetType, language);
}