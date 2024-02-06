using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.Otp
{
    public class JsonHelper : IJsonHelper
    {
        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string Serialize(object data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }
}