using Fanex.Customer.API.Constants;
using Fanex.Customer.API.Modules.CreateCustomer.EditSettings;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Services;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.EditCustomer.EditGroupTypeServices;
using Fanex.Customer.API.Modules.EditCustomer.GroupType152;
using Fanex.Customer.API.Modules.EditCustomer.GroupType153;
using Fanex.Customer.API.Modules.EditCustomer.GroupType154;
using Fanex.Customer.API.Modules.EditCustomer.GroupType162;
using Fanex.Customer.API.Modules.EditCustomer.Requests;
using Fanex.Customer.API.Modules.EditCustomer.Services.EditGroupTypeServices;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Shared.Extensions;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.EditCustomer.Services;

public interface IEditCustomerService
{
    bool ValidateRequest(EditRequest request);

    bool ValidateMultipleRequest(EditMultipleRequest request);

    Task<UpdateCustomerResult> Update(EditRequest request);

    Task<List<UpdateCustomerResult>> UpdateMultiple(EditMultipleRequest request);
}

public class EditCustomerService : BaseCustomerService, IEditCustomerService
{
    private readonly ISiteDBGroupService siteDBGroupService;

    private IEnumerable<int> groupedPTMemberIds;

    private List<UpdateGroupTypeResult> updateGroupedPTResults;

    private const int PTGroupTypeId = 153;

    protected readonly Dictionary<int, dynamic> groupItemForEditMapping;

    public EditCustomerService(
        ISiteDBGroupService siteDBGroupService,
        IDynamicRepository dynamicRepository,
        IOptions<AppSettings> appSettings,
        ISettingProfileService settingProfileService,
        ICacheRefService cacheRefService,
        IMessageService messageService,
        IUMService uMService,
        ICustomerSettingHelper helperService)
        : base(dynamicRepository, appSettings, settingProfileService, cacheRefService, messageService, uMService, helperService)
    {
        this.siteDBGroupService = siteDBGroupService;
        groupItemForEditMapping = new()
        {
            {150, new GroupType150Service(messageService, cacheRefService)},
            {151, new GroupType151Service(messageService, cacheRefService)},
            {152, new GroupType152Service(messageService, cacheRefService)},
            {153, new GroupType153Service(messageService, cacheRefService)},
            {154, new GroupType154Service(messageService, cacheRefService)},
            {155, new GroupType155Service(messageService, cacheRefService)},
            {156, new GroupType156Service(messageService, cacheRefService)},
            {158, new GroupType158Service(messageService, cacheRefService)},
            {160, new GroupType160Service(messageService, cacheRefService)},
            {162, new GroupType162Service(messageService, cacheRefService)},
        };
    }

    public bool ValidateMultipleRequest(EditMultipleRequest request)
        => Validate(request.Settings.GeneralInfo, request.Settings, request.Context.SiteLabel);

    public bool ValidateRequest(EditRequest request)
        => Validate(request.Settings.GeneralInfo, request.Settings, request.Context.SiteLabel);

