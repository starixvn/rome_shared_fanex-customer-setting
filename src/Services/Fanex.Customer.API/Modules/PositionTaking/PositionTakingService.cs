using System.Globalization;
using Fanex.Customer.API.Modules.PositionTaking.Criterias;
using Fanex.Customer.API.Modules.PositionTaking.Models;
using Fanex.Customer.API.Modules.PositionTaking.Resources;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data.Repository;
using Fanex.RemotingRef;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.PositionTaking;

public interface IPositionTakingService
{
    IList<int> GetPageSizeList();

    Task<PositionTakingViewModel> GetPT(PositionTakingRequest request);
}

public class PositionTakingService : IPositionTakingService
{
    private readonly AppSettings appSettings;

    private readonly IDynamicRepository dynamicRepository;

    private readonly IRemotingRefService remotingRefService;

    private const int Voidbridge = 4001;

    private readonly int[] NewSettingProductGroups = [ProductGroupConstants.Sportsbook, ProductGroupConstants.LiveAndRNGCasino, ProductGroupConstants.RNGGames];

    public PositionTakingService(IOptions<AppSettings> appSettingOptions, IDynamicRepository dynamicRepository, IRemotingRefService remotingRefService)
    {
        this.appSettings = appSettingOptions.Value;
        this.dynamicRepository = dynamicRepository;
        this.remotingRefService = remotingRefService;
    }

    public IList<int> GetPageSizeList()
    {
        return [10, 50, 100, 500];
    }

    public async Task<PositionTakingViewModel> GetPT(PositionTakingRequest request)
    {
        ProductViewModel productViewModel;
        using (var sr = new StreamReader($"{AppDomain.CurrentDomain.BaseDirectory}/App_Data/ProductSection.json"))
        {
            string jsonString = sr.ReadToEnd();
            productViewModel = JsonConvert.DeserializeObject<ProductViewModel>(jsonString);
        }

        var product = productViewModel.Products.SingleOrDefault(p => p.Id == request.ProductId);
        IEnumerable<PositionTakingEntity> positionTakingEntities;
        var criteria = new GetMasterPositionTakingCriteria
        {
            SettingStatus = appSettings.SettingStatus,
            PageSize = request.PageSize,
            PageIndex = request.PageIndex,
            ProductId = product.MainProductId != 0 ? product.MainProductId : request.ProductId,
            SRecommend = request.CustId
        };

        GetAgentPositionTakingCriteria agentCriteria = new();
        GetMemberPositionTakingCriteria memberCriteria = new();

        switch (request.RoleId)
        {
            case CustomerLevel.Super:
                positionTakingEntities = await GetPositionTakingEntities(criteria);

                break;

            case CustomerLevel.Master:
                agentCriteria = new()
                {
                    SettingStatus = appSettings.SettingStatus,
                    PageSize = request.PageSize,
                    PageIndex = request.PageIndex,
                    ProductId = product.MainProductId != 0 ? product.MainProductId : request.ProductId,
                    MRecommend = request.CustId
                };
                positionTakingEntities = await GetPositionTakingEntities(agentCriteria);

                break;

            default:
                memberCriteria = new()
                {
                    SettingStatus = appSettings.SettingStatus,
                    PageSize = request.PageSize,
                    PageIndex = request.PageIndex,
                    ProductId = product.MainProductId != 0 ? product.MainProductId : request.ProductId,
                    Recommend = request.CustId
                };
                positionTakingEntities = await GetPositionTakingEntities(memberCriteria);

                break;
        }

        var result = new PositionTakingViewModel
        {
            PageSizeList = GetPageSizeList(),
            PageIndex = criteria.PageIndex,
            PageSize = criteria.PageSize,
            Language = request.Language,
            CustomerInfo = new CustomerListInfo
            {
                CustId = request.ActorId,
                RoleId = request.ActorRoleId,
            },
            ViewedCustomerInfo = new CustomerListInfo
            {
                CustId = request.CustId,
                RoleId = request.RoleId
            },

            CustomerList = [],
            Product = new ProductModel
            {
                Id = criteria.ProductId,
                Name = product != null ? remotingRefService.GetResourceDataByRefIdAndResourceId("Product", product.Id.ToString(), LanguageUtils.ToLanguage(request.Language), string.Empty) :
                    ProductName(criteria.ProductId, LanguageUtils.ToLanguage(request.Language)),
                ActivationGroups = new List<ActivationGroupModel>(),
                ShowPTIcon = product.ShowPTIcon,
                CustomerSettingLink = product != null ? product.CustomerSettingLink : null,
                MainProductId = product.MainProductId,
                GroupId = request.ProductGroupId,
                IsNewSetting = NewSettingProductGroups.Contains(request.ProductGroupId)
            }
        };

        var rawData = new PositionTakingEntityList
        {
            CustPT = positionTakingEntities,
            TotalPages = request.RoleId switch {
                CustomerLevel.Super => criteria.TotalPages,
                CustomerLevel.Master => agentCriteria.TotalPages,
                _ => memberCriteria.TotalPages
            }
        };

        result.TotalPage = rawData.TotalPages;

        var sportNameDictionary = new Dictionary<int, string>();
        var liveTypeNameDictionary = new Dictionary<int, string>();
        var betTypeNameDictionary = new Dictionary<int, string>();

        switch (request.RoleId)
        {
            case CustomerLevel.Super:
                result.ViewedLevels = [request.RoleId];
                break;

            case CustomerLevel.Master:
                result.ViewedLevels = [CustomerLevel.Master, CustomerLevel.Agent];
                break;

            case CustomerLevel.Agent:
                if (result.CustomerInfo.RoleId == CustomerLevel.Super)
                {
                    result.ViewedLevels = [CustomerLevel.Super, CustomerLevel.Master, CustomerLevel.Agent];
                }
                else if (result.CustomerInfo.RoleId == CustomerLevel.Master)
                {
                    result.ViewedLevels = [CustomerLevel.Master, CustomerLevel.Agent];
                }
                else
                {
                    result.ViewedLevels = [CustomerLevel.Agent];
                }

                break;

            default:
                result.ViewedLevels = [request.RoleId];
                break;
        }

        result.IsShowUplinePT = result.ViewedLevels.Length > 1;
        CustomerPT(rawData, result, sportNameDictionary, betTypeNameDictionary, liveTypeNameDictionary, request.Language);

        return result;
    }

