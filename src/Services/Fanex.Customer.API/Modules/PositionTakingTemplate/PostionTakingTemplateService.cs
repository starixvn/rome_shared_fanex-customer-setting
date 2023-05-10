using System.Text;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public interface IPositionTakingTemplateService
{
    Task<IEnumerable<PTTemplate>> GetByCustId(int custId);

    Task<Result> Create(CreatePositionTakingTemplateRequest request);
}

public class PositionTakingTemplateService : IPositionTakingTemplateService
{
    private readonly AppSettings appSettings;
    private readonly IDynamicRepository dynamicRepository;

    public PositionTakingTemplateService(IOptions<AppSettings> appSettings, IDynamicRepository dynamicRepository)
    {
        this.appSettings = appSettings.Value;
        this.dynamicRepository = dynamicRepository;
    }

    public async Task<IEnumerable<PTTemplate>> GetByCustId(int custId)
    {
        var items = await dynamicRepository.FetchAsync<PositionTakingTemplateDto>(new GetPositionTakingTemplateCriteria(custId));

        var result = from item in items
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
                         Details = itemGroup.Select(i => new PTTemplateDetail
                         {
                             TemplateId = i.TemplateId,
                             GroupId = i.GroupId,
                             SubGroupId = i.SubGroupId,
                             PT = i.PT
                         })
                     };

        return result;
    }

    public async Task<Result> Create(CreatePositionTakingTemplateRequest request)
    {
        var template = request.Template;
        var command = new CreatePositionTakingTemplateCommand(
            request.CustId,
            template.Name,
            BuildXMLSettings(template.Details),
            template.IsDefault,
            appSettings.CreationStatus,
            request.Language,
            request.IP);

        await dynamicRepository.ExecuteAsync(command);

        return new Result(command.ErrorCode, command.ErrMsg);
    }

    private static string BuildXMLSettings(IEnumerable<PTTemplateDetail> details)
    {
        var builder = new StringBuilder();

        foreach (var item in details)
        {
            builder.Append(item.ToXml());
        }

        return $"<Root>{builder}</Root>";
    }
}