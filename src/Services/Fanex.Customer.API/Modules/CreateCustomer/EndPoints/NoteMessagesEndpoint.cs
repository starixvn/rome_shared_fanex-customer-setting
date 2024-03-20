using Fanex.Customer.API.Modules.CreateCustomer.Services;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CreateCustomer.EndPoints;

public class NoteMessagesEndpoint : IEndpoint<IResult, INoteMessageService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/note/headlines-belong-site", (INoteMessageService service)
            => HandleAsync(service))
            .Produces<HeadlineBelongSiteInfo>().WithTags("Create Customer Full Mode");
            
        app.MapGet("/api/note/super-prefix", (INoteMessageService service)
            => HandleSuperPrefixAsync(service))
            .Produces<SuperPrefix>().WithTags("Create Customer Full Mode");
    }

    public async Task<IResult> HandleAsync(INoteMessageService service)
    {
        return Results.Ok(await service.GetHeadlinesBelongSite());
    }

    public async Task<IResult> HandleSuperPrefixAsync(INoteMessageService service)
    {
        return Results.Ok(await service.GetSuperPrefixes());
    }
}