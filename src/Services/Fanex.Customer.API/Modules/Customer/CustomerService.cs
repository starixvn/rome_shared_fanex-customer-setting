using Fanex.Customer.API.Modules.Customer.Criteria;
using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Customer.API.Modules.Customer.Requests;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Shared.Enumerations;
using Fanex.Customer.API.Shared.Models;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.Customer;

public interface ICustomerService
{
    Task<CustomerPT> GetCustomerPT(int custId, byte custLevel);

    Task<IEnumerable<SuggestCustomer>> SearchCustomer(SearchCustomerRequest param);

    Task<CreditInfo> GetCreditInfo(string username);

    Task<Result> GetSuggestUsername(GetSuggestUsernameRequest request);

    Task<Result> GetSuggestDirectMemberUsername(string uplineUserName);

    Task<IEnumerable<SuggestDirectMember>> SearchSuggestionDirectMember(int uplineCustId, string searchUserName);
    
    Task<int> CheckExistDirectMember(int uplineCustId, string userName);

    Task<IEnumerable<SuggestCashMember>> SearchSuggestionCashMember(int uplineCustId, string searchUserName);
    
    Task<int> CheckExistCashMember(int uplineCustId, string userName);
    
    Task<bool> CheckExistedCustomer(string username);

    Task<Downline> GetDownlineByUsername(int custId, int subAccountId, string username);

    Task<bool> IsEnableCustomer(int custId);

    Task<CustomerStatus> GetCustomerStatus(int custId, int subAccId);

    Task<List<PTSettingInfo>> GetPTSettingInfo(GetPTSettingInformationRequest request);

    Task<IEnumerable<CurrentWinLossAmount>> GetCurrentWinLossAmount(GetCurrentWinLossAmountRequest request);

    Task<Object> GetProductCategories(GetProductCategoriesRequest request);

    Task<CommissionInfo> GetCommissionInfo(GetCommissionHintRequest request);

    Task<MaxPayoutSettingInfo> GetMaxPayoutSettingInfo(GetMaxPayoutSettingInformationRequest request);

    Task<MemberBalance> GetMemberBalance(int custId, string siteDBName);

    Task<List<string>> GetBackupDomains(string site, byte roleId);

    Task<List<string>> GetDomainsForCashMember(int uplineCustId);

    Task<AccountInfo> GetAccountInfo(int custId);

    Task<Object> GetWinLossLimitInformation(GetProductCategoriesRequest request);

    Task<int> GetCashServiceStatus(int custId);
    Task<int> GetCashServiceTotalDownline(int custId);
}

public class CustomerService : ICustomerService
{
    private readonly AppSettings appSettings;

    private readonly IDynamicRepository dynamicRepository;

    private readonly ICacheRefService cacheRefService;

    private readonly ISiteDBService siteDBService;

    public CustomerService(IDynamicRepository dynamicRepository, ICacheRefService cacheRefService, ISiteDBService siteDBService, IOptions<AppSettings> appSettings)
    {
        this.dynamicRepository = dynamicRepository;
        this.appSettings = appSettings.Value;
        this.cacheRefService = cacheRefService;
        this.siteDBService = siteDBService;
    }

    public async Task<CustomerPT> GetCustomerPT(int custId, byte custLevel)
        => await dynamicRepository.GetAsync<CustomerPT>(new GetCustomerPTInfoCriteria(Enumeration.FromValue<UserLevel>(custLevel), custId));

    public async Task<IEnumerable<SuggestCustomer>> SearchCustomer(SearchCustomerRequest param)
    {
        var criteria = new SearchCustomerCriteria(param.CustId, param.CustId, param.Query, param.IsSubAccount, param.ExcludeMember, param.DirectDownLine, param.IsAccInfo, param.IsInternalAgent);
        return await dynamicRepository.FetchAsync<SuggestCustomer>(criteria);
    }

