using Fanex.Customer.API.Shared.Services;

namespace Fanex.Customer.API.Modules.Message.Models;

public class BetSettingViolationSubMessage : SubMessage
{
    public override string ToString(ICacheRefService cacheRefService, string language)
        => string.Format($"{cacheRefService.GetSportTypeName(SportType, language)}");
}
