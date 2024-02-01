namespace Fanex.Customer.API.Modules.Password.Model
{
    public class ChangeSubAccountPasswordParam : ResetSubAccountPasswordParam
    {
        public string CurrentPassword { get; set; }

        public string Username { get; set; }

        public string SubUsername { get; set; }

        public string Nickname { get; set; }
    }
}