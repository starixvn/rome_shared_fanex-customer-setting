using Fanex.Caching;
using Fanex.Customer.API.Modules.CreateCustomer.Criterias;
using Fanex.Customer.API.Modules.CreateCustomer.Entities;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.SharedServices;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Shared.Services;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType150;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType151;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType152;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType155;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType156;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType157;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType159;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;
using Fanex.Customer.API.Modules.CreateCustomer.Resources;
using System.Collections;
using System.Linq;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface ICreateCustomerService
{
    Task<Hashtable> Create(CreateRequest request);
}

public class CreateCustomerService : BaseCustomerService, ICreateCustomerService
{
    private Dictionary<int, string> SettingTypesXml;

    public CreateCustomerService(IDynamicRepository dynamicRepository, IOptions<AppSettings> appSettings, ISharedService sharedService, ICacheRefService cacheRefService)
        : base(dynamicRepository, appSettings, sharedService, cacheRefService)
    {
    }

    public async Task<Hashtable> Create(CreateRequest request)
    {
        var parameters = await this.GetCreateCustomerParams(request, appSettings);

        return parameters;
    }

    private async Task<Hashtable> GetCreateCustomerParams(CreateRequest request, AppSettings appSettings)
    {
        var param = new Hashtable {
            { "firstname", request.Settings.GeneralInfo.FirstName },
            { "lastname", request.Settings.GeneralInfo.LastName },
            { "creator", request.Context.CreatorId },
            { "username", request.Settings.GeneralInfo.UserName },
            { "userpwd", request.Settings.GeneralInfo.Password },
            { "phone", request.Settings.GeneralInfo.Phone },
            { "mobilephone", request.Settings.GeneralInfo.MobilePhone },
            { "fax", request.Settings.GeneralInfo.Fax },
            { "disableautompt", request.Settings.GeneralInfo.AllowAutoPT },
            { "language", request.Context.Language },
            { "subaccid", request.Context.SubAccId },
            { "credit", request.Settings.GeneralInfo.MaxCredit },
            { "srecommend", request.Context.CreatorId },
            { "systemid", request.Context.SystemId },
            { "ip", request.Context.IP },
            { "site", request.Settings.GeneralInfo.Site },
            { "creationstatus", appSettings.CreationStatus }
        };

        var SettingTypesXml = request.Settings.ProductGroups
            .SelectMany(p => p.GroupTypes.SelectMany(gi => gi.GroupItems.Select(i => new { GroupId = p.Id, GroupTypeId = gi.Id, GroupItem = i })))
            .Join(
               await sharedService.GetSettingProfiles(),
               groupItems => new { groupItems.GroupId, groupItems.GroupTypeId, groupItems.GroupItem.SportTypeId, groupItems.GroupItem.BetTypeId },
               settingProfile => new { settingProfile.GroupId, GroupTypeId = settingProfile.GroupType, SportTypeId = settingProfile.SportType, BetTypeId = settingProfile.BetType },
               (groupItems, settingProfile) =>
               {
                   return new
                   {
                       settingProfile.SettingType,
                       groupItems.GroupItem
                   };
               }
            )
            .GroupBy(g => g.SettingType)
            .Select(t=>new {t.Key, xlm= string.Join("", t.Select(i => i.GroupItem.ToXml()))})
            .Select(i => {
                param.Add($"SettingType{i.Key}", i.xlm);
                return i;
            });
            
        return param;
    }


}