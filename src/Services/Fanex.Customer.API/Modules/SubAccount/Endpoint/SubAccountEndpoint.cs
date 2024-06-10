using Fanex.Customer.API.Modules.SubAccount.Model;
using Fanex.Customer.API.Modules.SubAccount.Service;
using Fanex.Customer.API.Shared.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.SubAccount.Endpoint
{
    public class SubAccountEndpoint :
        IEndpoint<IResult, int, ISubAccountService>,
        IEndpoint<IResult, int, int, ISubAccountService>,
        IEndpoint<IResult, AddSubAccountModel, ISubAccountService>,
        IEndpoint<IResult, UpdateSubAccountInfoModel, ISubAccountService>,
        IEndpoint<IResult, UpdateSubAccountPageAccessModel, ISubAccountService>
    {
        public void AddRoute(IEndpointRouteBuilder app)
        {
            var subAcc = app.MapGroup("/api/sub-account")
                .WithTags(EndpointTags.SubAccount);

            subAcc.MapGet("/index", (int custId, ISubAccountService service)
            => HandleAsync(custId, service))
            .Produces<Result>();

            subAcc.MapGet("/get", (int custId, int subAccId, ISubAccountService service)
            => HandleAsync(custId, subAccId, service))
            .Produces<Result>();

            subAcc.MapPost("/add", (AddSubAccountModel model, ISubAccountService service)
            => HandleAsync(model, service))
            .Produces<Result>();

            subAcc.MapPost("/update-info", (UpdateSubAccountInfoModel model, ISubAccountService service)
            => HandleAsync(model, service))
            .Produces<Result>();

            subAcc.MapPost("/update-pageaccess", (UpdateSubAccountPageAccessModel model, ISubAccountService service)
            => HandleAsync(model, service))
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

        public async Task<IResult> HandleAsync(AddSubAccountModel model, ISubAccountService service)
        {
            if (model.CustId > 0)
            {
                var response = await service.AddSubAccount(model);
                return Results.Ok(response);
            }

            return Results.BadRequest();
        }

        public async Task<IResult> HandleAsync(UpdateSubAccountInfoModel model, ISubAccountService service)
        {
            if (model.CustId > 0 && model.SubAccId > 0)
            {
                var response = await service.UpdateSubAccountInfo(model);
                return Results.Ok(response);
            }

            return Results.BadRequest();
        }

        public async Task<IResult> HandleAsync(UpdateSubAccountPageAccessModel model, ISubAccountService service)
        {
            if (model.CustId > 0 && model.SubAccId > 0)
            {
                var response = await service.UpdateSubAccountPageAccess(model);
                return Results.Ok(response);
            }

            return Results.BadRequest();
        }
    }
}