using Fanex.Customer.API.Modules.Password.Service;
using Fanex.Customer.API.Modules.Password.Model;
using Fanex.Customer.API.Shared.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Password.Endpoint
{
    public class PasswordEndpoint :
        IEndpoint<IResult, ResetMainAccountPasswordParam, IPasswordService>,
        IEndpoint<IResult, ChangeMainAccountPasswordParam, IPasswordService>,
        IEndpoint<IResult, ResetSubAccountPasswordParam, IPasswordService>,
        IEndpoint<IResult, ChangeSubAccountPasswordParam, IPasswordService>,
        IEndpoint<IResult, SkipForceChangePasswordParam, IPasswordService>,
        IEndpoint<int, UpdateLastChangePasswordIndirectAccountParam, IPasswordService>
    {
        public void AddRoute(IEndpointRouteBuilder app)
        {
            var mainAcc = app.MapGroup("/api/main-account/password")
                .WithTags("Main-Account");

            mainAcc.MapPost("/reset", (ResetMainAccountPasswordParam request1, IPasswordService request2)
            => HandleAsync(request1, request2))
            .Produces<Result>();

            mainAcc.MapPost("/change", (ChangeMainAccountPasswordParam request1, IPasswordService request2)
            => HandleAsync(request1, request2))
            .Produces<Result>();

            var subAccount = app.MapGroup("/api/sub-account/password")
                .WithTags("Sub-Account");

            subAccount.MapPost("/reset", (ResetSubAccountPasswordParam request1, IPasswordService request2)
            => HandleAsync(request1, request2))
            .Produces<Result>();

            subAccount.MapPost("/change", (ChangeSubAccountPasswordParam request1, IPasswordService request2)
            => HandleAsync(request1, request2))
            .Produces<Result>();

            var acc = app.MapGroup("/api/account/password")
                .WithTags("Sub-Main-Accounts");

            acc.MapPost("/skip", (SkipForceChangePasswordParam request1, IPasswordService request2)
            => HandleAsync(request1, request2))
            .Produces<Result>();

            acc.MapPost("/update-last-change-indirect", (UpdateLastChangePasswordIndirectAccountParam request1, IPasswordService request2)
            => HandleAsync(request1, request2))
            .Produces<Result>();
        }

        public async Task<IResult> HandleAsync(ResetMainAccountPasswordParam request1, IPasswordService request2)
        {
            var result = await request2.ResetMainAccountPassword(request1);
            return Results.Ok(result);
        }

        public async Task<IResult> HandleAsync(ChangeMainAccountPasswordParam request1, IPasswordService request2)
        {
            var result = await request2.ChangeMainAccountPassword(request1);
            return Results.Ok(result);
        }

        public async Task<IResult> HandleAsync(ResetSubAccountPasswordParam request1, IPasswordService request2)
        {
            var result = await request2.ResetSubAccountPassword(request1);
            return Results.Ok(result);
        }

        public async Task<IResult> HandleAsync(ChangeSubAccountPasswordParam request1, IPasswordService request2)
        {
            var result = await request2.ChangeSubAccountPassword(request1);
            return Results.Ok(result);
        }

        public async Task<IResult> HandleAsync(SkipForceChangePasswordParam request1, IPasswordService request2)
        {
            var result = await request2.SkipForceChangePassword(request1);
            return Results.Ok(result);
        }

        public async Task<int> HandleAsync(UpdateLastChangePasswordIndirectAccountParam request1, IPasswordService request2)
        {
            var result = await request2.UpdateLastChangePasswordIndirectAccount(request1);
            return result;
        }
    }
}