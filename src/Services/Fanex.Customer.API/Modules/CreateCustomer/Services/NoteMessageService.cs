using Fanex.Customer.API.Modules.CreateCustomer.Criteria;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface INoteMessageService
{
    Task<IEnumerable<HeadlineBelongSiteInfo>> GetHeadlinesBelongSite();
    
    Task<IEnumerable<SuperPrefix>> GetSuperPrefixes();
}

public class NoteMessageService : INoteMessageService
{
    private readonly IDynamicRepository dynamicRepository;

    public NoteMessageService(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
    }

    public async Task<IEnumerable<HeadlineBelongSiteInfo>> GetHeadlinesBelongSite()
        => await this.dynamicRepository.FetchAsync<HeadlineBelongSiteInfo>(new GetHeadlineBelongSiteCriteria());

    public async Task<IEnumerable<SuperPrefix>> GetSuperPrefixes()
        => await this.dynamicRepository.FetchAsync<SuperPrefix>(new SuperPrefixCriteria());
}
