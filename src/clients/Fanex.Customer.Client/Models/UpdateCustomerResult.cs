using System.Collections.Generic;

namespace Fanex.Customer.Client.Models
{
    public class UpdateCustomerResult
    {
        public int CustId { get; set; }

        public string CustName { get; set; }

        public IEnumerable<UpdateGroupTypeResult> SubResults { get; private set; }
    }

    public class UpdateGroupTypeResult : Result
    {
        public int ProductGroupId { get; set; }

        public string ProductGroupName { get; set; }

        public int GroupTypeId { get; set; }

        public string GroupTypeName { get; set; }

        public IEnumerable<SubMessageContent> SubMessages { get; }
    }
}