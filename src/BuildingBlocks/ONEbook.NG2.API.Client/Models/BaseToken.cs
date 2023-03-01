namespace ONEbook.NG2.API.Client.Models;

public class BaseToken
{
    public BaseToken()
    {
        Seq = Guid.NewGuid().ToString("D");
    }

    public string Seq { get; internal set; }
}