namespace ONEbook.NG2.API.DTO;

public class BaseToken
{
    public BaseToken()
    {
        Seq = Guid.NewGuid().ToString("D");
    }

    public string Seq { get; internal set; }
}