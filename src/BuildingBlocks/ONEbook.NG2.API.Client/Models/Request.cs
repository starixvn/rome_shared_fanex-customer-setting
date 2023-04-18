namespace ONEbook.NG2.API.DTO.CreateAccount;

public class Request : BaseInfo
{
    public Request(Auth auth, Call call)
    {
        Auth = auth;
        Call = call;
    }

    public Auth Auth { get; }

    public Call Call { get; }
}