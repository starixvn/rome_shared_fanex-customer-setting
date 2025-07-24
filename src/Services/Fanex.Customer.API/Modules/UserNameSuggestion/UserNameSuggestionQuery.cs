namespace Fanex.Customer.API.Modules.UserNameSuggestion;

public class UserNameSuggestionQuery
{
    public string Query { get; set; }

    public bool DirectDownlines { get; set; }

    public bool IsAccInfo { get; set; } = true;

    public int ActorId { get; set; }

    public bool IsInternal { get; set; }

    public bool ExcludeMember { get; set; }

    public bool ExcludeCashMember { get; set; }

    public bool IsValid() => ActorId > 0 && !string.IsNullOrEmpty(Query);
}