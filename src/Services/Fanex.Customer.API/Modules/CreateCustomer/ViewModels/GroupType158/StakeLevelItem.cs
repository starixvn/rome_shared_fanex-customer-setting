using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType158;

public class StakeLevelItem : GroupItemDefault
{
        public bool IsMultipleChoice { get; set; }

        public int StakeLevelId { get; set; }

        public string StakeLevelName { get; set; }

        public bool IsChecked { get; set; }

        public int MaxChoice { get; set; }
}