    public Task<CreditInfo> GetCreditInfo(string username)
        => dynamicRepository.GetAsync<CreditInfo>(new GetCustomerMaxCreditCriteria(username));

    public async Task<Result> GetSuggestUsername(GetSuggestUsernameRequest request)
    {
        var command = new GetSuggestUsernameCommand(request.ActorName, request.RoleId);
        await dynamicRepository.ExecuteAsync(command);
        return new Result(command.ErrCode, command.ErrMsg, command.SeqName);
    }

    public async Task<Result> GetSuggestDirectMemberUsername(string uplineUserName)
    {
        var command = new GetSuggestDirectMemberUsernameCommand(uplineUserName);
        await dynamicRepository.ExecuteAsync(command);
        return new Result(command.ErrCode, command.ErrMsg, command.SeqName);
    }

    public async Task<IEnumerable<SuggestDirectMember>> SearchSuggestionDirectMember(int uplineCustId, string searchUserName)
    {
        var command = new SearchSuggestionDirectMemberCriteria(uplineCustId, searchUserName);
        return await dynamicRepository.FetchAsync<SuggestDirectMember>(command);
    }

    public async Task<int> CheckExistDirectMember(int uplineCustId, string userName)
    {
        var command = new CheckExistDirectMemberUsernameCommand(uplineCustId, userName);
        var result = await dynamicRepository.FetchAsync<CheckDirectMember>(command);
        if (!result.Any())
        {
            return 0;
        }

        return result.First().IsEnabled ? result.First().CustId : -1;
    }

    public async Task<IEnumerable<SuggestCashMember>> SearchSuggestionCashMember(int uplineCustId, string searchUserName)
    {
        var command = new SearchSuggestionCashMemberCriteria(uplineCustId, searchUserName);
        return await dynamicRepository.FetchAsync<SuggestCashMember>(command);
    }

    public async Task<int> CheckExistCashMember(int uplineCustId, string userName)
    {
        var command = new CheckExistCashMemberUsernameCommand(uplineCustId, userName);
        var result = await dynamicRepository.FetchAsync<CheckDirectMember>(command);
        if(!result.Any())
        {
            return 0;
        }

        return result.First().IsEnabled ? result.First().CustId : -1;
    }

    public async Task<bool> CheckExistedCustomer(string username)
    {
        var command = new CheckExistedCustomerCommand(username);
        await dynamicRepository.ExecuteAsync(command);
        return command.IsExists;
    }

    public async Task<Downline> GetDownlineByUsername(int custId, int subAccountId, string username)
    {
        var command = new CheckBelongingByUsernameCommand(custId, username, subAccountId);
        await dynamicRepository.ExecuteAsync(command);
        return new(command.IsActive, command.CustId, command.IsSubAcc, command.RoleId);
    }

    public async Task<bool> IsEnableCustomer(int custId)
    {
        var command = new CheckEnableCustomerCommand(custId);
        await dynamicRepository.ExecuteAsync(command);
        return command.ErrCode.Equals(0);
    }

    public async Task<CustomerStatus> GetCustomerStatus(int custId, int subAccId)
    {
        var customerStatusParam = $"<r id=\"{custId}\" sub=\"{subAccId}\"/>";
        var custList = $"<Root>{customerStatusParam}</Root>";
        return (await dynamicRepository.FetchAsync<CustomerStatus>(new GetCustomerStatusCriteria(custList))).FirstOrDefault();
    }

