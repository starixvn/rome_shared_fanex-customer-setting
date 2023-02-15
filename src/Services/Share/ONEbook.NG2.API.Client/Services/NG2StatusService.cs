using System.Text;
using EasyNetQ;
using Fanex.Logging;
using Newtonsoft.Json;
using ONEbook.NG2.API.Client.Configurations;
using ONEbook.NG2.API.Client.Models;

namespace ONEbook.NG2.API.Client.Services;

public interface ING2StatusService
{
    Task<Response> SyncCustomers(Request request);
}

public class NG2StatusService : ING2StatusService
{
    private readonly ILogger logger;
    private readonly NG2Configuration configuration;
    private readonly HttpClient httpClient;

    public NG2StatusService(ILogger logger, NG2Configuration configuration, HttpClient httpClient)
    {
        this.logger = logger;
        this.configuration = configuration;
        this.httpClient = httpClient;
        this.httpClient.BaseAddress = new Uri(configuration.Url);
    }

    public async Task<Response> SyncCustomers(Request request)
    {
        var response = await SendCreateAccountRequest(request);
        if (configuration.Debug)
        {
            response.Result.ErrorCode = configuration.Success ? 0 : 1;
            foreach (var account in response.Result.AccountArray)
            {
                account.IsCreated = configuration.Success ? 1 : 0;
            }
        }

        if (!response.Result.IsSuccess)
        {
            await NotifyFailedRequest(request);
        }

        return response;
    }

    private async Task<Response> SendCreateAccountRequest(Request request)
    {
        try
        {
            using var stringContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/CreateAccount") { Content = stringContent };
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            httpResponseMessage.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<Response>(await httpResponseMessage.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            await logger.ErrorAsync(ex.Message, ex);
            var response = new Response();
            response.Result.ErrorCode = 5008;
            response.Result.ErrorDescription = ex.Message;

            return response;
        }
    }

    private async Task NotifyFailedRequest(Request request)
    {
        try
        {
            var rabbitConfig = configuration.Rabbit;
            var rabbitConnectionString = string.Format(
                "host={0};virtualHost={1};username={2};password={3};requestedHeartbeat={4}",
                rabbitConfig.Host,
                rabbitConfig.Name,
                rabbitConfig.Uid,
                rabbitConfig.Pwd,
                rabbitConfig.Timeout);

            using var bus = RabbitHutch.CreateBus(rabbitConnectionString);
            await bus.PublishAsync(request, rabbitConfig.Topic);
        }
        catch (Exception ex)
        {
            await logger.ErrorAsync(ex.Message, ex);
        }
    }
}