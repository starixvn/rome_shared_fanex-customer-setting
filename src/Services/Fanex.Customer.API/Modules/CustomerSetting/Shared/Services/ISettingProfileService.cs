using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using static Fanex.Customer.API.Modules.CustomerSetting.Shared.Services.SettingProfileService;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.Services;

public interface ISettingProfileService
{
    Task<IEnumerable<SettingProfile>> GetSettingProfiles(
        byte systemId,
        string site,
        int currency,
        ProfileMode mode,
        ServiceType serviceType = ServiceType.Credit);

    Task<IEnumerable<int>> GetAllSettingTypes(
        byte systemId,
        string site,
        int currency,
        ProfileMode mode,
        ServiceType serviceType = ServiceType.Credit);

    Task<int?> GetSettingTypeByGroupType(
        byte systemId,
        string site,
        int currency,
        int groupTypeId,
        ProfileMode mode,
        ServiceType serviceType = ServiceType.Credit);
}
