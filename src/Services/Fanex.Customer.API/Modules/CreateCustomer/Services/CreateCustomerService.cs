using Fanex.Customer.API.Modules.CreateCustomer.Entities;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.SharedServices;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Shared.Services;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using System.Collections;
using Fanex.Data;
using Fanex.Customer.API.Modules.Message;
using System.Xml;
using Newtonsoft.Json;
using Fanex.Customer.API.Shared.Utils;

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
        using IObjectDb db = ObjectDbFactory.CreateInstance(GetCreateNewSPName(request.Settings.GeneralInfo.UplineRoleId));
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Context.Language);
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
        var parameters = new Hashtable {
            { "firstname", request.Settings.GeneralInfo.FirstName ?? string.Empty },
            { "lastname", request.Settings.GeneralInfo.LastName ?? string.Empty},
            { "creator", request.Context.CreatorId },
            { "username", request.Settings.GeneralInfo.UserName },
            { "userpwd", request.Settings.GeneralInfo.Password },
            { "phone", request.Settings.GeneralInfo.Phone ?? string.Empty},
            { "mobilephone", request.Settings.GeneralInfo.MobilePhone ?? string.Empty},
            { "fax", request.Settings.GeneralInfo.Fax ?? string.Empty},
            { "transferoption", request.Settings.GeneralInfo.TransferOption },
            { "templatename", request.Settings.GeneralInfo.TemplateName },
            { "language", LanguageUtils.ToLanguageKey(request.Context.Language) },
            { "subaccid", request.Context.SubAccId },
            { "credit", request.Settings.GeneralInfo.MaxCredit },
            { "systemid", request.Context.SystemId },
            { "ip", request.Context.IP },
            { "creationstatus", appSettings.CreationStatus },
            { "settingtypeall", (request.Context.UIBehavior != null) ? ObjectToXml(request.Context.UIBehavior) : string.Empty },
        };

        if (request.Settings.GeneralInfo.UplineRoleId == (int)CustomerLevel.Super)
        {
            parameters.Add("disableautompt", request.Settings.GeneralInfo.DisableAutoMPT);
            parameters.Add("site", request.Settings.GeneralInfo.Site);
            parameters.Add("srecommend", request.Context.CreatorId);
        }
        else if (request.Settings.GeneralInfo.UplineRoleId == (int)CustomerLevel.Master)
        {
            parameters.Add("maxcredit", request.Settings.GeneralInfo.MemberMaxCredit);
            parameters.Add("mrecommend", request.Context.CreatorId);
        }

        return parameters;
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

    private static string ObjectToXml(object o)
    {
        Fanex.Common.Helper.Check.Argument.IsNotNull(o, "o");
        return  ((XmlDocument)JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(o)))?.OuterXml;
    }

    private static string GetCreateNewSPName(int roleId)
        => roleId switch
        {
            ((int)CustomerLevel.Super) =>"Acc_LiteMode_CustomerInsMember_SAg",
            ((int)CustomerLevel.Master) =>"Acc_LiteMode_CustomerInsMember_MAg",
            ((int)CustomerLevel.Agent) =>"Acc_LiteMode_CustSetting_MasterAllInfoSel",
            _ => throw new IndexOutOfRangeException($"RoleId cannot = {roleId}")
        };
}