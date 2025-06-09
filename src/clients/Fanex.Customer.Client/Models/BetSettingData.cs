using System.Collections.Generic;

namespace Fanex.Customer.Client.Models
{
    public class BetSettingData
    {
        public List<SabaSport> SabaSport { get; set; }
        public bool IsSpecialPlatform { get; set; }
    }

    public class SabaSport
    {
        public string SettingName { get; set; }
        public string SettingFollowedName { get; set; }
        public int RowSpan { get; set; }
    }
} 