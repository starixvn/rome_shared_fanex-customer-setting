using Fanex.Customer.API.Modules.Password.Service;
using Fanex.Customer.API.Modules.Password.Model;
using Fanex.Customer.API.Shared.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Password.Endpoint
{
    public class PasswordEndpoint :
        IEndpoint<Result, ResetMainAccountPasswordParam, IPasswordService>,
        IEndpoint<Result, ChangeMainAccountPasswordParam, IPasswordService>,
        IEndpoint<Result, ResetSubAccountPasswordParam, IPasswordService>,
        IEndpoint<Result, ChangeSubAccountPasswordParam, IPasswordService>,
        IEndpoint<Result, SkipForceChangePasswordParam, IPasswordService>
    {
        public void AddRoute(IEndpointRouteBuilder app)
        {
            var mainAcc = app.MapGroup("/api/main-account/password")
                .WithTags(EnpointTags.MainAccountPassword);

            mainAcc.MapPost("/reset", (ResetMainAccountPasswordParam request1, IPasswordService request2)
            => HandleAsync(request1, request2))
            .Produces<Result>();

            mainAcc.MapPost("/change", (ChangeMainAccountPasswordParam request1, IPasswordService request2)
            => HandleAsync(request1, request2))
            .Produces<Result>();

            var subAccount = app.MapGroup("/api/sub-account/password")
                .WithTags(EnpointTags.SubAccount);

            subAccount.MapPost("/reset", (ResetSubAccountPasswordParam request1, IPasswordService request2)
            => HandleAsync(request1, request2))
            .Produces<Result>();

            subAccount.MapPost("/change", (ChangeSubAccountPasswordParam request1, IPasswordService request2)
            => HandleAsync(request1, request2))
            .Produces<Result>();

            var acc = app.MapGroup("/api/account/password")
                .WithTags(EnpointTags.SubMainAccountsPassword);

            acc.MapPost("/skip", (SkipForceChangePasswordParam request1, IPasswordService request2)
            => HandleAsync(request1, request2))
            .Produces<Result>();
        }

        public async Task<Result> HandleAsync(ResetMainAccountPasswordParam request1, IPasswordService request2)
        {
            return await request2.ResetMainAccountPassword(request1);
        }

        public async Task<Result> HandleAsync(ChangeMainAccountPasswordParam request1, IPasswordService request2)
        {
            return await request2.ChangeMainAccountPassword(request1);
        }

        public async Task<Result> HandleAsync(ResetSubAccountPasswordParam request1, IPasswordService request2)
        {
            return await request2.ResetSubAccountPassword(request1);
        }

        public async Task<Result> HandleAsync(ChangeSubAccountPasswordParam request1, IPasswordService request2)
        {
            return await request2.ChangeSubAccountPassword(request1);
        }

        public async Task<Result> HandleAsync(SkipForceChangePasswordParam request1, IPasswordService request2)
        {
            return await request2.SkipForceChangePassword(request1);
        }
    }
}