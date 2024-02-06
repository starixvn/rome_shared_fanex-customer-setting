namespace Fanex.Customer.API.Modules.Otp
{
    public interface IJsonHelper
    {
        T Deserialize<T>(string json);

        string Serialize(object data);
    }
}