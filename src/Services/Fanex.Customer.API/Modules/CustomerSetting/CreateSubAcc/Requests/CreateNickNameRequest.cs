namespace Fanex.Customer.API.Modules.CreateSubAcc.Requests;
public class CreateNickNameRequest : BaseSubAccountRequest
{    
    public int SubAccId { get; set; }

    public bool IsEnableUpdateNickname{ get; set; }
}