namespace ONEbook.NG2.API.DTO.CreateAccount;

public class Call : BaseToken
{
    public Call()
    {
        AccountArray = new List<AccountInfo>();
    }

    public int UplineId { get; set; }

    public int Level { get; set; }

    public int CurrencyId { get; set; }

    public bool? IsDisabled { get; set; }

    public int SiteId
    {
        get
        {
            return Level != 1 ? 0 : 4100300;
        }
    }

    public IList<AccountInfo> AccountArray { get; set; }
}