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
using Fanex.Customer.API.Shared.Constants;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;
using Fanex.Customer.API.Modules.CreateCustomer.Resources;
using System.Collections;
using System.Linq;
using Fanex.Data;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.Message.Models;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface ICreateCustomerService
{
    Task<CreateCustomerResult> Create(CreateRequest request);
}

public class CreateCustomerService : BaseCustomerService, ICreateCustomerService
{
    public CreateCustomerService(
        IDynamicRepository dynamicRepository, 
        IOptions<AppSettings> appSettings, 
        ISharedService sharedService, 
        ICacheRefService cacheRefService,
        IMessageService messageService)
        : base(dynamicRepository, appSettings, sharedService, cacheRefService, messageService)
    {
    }

    public async Task<CreateCustomerResult> Create(CreateRequest request)
    {
        var parameters = await this.GetCreateCustomerParams(request, appSettings);

        using IObjectDb db = ObjectDbFactory.CreateInstance("Acc_LiteMode_CustomerInsMember_SAg");
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), SettingLevel.SportTypeBetType, request.Context.Language);
        return new CreateCustomerResult(Convert.ToInt32(outputParams["errcode"]), msg.MainMessage, msg.SubMessages);
    }

    private async Task<Hashtable> GetCreateCustomerParams(CreateRequest request, AppSettings appSettings)
    {
        var param = CreateGeneralInfoParams(request, appSettings);

        var settingProfiles = await sharedService.GetSettingProfiles();

        var settingTypesXml = CreateSettingTypesXml(request, settingProfiles);

        foreach (var item in settingTypesXml)
        {
            param.Add($"SettingType{item.Key}", $"<Root>{item.Value}</Root>");
        }

        return param;
    }

    private static Hashtable CreateGeneralInfoParams(CreateRequest request, AppSettings appSettings)
    {
        return new Hashtable {
            { "firstname", request.Settings.GeneralInfo.FirstName },
            { "lastname", request.Settings.GeneralInfo.LastName },
            { "creator", request.Context.CreatorId },
            { "username", request.Settings.GeneralInfo.UserName },
            { "userpwd", request.Settings.GeneralInfo.Password },
            { "phone", request.Settings.GeneralInfo.Phone },
            { "mobilephone", request.Settings.GeneralInfo.MobilePhone },
            { "fax", request.Settings.GeneralInfo.Fax },
            { "transferoption", request.Settings.GeneralInfo.TransferOption },
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
    }

    private static IEnumerable<KeyValuePair<int, string>> CreateSettingTypesXml(CreateRequest request, IEnumerable<SettingProfile> settingProfiles)
    {
        var groupItems = request.Settings.ProductGroups
        .SelectMany(p => p.GroupTypes.SelectMany(gi => gi.GroupItems.Select(i => new { GroupId = p.Id, GroupTypeId = gi.Id, GroupItem = i })));

        var joinedItems = groupItems.Join(
            settingProfiles,
            groupItem => new { groupItem.GroupId, groupItem.GroupTypeId, groupItem.GroupItem.SportTypeId, groupItem.GroupItem.BetTypeId, groupItem.GroupItem.IsLive },
            settingProfile => new { settingProfile.GroupId, GroupTypeId = settingProfile.GroupType, SportTypeId = settingProfile.SportType, BetTypeId = settingProfile.BetType, settingProfile.IsLive },
            (groupItem, settingProfile) =>
            {
                return new
                {
                    settingProfile.SettingType,
                    groupItem.GroupItem
                };
            }
        );

        return joinedItems.GroupBy(g => g.SettingType)
        .Select(t => new KeyValuePair<int, string>(t.Key, string.Join("", t.Select(i => i.GroupItem.ToXml()))));
    }
}