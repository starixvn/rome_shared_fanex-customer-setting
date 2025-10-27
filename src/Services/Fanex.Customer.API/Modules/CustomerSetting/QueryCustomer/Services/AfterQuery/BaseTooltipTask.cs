using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;

namespace Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Services.AfterQuery;

public abstract class BaseTooltipTask : IAfterQueryTask
{
    public abstract int GroupTypeId { get; }

    public abstract Task ExecuteAsync(CustomerSettingModel customerSetting, AfterQueryContext context);

    protected void SetTooltipForGroupItems(
        CustomerSettingModel customerSetting, 
        int groupTypeId,
        Func<GroupItemDefault, bool> itemFilter,
        string tooltipMessage)
    {
        var productGroupsList = customerSetting.ProductGroups?.ToList();
        if (productGroupsList == null) 
        {
            return;
        }

        foreach (var productGroup in productGroupsList)
        {
            ProcessProductGroupWithFilter(productGroup, groupTypeId, itemFilter, tooltipMessage);
        }
        customerSetting.ProductGroups = productGroupsList;
    }

    protected void SetTooltipForGroupItems(
        CustomerSettingModel customerSetting,
        int groupTypeId,
        Dictionary<(int SportTypeId, int BetTypeId), string> tooltipMappings)
    {
        var productGroupsList = customerSetting.ProductGroups?.ToList();
        if (productGroupsList == null) 
        {
            return;
        }

        foreach (var productGroup in productGroupsList)
        {
            ProcessProductGroupWithMappings(productGroup, groupTypeId, tooltipMappings);
        }
        customerSetting.ProductGroups = productGroupsList;
    }

    internal void ProcessProductGroupWithFilter(
        ProductGroup productGroup,
        int groupTypeId,
        Func<GroupItemDefault, bool> itemFilter,
        string tooltipMessage)
    {
        if (productGroup.GroupTypes == null) 
        {
            return;
        }

        var groupTypesList = productGroup.GroupTypes.ToList();
        foreach (var groupType in groupTypesList)
        {
            ProcessGroupTypeWithFilter(groupType, groupTypeId, itemFilter, tooltipMessage);
        }
        productGroup.GroupTypes = groupTypesList;
    }

    internal void ProcessProductGroupWithMappings(
        ProductGroup productGroup,
        int groupTypeId,
        Dictionary<(int SportTypeId, int BetTypeId), string> tooltipMappings)
    {
        if (productGroup.GroupTypes == null) 
        {
            return;
        }

        var groupTypesList = productGroup.GroupTypes.ToList();
        foreach (var groupType in groupTypesList)
        {
            ProcessGroupTypeWithMappings(groupType, groupTypeId, tooltipMappings);
        }
        productGroup.GroupTypes = groupTypesList;
    }

    internal void ProcessGroupTypeWithFilter(
        GroupType groupType,
        int groupTypeId,
        Func<GroupItemDefault, bool> itemFilter,
        string tooltipMessage)
    {
        if (groupType.Id != groupTypeId || groupType.GroupItems == null) 
        {
            return;
        }

        var groupItemsList = groupType.GroupItems.ToList();
        var matchingItems = groupItemsList.Where(itemFilter);
        foreach (var groupItem in matchingItems)
        {
            groupItem.TooltipMessage = tooltipMessage;
        }
        groupType.GroupItems = groupItemsList;
    }

    internal void ProcessGroupTypeWithMappings(
        GroupType groupType,
        int groupTypeId,
        Dictionary<(int SportTypeId, int BetTypeId), string> tooltipMappings)
    {
        if (groupType.Id != groupTypeId || groupType.GroupItems == null) 
        {
            return;
        }

        var groupItemsList = groupType.GroupItems.ToList();
        var itemsWithTooltip = groupItemsList.Where(item => 
        {
            var key = (item.SportTypeId, item.BetTypeId);
            return tooltipMappings.ContainsKey(key);
        });
        
        foreach (var groupItem in itemsWithTooltip)
        {
            var key = (groupItem.SportTypeId, groupItem.BetTypeId);
            groupItem.TooltipMessage = tooltipMappings[key];
        }
        groupType.GroupItems = groupItemsList;
    }
}