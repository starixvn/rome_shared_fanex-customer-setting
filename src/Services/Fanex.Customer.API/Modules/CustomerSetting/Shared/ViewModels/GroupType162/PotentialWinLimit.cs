using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Shared.Models;
using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType162;

public class PotentialWinLimit : GroupItemDefault
{
    public decimal WinLimit { get; set; }
    
    [JsonIgnore]
    public bool IsAllowUnlimitWinLimit => WinLimit == 0;

    public bool IsAutoReset { get; set; }    

    [JsonIgnore]
    public decimal AccumWinLoss { get; set; }

    [JsonIgnore]
    public decimal AccumToWin { get; set; }
    public override Result Validate(string language)
    {
        // Add validation logic here
        return Result.CreateSuccess();
    }
}