    private async Task<IEnumerable<PositionTakingEntity>> GetPositionTakingEntities(ICriteria criteria)
    {
        return await dynamicRepository.FetchAsync<PositionTakingEntity>(criteria);
    }

    private void CustomerPT(
            PositionTakingEntityList rawData,
            PositionTakingViewModel result,
            Dictionary<int, string> sportNameDictionary,
            Dictionary<int, string> betTypeNameDictionary,
            Dictionary<int, string> liveTypeNameDictionary,
            string languageCode)
    {
        var customerGroups = rawData.CustPT.GroupBy(custPt =>
            new
            {
                custPt.CustId,
                CustName = custPt.Username,
                custPt.Closed,
                custPt.ClosedReason,
                custPt.Suspended,
                custPt.Enabled,
                custPt.Locked,
                GroupIndex = custPt.RowNumber
            });

        var uplineLength = result.ViewedLevels.Length - 1;
        foreach (var customerGroup in customerGroups)
        {
            var customerPT = new PositionTakingModel
            {
                CustomerInfo = new CustomerListInfo
                {
                    CustId = customerGroup.Key.CustId,
                    CustName = customerGroup.Key.CustName,
                    Closed = customerGroup.Key.Closed,
                    ClosedReason = customerGroup.Key.Closed ? new CustomerClosedReason(customerGroup.Key.ClosedReason, LanguageUtils.ToLanguage(languageCode)) : null,
                    Suspended = customerGroup.Key.Suspended,
                    Enabled = customerGroup.Key.Enabled,
                    LockedAccess = new CustomerLockedAccess(customerGroup.Key.Locked, LanguageUtils.ToLanguage(languageCode))
                },
                UplinePT = new List<PositionTakingModel>(),
                GroupIndex = customerGroup.Key.GroupIndex,
                PositionTakingByGroups = SportPT(
                    result,
                    customerGroup.Where(custPT => custPT.RoleId == result.ViewedLevels[uplineLength]).ToList(),
                    sportNameDictionary,
                    betTypeNameDictionary,
                    liveTypeNameDictionary,
                    languageCode)
            };

            for (var i = 0; i < uplineLength; i++)
            {
                customerPT.UplinePT.Add(new PositionTakingModel
                {
                    CustomerInfo = new CustomerListInfo
                    {
                        CustId = customerGroup.Key.CustId,
                        CustName = customerGroup.Key.CustName,
                        Closed = customerGroup.Key.Closed,
                        ClosedReason = customerGroup.Key.Closed ? new CustomerClosedReason(customerGroup.Key.ClosedReason, LanguageUtils.ToLanguage(languageCode)) : null,
                        Suspended = customerGroup.Key.Suspended,
                        Enabled = customerGroup.Key.Enabled,
                        LockedAccess = new CustomerLockedAccess(customerGroup.Key.Locked, LanguageUtils.ToLanguage(languageCode))
                    },
                    PositionTakingByGroups = SportPT(
                        result,
                        customerGroup.Where(custPT => custPT.RoleId == result.ViewedLevels[i]).ToList(),
                        sportNameDictionary,
                        betTypeNameDictionary,
                        liveTypeNameDictionary,
                        languageCode)
                });
            }

            result.CustomerList.Add(customerPT);
        }
    }

