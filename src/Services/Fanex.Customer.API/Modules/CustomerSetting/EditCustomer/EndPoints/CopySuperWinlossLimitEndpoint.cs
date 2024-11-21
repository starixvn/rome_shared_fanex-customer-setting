using Fanex.Customer.API.Constants;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType155;
using Fanex.Customer.API.Modules.EditCustomer.Requests;
using Fanex.Customer.API.Modules.EditCustomer.Services;
using Fanex.Customer.API.Modules.QueryCustomer.Services;
using Fanex.Customer.API.Shared.Constants;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.EditCustomer.EndPoints;

public class CopySuperWinlossLimitEndpoint : IEndpoint<IResult, CopySuperWinlossLimitRequest, IQueryGroupTypeBaseService<WinLossLimitItem>, IEditCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/customer/copy-supers-winloss-limit", (CopySuperWinlossLimitRequest request, IQueryGroupTypeBaseService<WinLossLimitItem> groupTypeBaseService, IEditCustomerService service)
           => HandleAsync(request, groupTypeBaseService, service))
           .Produces<UpdateCustomerResult>().WithTags(EndpointTags.CustomerSettings);
    }

    public async Task<IResult> HandleAsync(CopySuperWinlossLimitRequest request, IQueryGroupTypeBaseService<WinLossLimitItem> groupTypeBaseService, IEditCustomerService service)
    {
        if(request.SourceSuperId == 0 || string.IsNullOrWhiteSpace(request.CopiedSuperIds))
        {
            return Results.BadRequest("SourceSuperId and CopiedSuperIds cannot be empty.");
        }

        if(!request.SportTypeIds.Any())
        {
            return Results.BadRequest("SportTypeIds cannot be empty.");
        }

        var currentLiveCSWLLimit = (await groupTypeBaseService.QueryForEdit(request.SourceSuperId, CustomerLevel.Super, null, 3, GroupTypes.WinlossLimit, false, 0, "", request.SystemId, "", true, false)).Where(item => request.SportTypeIds.Contains(item.SportTypeId));
        var currentRNGGamesWLLimit = (await groupTypeBaseService.QueryForEdit(request.SourceSuperId, CustomerLevel.Super, null, 2, GroupTypes.WinlossLimit, false, 0, "", request.SystemId, "", true, false)).Where(item => request.SportTypeIds.Contains(item.SportTypeId));
        if (!currentLiveCSWLLimit.Any() && !currentRNGGamesWLLimit.Any())
        { 
            return Results.BadRequest("There is no GroupItems existed.");
        }

        var productGroups = new List<ProductGroup>();

        if (currentLiveCSWLLimit.Any())
        {
            productGroups.Add(new ProductGroup()
            {
                Id = 3,
                GroupTypes = new List<GroupType>(){
                    new GroupType()
                    {
                        Id = 155,
                        GroupItems = currentLiveCSWLLimit.ToList()
                    }
                }
            });
        }

        if (currentRNGGamesWLLimit.Any())
        {
            productGroups.Add(new ProductGroup()
            {
                Id = 2,
                GroupTypes = new List<GroupType>(){
                            new GroupType()
                            {
                                Id = 155,
                                GroupItems = currentRNGGamesWLLimit.ToList()
                            }
                        }
            });
        }

        var editMultipleRequest = new EditMultipleRequest
        {
            Context = new EditMultipleRequestContext()
            {
                ActorId = request.ActorId,
                IP = request.IP,
                Language = request.Language,
                CustId = request.SourceSuperId,
                UserName = request.SourceUserName,
                RoleId = CustomerLevel.Super,
                CustomerIds = request.CopiedSuperIds,
                CustomerNames = request.CopiedUserNames,
                EditFuntion = EditFuntionConstants.EditMultipleSuperWinLimit
            },
            Settings = new CustomerSettingModel()
            {
                ProductGroups = productGroups
            }
        };

        return Results.Ok(await service.UpdateMultiple(editMultipleRequest));
    }
}
