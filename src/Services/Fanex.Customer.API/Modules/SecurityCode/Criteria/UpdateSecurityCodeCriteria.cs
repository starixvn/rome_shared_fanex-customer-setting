using Fanex.Customer.API.Modules.Password.Criteria;

namespace Fanex.Customer.API.Modules.SecurityCode.Criteria
{
    public abstract class UpdateSecurityCodeCriteria : BaseCommand
    {
        public int CustId { get; set; }

        public string Language { get; set; }

        public string IP { get; set; }

        public override bool IsValid() => CustId > 0;
    }
}
