namespace Fanex.Customer.API.Modules.Password.Service.Model
{
    public class ChangeMainAccountPasswordParam : ResetSubAccountPasswordParam
    {
        public string CurrentPassword { get; set; }

        public string NickName { get; set; }
    }
}