    private List<PositionTakingByGroupModel> SportPT(
            PositionTakingViewModel result,
            List<PositionTakingEntity> customerGroup,
            Dictionary<int, string> sportNameDictionary,
            Dictionary<int, string> betTypeNameDictionary,
            Dictionary<int, string> liveTypeNameDictionary,
            string languageCode)
    {
        var sportIds = customerGroup.Select(custPT => custPT.SportType).Distinct().ToList();
        var sportPTs = new List<PositionTakingByGroupModel>();

        foreach (var sportId in sportIds)
        {
            ActivationGroupModel sportInfo = null;

            if (!sportNameDictionary.ContainsKey(sportId))
            {
                sportNameDictionary.Add(sportId, 
                    remotingRefService.GetSportName(sportId.ToString(), LanguageUtils.ToLanguage(languageCode), null) ??
                    remotingRefService.GetResourceDataByRefIdAndResourceId("DummySport", sportId.ToString(), LanguageUtils.ToLanguage(languageCode), string.Empty));
                sportInfo = new ActivationGroupModel
                {
                    Id = sportId,
                    Name = sportNameDictionary[sportId],
                    Active = true,
                    ActivationGroups = new List<ActivationGroupModel>()
                };
            }

            var sportGroup = customerGroup.Where(custPT => custPT.SportType == sportId);

            sportPTs.Add(new PositionTakingByGroupModel
            {
                Id = sportId,
                Name = sportNameDictionary[sportId],
                PositionTakingByGroups = LiveTypePT(sportInfo, sportGroup, betTypeNameDictionary, liveTypeNameDictionary, languageCode)
            });

            if (sportInfo != null)
            {
                result.Product.ActivationGroups.Add(sportInfo);
            }
        }

        return sportPTs;
    }

    private List<PositionTakingByGroupModel> LiveTypePT(
        ActivationGroupModel sportInfo,
        IEnumerable<PositionTakingEntity> sportGroup,
        Dictionary<int, string> betTypeNameDictionary,
        Dictionary<int, string> liveTypeNameDictionary,
        string languageCode)
    {
        var liveTypeIds = sportGroup.Select(custPT => custPT.IsLive).Distinct().ToList();
        var liveTypePTs = new List<PositionTakingByGroupModel>();

        foreach (var liveTypeId in liveTypeIds)
        {
            var liveType = liveTypeId ? 1 : 0;

            if (!liveTypeNameDictionary.ContainsKey(liveType))
            {
                liveTypeNameDictionary.Add(liveType, string.Empty);
            }

            ActivationGroupModel liveTypeInfo = new ActivationGroupModel
            {
                Id = liveType,
                Name = liveTypeNameDictionary[liveType],
                Active = true,
                ActivationGroups = new List<ActivationGroupModel>()
            };

            var liveTypeGroup = sportGroup.Where(custPT => custPT.IsLive == liveTypeId);
            var liveTypePT = new PositionTakingByGroupModel
            {
                Id = liveType,
                Name = liveTypeNameDictionary[liveType],
                PositionTakingByGroups = BetTypePT(liveTypeInfo, liveTypeGroup, betTypeNameDictionary, languageCode)
            };

            liveTypePTs.Add(liveTypePT);

            if (sportInfo != null)
            {
                sportInfo.ActivationGroups.Add(liveTypeInfo);
            }
        }

        return liveTypePTs;
    }

    private List<PositionTakingByGroupModel> BetTypePT(
        ActivationGroupModel liveTypeInfo,
        IEnumerable<PositionTakingEntity> liveTypeGroup,
        Dictionary<int, string> betTypeNameDictionary,
        string languageCode)
    {
        var dummySportForRNGSlot = "9906";
        var betTypeIds = liveTypeGroup.Select(custPT => custPT.BetType).Distinct().ToList();
        var betTypePTs = new List<PositionTakingByGroupModel>();

        foreach (var betTypeId in betTypeIds)
        {
            if (!betTypeNameDictionary.ContainsKey(betTypeId))
            {
                betTypeNameDictionary.Add(
                    betTypeId,
                    betTypeId == Voidbridge ?
                    remotingRefService.GetResourceDataByRefIdAndResourceId("DummySport", dummySportForRNGSlot, LanguageUtils.ToLanguage(languageCode), string.Empty) :
                    remotingRefService.GetBetTypeName(betTypeId.ToString(), LanguageUtils.ToLanguage(languageCode)));
            }

            ActivationGroupModel betTypeInfo = new ActivationGroupModel
            {
                Id = betTypeId,
                Active = true,
                Name = betTypeNameDictionary[betTypeId],
                ActivationGroups = new List<ActivationGroupModel>()
            };

            var betTypePT = liveTypeGroup.FirstOrDefault(custPT => custPT.BetType == betTypeId);
            betTypePTs.Add(new PositionTakingByGroupModel
            {
                Id = betTypeId,
                Name = betTypeNameDictionary[betTypeId],
                Value = betTypePT.PositionTaking / 100
            });

            liveTypeInfo.ActivationGroups.Add(betTypeInfo);
        }

        return betTypePTs;
    }

    private string ProductName(int productId, string language)
    {
        return Lang.ResourceManager.GetString(string.Format("{0}{1}", "product_name_", productId), new CultureInfo(language)) ?? string.Empty;
    }
}