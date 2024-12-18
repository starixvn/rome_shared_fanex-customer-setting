using Fanex.Data.Repository;
using TelegramBot.Client.Extensions;

namespace Fanex.Customer.API.Modules.Password.Service;

public class PasswordServiceBase : BaseService
{
    private readonly IEncryption encryption;
    private readonly AppSettings appSettings;
    private readonly HttpClient httpClient;

    public PasswordServiceBase(IDynamicRepository dynamicRepository, IEncryption encryption, AppSettings appSettings, HttpClient httpClient) : base(dynamicRepository)
    {
        this.encryption = encryption;
        this.appSettings = appSettings;
        this.httpClient = httpClient;
        this.httpClient.BaseAddress = new Uri(this.appSettings.PasswordService.Uri);
    }

    protected async Task<string> HashPassword(string password)
    {
        var newHash = encryption.CfsEnCode(password);

        try
        {
            if (appSettings.PasswordService.IsEnabled)
            {
                return await httpClient.PostAsJsonAsync<string>("password/hash", new { password });
            }

            return newHash;
        }
        catch
        {
            return newHash;
        }
    }
}