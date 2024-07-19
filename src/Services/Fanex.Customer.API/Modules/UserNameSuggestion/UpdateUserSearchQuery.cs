namespace Fanex.Customer.API.Modules.UserNameSuggestion;

public class UpdateUserSearchQuery
{
    public int UserId { get; set; }
    
    public string Search { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Nickname { get; set; }

    public bool IsValid() => UserId > 0 && !string.IsNullOrWhiteSpace(Search);
}