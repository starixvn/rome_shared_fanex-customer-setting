using Fanex.Customer.API.Modules.Customer.Criteria;
using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Customer.API.Modules.Customer.Requests;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Models;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data.Repository;
using MinimalApi.Endpoint;
using RabbitMQ.Client.Impl;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class GetCustomerInfoEndpoint :
    IEndpoint<IResult, int, IDynamicRepository>,
    IEndpoint<IResult, UpdateCustomerInfoRequest, IDynamicRepository>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{custId}", (int custId, IDynamicRepository dynamicRepository)
            => HandleAsync(custId, dynamicRepository))
            .Produces<CustomerInfo>()
            .WithTags(EndpointTags.CustomerInfos);

        app.MapPut("/api/customer/info", (UpdateCustomerInfoRequest request, IDynamicRepository dynamicRepository)
            => HandleAsync(request, dynamicRepository))
            .Produces<Result>()
            .WithTags(EndpointTags.CustomerInfos);
    }

    public async Task<IResult> HandleAsync(int custId, IDynamicRepository dynamicRepository)
    {
        if (custId > 0)
        {
            var customerInfo = await dynamicRepository.GetAsync<CustomerInfo>(new GetCustomerInfoCriteria { CustId = custId });
            return Results.Ok(customerInfo);
        }

        return Results.BadRequest();
    }

    public async Task<IResult> HandleAsync(UpdateCustomerInfoRequest request, IDynamicRepository dynamicRepository)
    {
        if (request.IsValid())
        {
            var command = new UpdateCustomerInfoCommand(
                request.CustId,
                request.FirstName,
                request.LastName,
                request.Phone,
                request.MobilePhone,
                request.Email,
                request.Fax,
                request.UserId,
                request.SubAccId,
                request.IP,
                request.Language);
            await dynamicRepository.ExecuteAsync(command);
            return Results.Ok(new Result(command.ErrorCode, command.ErrorMsg));
        }

        var currentLanguage = LanguageUtils.ToLanguage(request.Language);

        return Results.Ok(Result.CreateError(StaticResources.Get("InvalidParameterValue", currentLanguage)));
    }
}