    public async Task<List<PTSettingInfo>> GetPTSettingInfo(GetPTSettingInformationRequest request)
    {
        List<PTSettingInfo> ptSettingInfo = new List<PTSettingInfo>();
        var ptSettingInfoDtos = await dynamicRepository.FetchAsync<PTSettingInfoDto>(new GetPTSettingInfoCriteria(request.CustId, request.IsTest, request.Site));
        foreach (var ptSettingInfoDto in ptSettingInfoDtos)
        {
            ptSettingInfoDto.BettypeName = cacheRefService.GetDisplayedBetTypeName(Convert.ToInt32(ptSettingInfoDto.TypeDesc), request.Language);
            ptSettingInfoDto.DisplayTypeName = cacheRefService.GetBetTypeName(ptSettingInfoDto.TypeId, request.Language);
        }

        foreach (var group in ptSettingInfoDtos.GroupBy(p => new { p.TypeDesc, p.BettypeName }))
        {
            var ptInfoGroup = new PTSettingInfo
            {
                TypeDesc = Convert.ToInt32(group.Key.TypeDesc),
                BettypeName = group.Key.BettypeName,
                Bettypes = new List<BettypeInfo>()
            };

            foreach (var item in group)
            {
                ptInfoGroup.Bettypes.Add(new BettypeInfo { TypeId = item.TypeId, DisplayTypeName = item.DisplayTypeName });
            }

            ptSettingInfo.Add(ptInfoGroup);
        }

        return ptSettingInfo;
    }

    public async Task<IEnumerable<CurrentWinLossAmount>> GetCurrentWinLossAmount(GetCurrentWinLossAmountRequest request)
    {
        var siteDBConfig = await siteDBService.GetSiteDBConfigByCustId(request.CustId);
        var result = await dynamicRepository.FetchAsync<CurrentWinLossAmount>(new GetCurrentWinLossAmountCriteria(
            request.CustId,
            request.SportType,
            request.BetType,
            appSettings.SettingStatus,
            siteDBConfig.LinkedServerName));

        foreach (var item in result)
        {
            item.SportTypeName = cacheRefService.GetSportTypeName(item.SportType, request.Language);
        }

        return result;
    }

