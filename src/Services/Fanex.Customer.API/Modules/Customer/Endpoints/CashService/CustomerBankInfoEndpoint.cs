using Fanex.Customer.API.Modules.Customer.Requests.CashService;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class CustomerBankInfoEndpoint :
    IEndpoint<IResult, int, int, ICashCustomerService>,
    IEndpoint<IResult, int, ICashCustomerService>,
    IEndpoint<IResult, AddMemberBankInfoRequest, ICashCustomerService>,
    IEndpoint<IResult, UpdateMemberBankInfoRequest, ICashCustomerService>
    // IEndpoint<IResult, RemoveMemberBankInfoRequest, ICashCustomerService>
{

    public void AddRoute(IEndpointRouteBuilder app)
    {
        var enpoint = app.MapGroup("/api/customer/cash-service-bank-info").WithTags(EndpointTags.CashService);


        enpoint.MapGet("/agent/{agentId}/{transType}",
                (int agentId, int transType, ICashCustomerService customerService)
                    => HandleAsync(agentId, transType, customerService))
            .Produces<IResult>();


        enpoint.MapGet("/member/{custId}", (int custId, ICashCustomerService customerService)
                => HandleAsync(custId, customerService))
            .Produces<IResult>();


        enpoint.MapPost("/member/add", (AddMemberBankInfoRequest request, ICashCustomerService customerService)
                => HandleAsync(request, customerService))
            .Produces<IResult>();


        enpoint.MapPut("/member/update", (UpdateMemberBankInfoRequest request, ICashCustomerService customerService)
                => HandleAsync(request, customerService))
            .Produces<IResult>();


        // enpoint.MapDelete("/member/delete", (RemoveMemberBankInfoRequest request, ICashCustomerService customerService)
        //         => HandleAsync(request, customerService))
        //     .Produces<IResult>();

    }

    public async Task<IResult> HandleAsync(int custId, ICashCustomerService customerService)
    {
        if (custId <= 0)
        {
            return Results.BadRequest();
        }

        var result = await customerService.GetMemberBanks(custId);
        return Results.Ok(result);
    }

    public async Task<IResult> HandleAsync(int agentId, int transType, ICashCustomerService customerService)
    {
        if (agentId <= 0)
        {
            return Results.BadRequest();
        }

        var result = await customerService.GetAgentBanks(agentId, transType);
        return Results.Ok(result);
    }

    public async Task<IResult> HandleAsync(AddMemberBankInfoRequest request, ICashCustomerService customerService)
    {
        if (request.CustId <= 0)
        {
            return Results.BadRequest();
        }

        if (request.ActorId <= 0)
        {
            return Results.BadRequest();
        }

        var result = await customerService.AddMemberBankInfo(request);
        return Results.Ok(result);
    }

    public async Task<IResult> HandleAsync(UpdateMemberBankInfoRequest request, ICashCustomerService customerService)
    {
        if (request.CustId <= 0)
        {
            return Results.BadRequest();
        }

        var result = await customerService.UpdateMemberBankInfo(request);
        return Results.Ok(result);
    }

    public async Task<IResult> HandleAsync(RemoveMemberBankInfoRequest request, ICashCustomerService customerService)
    {
        if (request.CustId <= 0)
        {
            return Results.BadRequest();
        }

        var result = await customerService.RemoveMemberBankInfo(request);
        return Results.Ok(result);
    }
}