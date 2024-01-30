using Fanex.Customer.API.Modules.SubAccount.Model;
using Fanex.Customer.API.Modules.SubAccount.Service;
using Fanex.Customer.API.Shared.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.SubAccount.Endpoint
{
    public class SubAccountEndpoint :
        IEndpoint<IResult, int, ISubAccountService>,
        IEndpoint<IResult, int, int, ISubAccountService>,
        IEndpoint<IResult, int, AddSubAccountModel, ISubAccountService>,
        IEndpoint<IResult, int, UpdateSubAccountInfoModel, ISubAccountService>,
        IEndpoint<IResult, int, UpdateSubAccountPageAccessModel, ISubAccountService>
    {
        public void AddRoute(IEndpointRouteBuilder app)
        {
            app.MapGet("api/subaccount/index", (int custId, ISubAccountService service)
            => HandleAsync(custId, service))
            .Produces<Result>();

            app.MapGet("api/subaccount/get", (int custId, int subAccId, ISubAccountService service)
            => HandleAsync(custId, subAccId, service))
            .Produces<Result>();

            app.MapPost("api/subaccount/add", (int custId, AddSubAccountModel model, ISubAccountService service)
            => HandleAsync(custId, model, service))
            .Produces<Result>();

            app.MapPost("api/subaccount/update-info", (int custId, UpdateSubAccountInfoModel model, ISubAccountService service)
            => HandleAsync(custId, model, service))
            .Produces<Result>();

            app.MapPost("api/subaccount/update-pageaccess", (int custId, UpdateSubAccountPageAccessModel model, ISubAccountService service)
            => HandleAsync(custId, model, service))
            .Produces<Result>();
        }

        public async Task<IResult> HandleAsync(int custId, ISubAccountService service)
        {
            if (custId > 0)
            {
                var response = await service.GetSubAccounts(custId);
                return Results.Ok(response);
            }

            return Results.BadRequest();
        }

        public async Task<IResult> HandleAsync(int custId, int subAccId, ISubAccountService service)
        {
            if (custId > 0 && subAccId > 0)
            {
                var response = await service.GetSubAccount(custId, subAccId);
                return Results.Ok(response);
            }
            
            return Results.BadRequest();
        }

        public async Task<IResult> HandleAsync(int custId, AddSubAccountModel model, ISubAccountService service)
        {
            if (custId > 0)
            {
                var response = await service.AddSubAccount(custId, model);
                return Results.Ok(response);
            }

            return Results.BadRequest();
        }

        public async Task<IResult> HandleAsync(int custId, UpdateSubAccountInfoModel model, ISubAccountService service)
        {
            if (custId > 0 && model.SubAccId > 0)
            {
                var response = await service.UpdateSubAccountInfo(custId, model);
                return Results.Ok(response);
            }

            return Results.BadRequest();
        }

        public async Task<IResult> HandleAsync(int custId, UpdateSubAccountPageAccessModel model, ISubAccountService service)
        {
            if (custId > 0 && model.SubAccId > 0)
            {
                var response = await service.UpdateSubAccountPageAccess(custId, model);
                return Results.Ok(response);
            }

            return Results.BadRequest();
        }
    }
}