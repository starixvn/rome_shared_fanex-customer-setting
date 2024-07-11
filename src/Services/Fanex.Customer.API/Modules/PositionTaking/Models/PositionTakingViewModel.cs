namespace Fanex.Customer.API.Modules.PositionTaking.Models;

public class PositionTakingViewModel
{
    public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public int TotalPage { get; set; }

        public IList<int> PageSizeList { get; set; }

        public int[] ViewedLevels { get; set; }

        public bool IsShowUplinePT { get; set; }

        public CustomerListInfo CustomerInfo { get; set; }

        public CustomerListInfo ViewedCustomerInfo { get; set; }

        public List<PositionTakingModel> CustomerList { get; set; }

        public ProductModel Product { get; set; }

        public string SiteLabel { get; set; }

        public string Language { get; set; }
}