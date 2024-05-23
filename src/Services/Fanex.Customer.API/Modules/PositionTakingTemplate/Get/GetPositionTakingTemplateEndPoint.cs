using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class GetPositionTakingTemplateEndPoint : IEndpoint<IResult, int, IDynamicRepository>
{
    private readonly AppSettings appSettings;

    public GetPositionTakingTemplateEndPoint(IOptions<AppSettings> appSettings)
    {
        this.appSettings = appSettings.Value;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{custId}/pt-templates", (int custId, IDynamicRepository dynamicRepository)
            => HandleAsync(custId, dynamicRepository))
            .Produces<IEnumerable<PTTemplate>>()
            .WithTags(EndpointTags.CustomerSettings);
    }

    public async Task<IResult> HandleAsync(int custId, IDynamicRepository dynamicRepository)
    {
        if (custId < 0)
        {
            return Results.BadRequest();
        }

        var items = await dynamicRepository.FetchAsync<PositionTakingTemplateDto>(new GetPositionTakingTemplateCriteria(custId, appSettings.SettingStatus));

        var response = from item in items
                       group item by new
                       {
                           Id = item.TemplateId,
                           Name = item.TemplateName,
                           item.IsDefault
                       } into itemGroup
                       select new PTTemplate
                       {
                           Id = itemGroup.Key.Id,
                           Name = itemGroup.Key.Name,
                           IsDefault = itemGroup.Key.IsDefault,
                           Settings = itemGroup.Select(i => new PTTemplateDetail
                           {
                               GroupId = i.GroupId,
                               SubGroupId = i.SubGroupId,
                               PT = i.PT
                           })
                       };

        return Results.Ok(response);
    }
}