namespace Fanex.Customer.API.Modules.PositionTaking.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Active { get; set; }
    public int SportId { get; set; }
    public string ReportSPParam { get; set; }
    public string ViewPTLink { get; set; }
    public string ViewCommLink { get; set; }
    public string MicroSiteNameAppSetting { get; set; }
    public string CustomerSettingLink { get; set; }
    public string StatusHandlerType { get; set; }
    public bool ShowPTIcon
    {
        get
        {
            var viewPTLinkItems = ViewPTLink?.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            var ptIconItems = viewPTLinkItems?.SingleOrDefault(item => item?.Contains("showpticon") == true);
            var showPTIcon = ptIconItems?.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)?.ElementAt(1);
            return string.IsNullOrWhiteSpace(showPTIcon) ? false : Convert.ToBoolean(showPTIcon);
        }
    }
    public int MainProductId
    {
        get
        {
            var viewPTLinkItems = ViewPTLink?.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            var mainProductIdItems = viewPTLinkItems?.SingleOrDefault(item => item?.Contains("mainProductId") == true);
            var mainProductId = mainProductIdItems?.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)?.ElementAt(1);

            return string.IsNullOrWhiteSpace(mainProductId) ? 0 : Convert.ToInt32(mainProductId);
        }
    }
}

public class ProductViewModel
{
    public IEnumerable<Product> Products { get; set; }
}