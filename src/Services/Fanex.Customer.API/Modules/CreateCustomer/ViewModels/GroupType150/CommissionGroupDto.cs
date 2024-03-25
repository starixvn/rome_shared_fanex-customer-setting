namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType150;

public class CommissionGroupDto
{
        public short SportTypeId { get; set; }

        public short BetTypeId { get; set; }

        public decimal Discount { get; set; }

        public decimal Step { get; set; }

        public decimal UplineDiscount { get; set; }

        public decimal? UplineOfUplineDiscount { get; set; }
}
