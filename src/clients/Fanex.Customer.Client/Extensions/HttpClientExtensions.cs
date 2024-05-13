using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Fanex.Customer.Client.Extensions
{
    public static class HttpClientExtensions
    {
        private static readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public static async Task<T> GetAsJsonAsync<T>(this HttpClient httpClient, string requestUri)
        {
            var content = await httpClient.GetStringAsync(requestUri);
            return JsonSerializer.Deserialize<T>(content, serializerOptions);
        }

        public static async Task<T> PostAsJsonAsync<T>(this HttpClient httpClient, string requestUri, object payload = null)
        {
            var response = await httpClient.PostAsStringAsync(requestUri, payload);
            return JsonSerializer.Deserialize<T>(response, serializerOptions);
        }

        public static async Task<string> PostAsStringAsync(this HttpClient httpClient, string requestUri, object payload = null)
        {
            using (var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"))
            {
                using (var response = await httpClient.PostAsync(requestUri, content).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        public static async Task<string> PutAsStringAsync(this HttpClient httpClient, string requestUri, object payload = null)
        {
            using (var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"))
            {
                using (var response = await httpClient.PutAsync(requestUri, content).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        public static async Task<T> PutAsJsonAsync<T>(this HttpClient httpClient, string requestUri, object payload = null)
        {
            var response = await httpClient.PutAsStringAsync(requestUri, payload);
            return JsonSerializer.Deserialize<T>(response, serializerOptions);
        }
    }
}
