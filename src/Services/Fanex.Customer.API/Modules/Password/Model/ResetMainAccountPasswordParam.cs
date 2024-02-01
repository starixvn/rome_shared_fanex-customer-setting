namespace Fanex.Customer.API.Modules.Password.Model
{
    public class ResetMainAccountPasswordParam : ResetSubAccountPasswordParam
    {
        public bool IsResetMemberPassword { get; set; }
    }
}