using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Shared.Models;
using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType162;

public class PotentialWinLimitAmount
{
    public decimal AccumWinLoss { get; set; }

    public decimal AccumToWin { get; set; }
}