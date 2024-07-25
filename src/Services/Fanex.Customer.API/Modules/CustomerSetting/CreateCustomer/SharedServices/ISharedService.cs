using Fanex.Customer.API.Modules.CreateCustomer.Entities;
using static Fanex.Customer.API.Modules.CreateCustomer.SharedServices.SharedService;

namespace Fanex.Customer.API.Modules.CreateCustomer.SharedServices;

public interface ISharedService
{
    Task<IEnumerable<SettingProfile>> GetSettingProfiles(byte systemId, string site, int currency, ProfileMode mode);

    Task<IEnumerable<int>> GetAllSettingTypes(byte systemId, string site, int currency, ProfileMode mode);

    Task<int?> GetSettingTypeByGroupType(byte systemId, string site, int currency, int groupTypeId, ProfileMode mode);
}