    private bool Validate(GeneralInformation generalInformation, CustomerSettingModel settings, string siteLabel)
    {
        if (generalInformation != null)
        {
            if ((!string.IsNullOrWhiteSpace(generalInformation.FirstName) && generalInformation.FirstName.ContainsHTML())
            || (!string.IsNullOrWhiteSpace(generalInformation.LastName) && generalInformation.LastName.ContainsHTML())
            || (!string.IsNullOrWhiteSpace(generalInformation.Phone) && generalInformation.Phone.ContainsHTML())
            || (!string.IsNullOrWhiteSpace(generalInformation.MobilePhone) && generalInformation.MobilePhone.ContainsHTML())
            || (!string.IsNullOrWhiteSpace(generalInformation.Fax) && generalInformation.Fax.ContainsHTML()))
            {
                return false;
            }
        }

        if (!appSettings.IsSupportPT3Decimal(siteLabel))
        {
            foreach (var productGroup in settings.ProductGroups)
            {
                if (productGroup.GroupTypes.Any(gt => gt.Id == 153 && gt.GroupItems.Any(gi => gi.Is3DecimalPTValue())))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public async Task<UpdateCustomerResult> Update(EditRequest request)
    {
        var updatedSportTypes = request.Settings.ProductGroups.SelectMany(g => g.GroupTypes.SelectMany(gt => gt.GroupItems.Select(i => i.SportTypeId)));
        var subResults = new List<UpdateGroupTypeResult>();
        CustomerSettingResult uMResult = uMService.GetUMResult(request.Context.CustId, request.Settings.ProductGroups.FirstOrDefault()?.Id ?? 0, updatedSportTypes, request.Context.Language);

        if (request.Settings.GeneralInfo != null
            && !(request.Context.IsMultipleMode && request.Context.RoleId == CustomerLevel.Super))
        {
            subResults.Add(await UpdateGeneralInfo(request));
        }

        foreach (var productGroup in request.Settings.ProductGroups)
        {
            foreach (var groupType in productGroup.GroupTypes.Where(gt => !gt.IsReadOnly))
            {
                var allowEdit = request.Context.EditFuntion == EditFuntionConstants.EditMultipleSuperWinLimit
                    || !request.Context.IsMultipleMode
                    || request.Context.RoleId != CustomerLevel.Super
                    || groupType.Id == GroupTypes.BetSetting
                    || groupType.Id == GroupTypes.BetSettingWithMaxPerBall
                    || groupType.Id == GroupTypes.ParlayMaxPayout
                    || groupType.Id == GroupTypes.StakeLevel;

                if (!allowEdit) { continue; }

                if (request.Context.RoleId == CustomerLevel.Member &&
                    helperService.IsPTGroupType(groupType.Id)
                    && groupedPTMemberIds != null
                    && groupedPTMemberIds.Any(id => id == request.Context.CustId))
                {
                    foreach (var updateResult in updateGroupedPTResults.Where(rs => rs.ProductGroupId == productGroup.Id && rs.GroupTypeId == groupType.Id))
                    {
                        subResults.Add(updateResult);
                    }
                }
                else
                {
                    var subResult = await UpdateGroupType(request.Context, productGroup.Id, groupType.Id, request.Settings.Group, groupType.GroupItems, appSettings.SettingStatus);
                    if (subResult.ExtraResult != null)
                    {
                        subResults.Add(subResult.ExtraResult);
                    }
                    subResults.Add(subResult);
                }
            }
        }

        subResults = subResults.Where(r => r.Code != UMService.SuccessButDoNothingBecauseUM).ToList();
        if (uMResult != null)
        {
            subResults.Insert(0, new UpdateGroupTypeResult(uMResult));
        }

        return new UpdateCustomerResult(request.Context.CustId, request.Context.UserName, subResults);
    }

    public async Task<List<UpdateCustomerResult>> UpdateMultiple(EditMultipleRequest request)
    {
        MapUpdatedCustomerListFromString(request);
        var result = new List<UpdateCustomerResult>();
        if (request.Context.RoleId == CustomerLevel.Member && IsUpdatePT(request.Settings))
        {
            await UpdateGroupedPTMembers(request);
        }

        request.Context.IsMultipleMode = true;
        foreach (var updatedCustomer in request.Context.UpdatedCustomerList)
        {
            request.Context.CustId = updatedCustomer.CustId;
            request.Context.UserName = updatedCustomer.UserName;
            var updateResult = await Update(new EditRequest()
            {
                Context = request.Context,
                Settings = request.Settings
            });

            result.Add(updateResult);
        }

        return result;
    }

    private async Task UpdateGroupedPTMembers(EditMultipleRequest request)
    {
        var groupedMemberIds = await GetGroupedMemberIds(request.Context.CustomerIds);
        if (groupedMemberIds.Any())
        {
            groupedPTMemberIds = groupedMemberIds;
            updateGroupedPTResults = new List<UpdateGroupTypeResult>();
            foreach (var productGroup in request.Settings.ProductGroups)
            {
                foreach (var groupType in productGroup.GroupTypes.Where(gt => helperService.IsPTGroupType(gt.Id)))
                {
                    var updateResult = await ((GroupType153Service)groupItemForEditMapping[PTGroupTypeId]).UpdateGroupedPTMembers(
                        request.Context,
                        productGroup.Id,
                        groupType.Id,
                        string.Join(",", groupedMemberIds),
                        groupType.GroupItems,
                        appSettings.SettingStatus);
                    if (updateResult.ExtraResult != null)
                    {
                        updateGroupedPTResults.Add(updateResult.ExtraResult);
                    }

                    updateGroupedPTResults.Add(updateResult);
                }
            }
        }
    }

    private async Task<UpdateGroupTypeResult> UpdateGeneralInfo(EditRequest request)
    {
        GeneralInfoService generalInfoService = new(siteDBGroupService);
        using IObjectDb db = ObjectDbFactory.CreateInstance(generalInfoService.GetSPName());
        var parameters = await generalInfoService.GenerateSPParams(request, appSettings.SettingStatus);
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(0, Convert.ToString(outputParams["errmsg"]), request.Context.Language);
        var result = new UpdateGroupTypeResult(0, null, 0, StaticResources.Get("GeneralInfo", request.Context.Language), Convert.ToInt32(outputParams["errcode"]), msg.MainMessage, msg.SubMessages);
        if (result.IsSuccessful)
        {
            result.Message = StaticResources.Get($"UpdateSuccess_GeneralInfo", request.Context.Language);
        }

        return result;
    }

    private async Task<UpdateGroupTypeResult> UpdateGroupType(
        EditRequestContext context,
        int productGroupId,
        int groupTypeId,
        string uGroup,
        IEnumerable<GroupItemDefault> groupItems,
        byte settingStatus)
    {
        IEditGroupTypeBaseService editGrouptypeService = groupItemForEditMapping[groupTypeId];

        if (context.IsMultipleMode && context.RoleId == CustomerLevel.Super)
        {
            return await editGrouptypeService.UpdateByColumns(context, productGroupId, groupTypeId, groupItems, settingStatus);
        }

        return helperService.IsSBCommGroupType(groupTypeId)
            ? await editGrouptypeService.UpdateSBComm(context, productGroupId, groupTypeId, uGroup, groupItems, settingStatus)
            : await editGrouptypeService.Update(context, productGroupId, groupTypeId, groupItems, settingStatus);
    }

    private async Task<IEnumerable<int>> GetGroupedMemberIds(string memberIds)
    {
        using IObjectDb db = ObjectDbFactory.CreateInstance("Acc_CustSetting_GroupedPTMemberLstSelect");
        return await db.QueryAsync<int>(new { CustIDList = memberIds });
    }

    private void MapUpdatedCustomerListFromString(EditMultipleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Context.CustomerIds))
        {
            throw new InvalidOperationException("Customer Ids cannot be null or empty");
        }

        if (string.IsNullOrWhiteSpace(request.Context.CustomerNames))
        {
            throw new InvalidOperationException("Customer Names cannot be null or empty");
        }

        var custIdsList = request.Context.CustomerIds.Split(',').Select(custId => Convert.ToInt32(custId)).ToList();
        var custNameList = request.Context.CustomerNames.Split(',').ToList();

        if (custIdsList.Count != custNameList.Count)
        {
            throw new InvalidOperationException("Customer Ids & Customer Names must be same quantity");
        }

        request.Context.UpdatedCustomerList = new List<EditMultipleCustomerItem>();
        for (int i = 0; i < custIdsList.Count; i++)
        {
            request.Context.UpdatedCustomerList.Add(new EditMultipleCustomerItem { CustId = custIdsList[i], UserName = custNameList[i] });
        }
    }

    private bool IsUpdatePT(CustomerSettingModel settings)
        => settings.ProductGroups.Any(gr => gr.GroupTypes.Any(gt => helperService.IsPTGroupType(gt.Id)));
}
