namespace Fanex.Customer.API.Modules.UserNameSuggestion.Models
{
    public class UsernameSuggestion
    {
        public UsernameSuggestion()
        {
        }

        public UsernameSuggestion(string userName, string firstName, string lastName, string username2)
        {
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            UserName2 = username2;
        }

        public string UserName { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string UserName2 { get; set; } = string.Empty;
    }
}