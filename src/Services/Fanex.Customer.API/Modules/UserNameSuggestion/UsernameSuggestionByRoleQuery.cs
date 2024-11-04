namespace Fanex.Customer.API.Modules.UserNameSuggestion;

public class UsernameSuggestionByRoleQuery
{
    public int CustId { get; set; }

    public string Username { get; set; }

    public byte RoleId { get; set; }
}