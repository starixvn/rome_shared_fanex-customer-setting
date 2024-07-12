using Fanex.Customer.API.Modules.PositionTaking.Criterias;
using Fanex.Customer.API.Modules.PositionTaking.Models;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Data.Repository;
using Fanex.RemotingRef;
using Microsoft.Extensions.Options;

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

    public PositionTakingService(IOptions<AppSettings> appSettingOptions, IDynamicRepository dynamicRepository, IRemotingRefService remotingRefService)
    {
        this.appSettings = appSettingOptions.Value;
        this.dynamicRepository = dynamicRepository;
        this.remotingRefService = remotingRefService;
    }

    public IList<int> GetPageSizeList()
    {
        return new List<int> { 10, 50, 100, 500 };
    }

    public async Task<PositionTakingViewModel> GetPT(PositionTakingRequest request)
    {
        IEnumerable<PositionTakingEntity> positionTakingEntities;
        var criteria = new GetPositionTakingCriteriaBase
        {
            SettingStatus = appSettings.SettingStatus,
            PageSize = request.PageSize,
            PageIndex = request.PageIndex,
            ProductId = request.ProductId,
            TotalPages = request.TotalPages
        };

        switch (request.RoleId)
        {
            case CustomerLevel.Super:
                var masterCriteria = criteria as GetMasterPositionTakingCriteria;
                masterCriteria.SRecommend = request.CustId;
                positionTakingEntities = await GetPositionTakingEntities(masterCriteria);

                break;

            case CustomerLevel.Master:
                var agentCriteria = criteria as GetAgentPositionTakingCriteria;
                agentCriteria.MRecommend = request.CustId;
                positionTakingEntities = await GetPositionTakingEntities(agentCriteria);

                break;

            default:
                var memberCriteria = criteria as GetMemberPositionTakingCriteria;
                memberCriteria.Recommend = request.CustId;
                positionTakingEntities = await GetPositionTakingEntities(memberCriteria);

                break;
        }

        //var  product = ProductManager.GetProduct(criteria.ProductId);
        var result = new PositionTakingViewModel
        {
            PageSizeList = GetPageSizeList(),
            PageIndex = criteria.PageIndex,
            PageSize = criteria.PageSize,
            Language = request.Language,
            CustomerInfo = new CustomerListInfo
            {
                CustId = request.CustId,
                RoleId = request.RoleId,
            },
            ViewedCustomerInfo = new CustomerListInfo
            {
                CustId = request.CustId,
                RoleId = request.RoleId
            },

            CustomerList = new List<PositionTakingModel>(),
            Product = new ProductModel
            {
                Id = criteria.ProductId,
                // Name = product != null ? product.RemotingRefName : ProductService.ProductName(criteria.ProductId),
                // ActivationGroups = new List<ActivationGroupModel>(),
                // ShowPTIcon = criteria.ShowPTIcon,
                // CustomerSettingLink = product != null ? product.CustomerSettingLink : null,
                // MainProductId = criteria.MainProductId,
                // GroupId = criteria.ProductGroupId,
                // IsNewSetting = NewSettingProductGroups.Contains(criteria.ProductGroupId)
            }
        };

        var rawData = new PositionTakingEntityList
        {
            CustPT = positionTakingEntities,
            TotalPages = criteria.TotalPages
        };

        criteria.ProductId = request.MainProductId != 0 ? request.MainProductId : request.ProductId;
        result.TotalPage = rawData.TotalPages;

        var sportNameDictionary = new Dictionary<int, string>();
        var liveTypeNameDictionary = new Dictionary<int, string>();
        var betTypeNameDictionary = new Dictionary<int, string>();

        switch (request.RoleId)
        {
            case CustomerLevel.Super:
                result.ViewedLevels = new int[] { request.RoleId };
                break;

            case CustomerLevel.Master:
                result.ViewedLevels = new int[] { CustomerLevel.Master, CustomerLevel.Agent };
                break;

            case CustomerLevel.Agent:
                if (result.CustomerInfo.RoleId == CustomerLevel.Super)
                {
                    result.ViewedLevels = new int[] { CustomerLevel.Super, CustomerLevel.Master, CustomerLevel.Agent };
                }
                else if (result.CustomerInfo.RoleId == CustomerLevel.Master)
                {
                    result.ViewedLevels = new int[] { CustomerLevel.Master, CustomerLevel.Agent };
                }
                else
                {
                    result.ViewedLevels = new int[] { CustomerLevel.Agent };
                }

                break;

            default:
                result.ViewedLevels = new int[] { request.RoleId };
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
                    ClosedReason = customerGroup.Key.Closed ? new CustomerClosedReason(customerGroup.Key.ClosedReason) : null,
                    Suspended = customerGroup.Key.Suspended,
                    Enabled = customerGroup.Key.Enabled,
                    LockedAccess = new CustomerLockedAccess(customerGroup.Key.Locked)
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
                        ClosedReason = customerGroup.Key.Closed ? new CustomerClosedReason(customerGroup.Key.ClosedReason) : null,
                        Suspended = customerGroup.Key.Suspended,
                        Enabled = customerGroup.Key.Enabled,
                        LockedAccess = new CustomerLockedAccess(customerGroup.Key.Locked)
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
                sportNameDictionary.Add(sportId, remotingRefService.GetSportName(sportId.ToString(), languageCode));
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
                    remotingRefService.GetResourceDataByRefIdAndResourceId("DummySport", dummySportForRNGSlot, languageCode, string.Empty) :
                    remotingRefService.GetBetTypeName(betTypeId.ToString(), languageCode));
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
                Value = betTypePT.PositionTaking
            });

            liveTypeInfo.ActivationGroups.Add(betTypeInfo);
        }

        return betTypePTs;
    }
}