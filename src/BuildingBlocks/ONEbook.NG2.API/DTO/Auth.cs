namespace ONEbook.NG2.API.DTO;

public class Auth
{
    public Auth(string login, string password, int siteId)
    {
        Login = login;
        Password = password;
        SiteId = siteId;
    }

    public string Login { get; set; }

    public string Password { get; set; }

    public int SiteId { get; set; }
}