    public async Task<Object> GetProductCategories(GetProductCategoriesRequest request)
    {
        const int RNGGameGroupdId = 2;
        const int LiveRNGCasinoGroupdId = 3;
        const int BBINProductID = 47;
        const int NumberGameProductID = 3;
        const int RNGKENOProductID = 8;
        const int LotteryProductID = 28;
        const int TableGameProductID = 40;
        const int KenoLotterySportId = 9919;

        var products = (await dynamicRepository.FetchAsync<ProductCategories>(new GetProductCategoriesCriteria(appSettings.SettingStatus, request.Site, request.CustId, request.CurrencyId))).ToList();
        foreach (var product in products)
        {
            if (product.RealProductId == RNGKENOProductID
            || product.RealProductId == LotteryProductID
            || product.RealProductId == TableGameProductID)
            {
                product.DummySportTypeId = KenoLotterySportId;
            }
            else
            {
                product.DummySportTypeId = product.RealProductId;
            }
        }

        var groupedData = products
            .GroupBy(d => d.GroupId)
            .Select(group => new
            {
                Id = group.Key,
                Name = cacheRefService.GetProductGroupName(group.Key, request.Language),
                Products = group.GroupBy(d => d.SubGroupId)
                    .Select(subGroup => new
                    {
                        Id = subGroup.Key,
                        Name = cacheRefService.GetSubGroupName(subGroup.Key, request.Language),
                        Products = subGroup
                            .GroupBy(g => new { g.GroupId, g.SubGroupId, g.DummySportTypeId })
                            .Select(d => new
                            {
                                Id = d.Key.DummySportTypeId,
                                Name = d.Key.DummySportTypeId == KenoLotterySportId ?
                                    $"{cacheRefService.GetSportTypeName(d.Key.DummySportTypeId, request.Language)} ({string.Join(", ", d.Select(e => cacheRefService.GetProductName(e.RealProductId, request.Language, appSettings.Belonging777Group(request.SiteLabel))))})"
                                    : cacheRefService.GetProductName(d.Key.DummySportTypeId, request.Language, appSettings.Belonging777Group(request.SiteLabel)),
                                Products = ""
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .ToList();

        var jsonResponse = new
        {
            ProductSettingInfo = groupedData,
            ShowBBin = products
                .Where(gr => gr.GroupId == LiveRNGCasinoGroupdId)
                .Any(e => e.RealProductId == BBINProductID),
            ShowRNGKenoNumberGame = products
                .Where(gr => gr.GroupId == RNGGameGroupdId)
                .Any(e => e.RealProductId == NumberGameProductID || e.RealProductId == RNGKENOProductID),
        };
        return jsonResponse;
    }

    public async Task<CommissionInfo> GetCommissionInfo(GetCommissionHintRequest request)
    {
        CommissionInfo commissionInfo = new();
        var commInfoDtos = await dynamicRepository.FetchAsync<CommissionInfoDto>(new GetCommissionHintCriteria(request.CustId, request.Site));
        var commissionTypeList = new List<CommissionHint>();
        foreach (var commInfoDto in commInfoDtos)
        {
            commissionTypeList.Add(new CommissionHint
            {
                CommissionId = commInfoDto.CommId,
                CommissionName = StaticResources.Get($"Comm{commInfoDto.CommId}", request.Language),
                Bettypes = commInfoDto.TypeId
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => new CommissionBettype()
                    {
                        Id = Convert.ToInt32(s),
                        Name = cacheRefService.GetBetTypeName(Convert.ToInt32(s), request.Language)
                    })
                    .Distinct()
                    .ToList()
            });
        }

        commissionInfo.CommissionTypeList = commissionTypeList;
        return commissionInfo;
    }

    public async Task<MaxPayoutSettingInfo> GetMaxPayoutSettingInfo(GetMaxPayoutSettingInformationRequest request)
    {
        var maxPayoutSettingInfo = new MaxPayoutSettingInfo();
        var betTypeIds = new[] { 1, 3, 5, 7, 8, 15, 20, 21, 153, 501, 609, 610, 612, 701, 704, 705, 9400, 9404, 9405, 9406, 9407, 9408, 9409 };
        var sportTypeIds = new[] { 43, 44 };
        var sportTypeNameArray = sportTypeIds.Select(sportTypeId => cacheRefService.GetSportTypeName(sportTypeId)).ToArray();
        var isSpecialPlatform = appSettings.Belonging777Group(request.SiteLabel);

        string[] betTypeNames = betTypeIds.Select(betTypeId =>
        {
            if (betTypeId == 5)
            {
                return StaticResources.Get("lbl1X2", request.Language);
            }

            return cacheRefService.GetBetTypeName(betTypeId, request.Language);
        }).ToArray();

        string sportTypeNames = string.Format(StaticResources.Get("MaxPayoutSettingInfoSportsName", request.Language), sportTypeNameArray[0], sportTypeNameArray[1]);
        maxPayoutSettingInfo.MaxPayoutSport = new MaxPayoutSport
        {
            SportName = sportTypeNames,
            BettypeName = betTypeNames
        };

        maxPayoutSettingInfo.SabaSport = await dynamicRepository.FetchAsync<SabaSport>(new GetSabaSportsCriteria(request.SiteName));
        foreach (var sabaSport in maxPayoutSettingInfo.SabaSport)
        {
            sabaSport.SettingName = cacheRefService.GetParlayMaxPayoutBetSettingInfoSportName(sabaSport.SettingId, isSpecialPlatform, request.Language);
            sabaSport.SettingFollowedName = cacheRefService.GetParlayMaxPayoutBetSettingInfoSportName(sabaSport.SettingIdFollowed, isSpecialPlatform, request.Language);
        }
        maxPayoutSettingInfo.IsSpecialPlatform = isSpecialPlatform;
        return maxPayoutSettingInfo;
    }

    public async Task<MemberBalance> GetMemberBalance(int custId, string siteDBName)
    {
        var memberOutstandingCriteria = new SelectMemberOutstandingCriteria(custId);
        var memberBalanceCriteria = new SelectMemberBalanceBySiteDBCriteria(custId, siteDBName);
        var memberBalanceDTO = await dynamicRepository.GetAsync<MemberBalanceDTO>(new SelectMemberBalanceCriteria(custId));

        await Task.WhenAll(dynamicRepository.ExecuteAsync(memberOutstandingCriteria), dynamicRepository.ExecuteAsync(memberBalanceCriteria));

        return new MemberBalance
        {
            CustId = memberBalanceDTO.CustId,
            YesterdayCustomerCash = memberBalanceDTO.YesterdayCustomerCash,
            Balance = memberBalanceCriteria.Balance,
            PendingTransfer3rd = memberBalanceCriteria.PendingTransfer3rd,
            CustomerOutstanding = memberOutstandingCriteria.Outstanding,
            NetPosition = memberBalanceCriteria.NetPosition
        };
    }

    public async Task<List<string>> GetBackupDomains(string site, byte roleId)
    {
        var backupDomainsResult = await dynamicRepository.FetchAsync<BackupDomain>(new GetGetBackupDomainsCriteria(site));
        var agencyDomains = new List<string>();
        var memberDomains = new List<string>();

        foreach (var item in backupDomainsResult)
        {
            if (item.IsAgency)
            {
                agencyDomains.Add(item.Domain);
            }
            else
            {
                memberDomains.Add(item.Domain);
            }
        }
        var domains = roleId == CustomerLevel.Member ? memberDomains : agencyDomains;
        return domains;
    }

    public async Task<List<string>> GetDomainsForCashMember(int uplineCustId)
    {
        var backupDomainsResult = await dynamicRepository.FetchAsync<BackupDomain>(new GetDomainsForCashMemberCriteria(uplineCustId));
        return [.. backupDomainsResult.Select(d => d.Domain)];
    }

    public async Task<AccountInfo> GetAccountInfo(int custId)
    {
        var accountInfo = await dynamicRepository.GetAsync<AccountInfo>(new GetCustomerInfoCriteria { CustId = custId });
        var domains = await GetBackupDomains(accountInfo.Site, accountInfo.RoleId);
        accountInfo.Domains = domains;
        return accountInfo;
    }

    public async Task<Object> GetWinLossLimitInformation(GetProductCategoriesRequest request)
    {
        const int RNGGameGroupdId = 2;

        var products = (await dynamicRepository.FetchAsync<ProductCategories>(new GetProductCategoriesCriteria(appSettings.SettingStatus, request.Site, request.CustId, request.CurrencyId))).ToList();

        var groupedData = products
            .Where(p => p.GroupId == RNGGameGroupdId)
            .GroupBy(d => d.GroupId)
            .Select(group => new
            {
                Id = group.Key,
                Name = cacheRefService.GetProductGroupName(group.Key, request.Language),
                Products = group.GroupBy(d => d.SubGroupId)
                    .Select(subGroup => new
                    {
                        Id = subGroup.Key,
                        Name = cacheRefService.GetSubGroupName(subGroup.Key, request.Language),
                        Products = subGroup
                            .GroupBy(g => new { g.GroupId, g.SubGroupId, g.RealProductId })
                            .Select(d => new
                            {
                                Id = d.Key.RealProductId,
                                Name = cacheRefService.GetProductName(d.Key.RealProductId, request.Language, appSettings.Belonging777Group(request.SiteLabel)),
                                Products = ""
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .ToList();

        var jsonResponse = new
        {
            ProductSettingInfo = groupedData
        };

        return jsonResponse;
    }

    public async Task<int> GetCashServiceStatus(int custId)
    {
        var criteria = new GetCashServiceStatusCriteria(custId);
        var result = await dynamicRepository.FetchAsync<dynamic>(criteria);
        // Expecting result to have a property named ServiceType (0: Credit, 1: Cash)
        return criteria.ServiceType;
    }
    
    public async Task<int> GetCashServiceTotalDownline(int custId)
    {
        var criteria = new GetCashServiceTotalDownlineCriteria(custId);
        await dynamicRepository.FetchAsync<dynamic>(criteria);
        return criteria.TotalCashMember;
    }
}