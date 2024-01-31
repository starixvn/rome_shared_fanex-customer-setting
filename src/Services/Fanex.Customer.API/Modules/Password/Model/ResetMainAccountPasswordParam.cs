namespace Fanex.Customer.API.Modules.Password.Service.Model
{
    public class ResetMainAccountPasswordParam : ResetSubAccountPasswordParam
    {
        public bool IsResetMemberPassword { get; set; }
    }
}