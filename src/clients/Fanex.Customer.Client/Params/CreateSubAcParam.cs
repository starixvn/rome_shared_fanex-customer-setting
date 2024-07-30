using Fanex.Customer.Client.Constants;

namespace Fanex.Customer.Client.Params
{
    public class CreateSubAcParam
    {
        public CreateSubAcParam(
           int custId, 
           string userName, 
           string password, 
           string firstName, 
           string lastName, 
           string pageaccess, 
           int loginsubaccid,            
           int userId, 
           string ip, 
           string site,
           string language= Languages.English)
        {
            CustId = custId;
            UserName = userName;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            PageAccess = pageaccess;
            LoginSubAccId = loginsubaccid;
            Language = language;
            UserId = userId;
            IP = ip;
            Site = site;
        }

        public int CustId { get; }
        public string UserName { get; }
        public string Password { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string PageAccess { get; }
        public int LoginSubAccId { get; }
        public string Language { get; }
        public int UserId { get; }
        public string IP { get; }
        public string Site { get; }
    }
}
