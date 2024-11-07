namespace Fanex.Customer.API.Modules.Customer.Requests;

public class GetSuggestDirectMemberUsernameRequest
{
    public string UplineUsername { get; set; }

    public bool IsValid()
        => !string.IsNullOrWhiteSpace(UplineUsername);
}