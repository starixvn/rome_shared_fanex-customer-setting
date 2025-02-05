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
                .WithTags(EndpointTags.Passwords);

            mainAcc.MapPost("/reset", (ResetMainAccountPasswordParam request, IPasswordService service)
            => HandleAsync(request, service))
            .Produces<Result>();

            mainAcc.MapPost("/change", (ChangeMainAccountPasswordParam request, IPasswordService service)
            => HandleAsync(request, service))
            .Produces<Result>();

            var subAccount = app.MapGroup("/api/sub-account/password")
                .WithTags(EndpointTags.Passwords);

            subAccount.MapPost("/reset", (ResetSubAccountPasswordParam request, IPasswordService service)
            => HandleAsync(request, service))
            .Produces<Result>();

            subAccount.MapPost("/change", (ChangeSubAccountPasswordParam request, IPasswordService service)
            => HandleAsync(request, service))
            .Produces<Result>();

            var acc = app.MapGroup("/api/account/password")
                .WithTags(EndpointTags.Passwords);

            acc.MapPost("/skip", (SkipForceChangePasswordParam request, IPasswordService service)
            => HandleAsync(request, service))
            .Produces<Result>();
        }

        public async Task<Result> HandleAsync(ResetMainAccountPasswordParam request, IPasswordService service)
        {
            return await service.ResetMainAccountPassword(request);
        }

        public async Task<Result> HandleAsync(ChangeMainAccountPasswordParam request, IPasswordService service)
        {
            return await service.ChangeMainAccountPassword(request);
        }

        public async Task<Result> HandleAsync(ResetSubAccountPasswordParam request, IPasswordService service)
        {
            return await service.ResetSubAccountPassword(request);
        }

        public async Task<Result> HandleAsync(ChangeSubAccountPasswordParam request, IPasswordService service)
        {
            return await service.ChangeSubAccountPassword(request);
        }

        public async Task<Result> HandleAsync(SkipForceChangePasswordParam request, IPasswordService service)
        {
            return await service.SkipForceChangePassword(request);
        }
    }
}