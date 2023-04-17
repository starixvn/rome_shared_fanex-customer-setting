namespace Fanex.Customer.API.Modules.Customer.Requests;

public class GetSuggestUsernameRequest
{
    public string ActorName { get; set; }

    public byte RoleId { get; set; }

    public bool IsValid()
        => !string.IsNullOrWhiteSpace(ActorName)
            && 1 < RoleId && RoleId <= 4;
}