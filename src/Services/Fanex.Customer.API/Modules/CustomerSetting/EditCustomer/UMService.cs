using Fanex.Common.Helper;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.EditCustomer.Criteria;
using Fanex.Customer.API.Modules.EditCustomer.Models;
using Fanex.Customer.API.Modules.Message.Models;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.EditCustomer;

public interface IUMService
    {
        CustomerSettingResult GetUMResult(int custId, int groupId, IEnumerable<int> updatedSportTypes, string language = "en-US");
    }
public class UMService : IUMService
    {
        private readonly IDynamicRepository _dynamicRepository;
        private readonly ICacheRefService _cacheRefService;
        private readonly AppSettings _appSettings;
        private const string UMDateTimeFormat = "dd/MM/yyyy HH:mm:ss";
        public const int SuccessButDoNothingBecauseUM = 90017;

        public UMService(IDynamicRepository dynamicRepository, ICacheRefService cacheRefService, IOptions<AppSettings> appSettings)
        {
            _dynamicRepository = dynamicRepository;
            _cacheRefService= cacheRefService;
            _appSettings = appSettings.Value; 
        }

        public CustomerSettingResult GetUMResult(int custId, int groupId, IEnumerable<int> updatedSportTypes, string language = "en-US")
            => ComposeUMMessage(_dynamicRepository.Fetch<UnderMaintainanceInfo>(new UnderMaintainanceQueryCriteria
            {
                CustId = custId,
                SettingStatus = _appSettings.SettingStatus,
                GroupId = groupId
            }), updatedSportTypes, language);

        private CustomerSettingResult ComposeUMMessage(IEnumerable<UnderMaintainanceInfo> UMInfos, IEnumerable<int> updatedSportTypes, string language)
        {
            Check.Argument.IsNotNull(UMInfos, "UMInfos");
            var UMSubMessages = new List<SubMessageContent>();
            foreach (var UMInfo in UMInfos)
            {
                if (UMInfo.SportType != 0 && !updatedSportTypes.Contains(UMInfo.SportType))
                { 
                    continue;
                }

                if (UMInfo.ProductId == 0)
                {
                    UMSubMessages.Add(new SubMessageContent(string.Format(
                        StaticResources.Get("UMSubMsg", language), 
                        StaticResources.Get("AllProducts", language), 
                        StaticResources.Get("CustomerSettings", language), 
                        UMInfo.BeginTime?.ToString(UMDateTimeFormat), 
                        UMInfo.EndTime?.ToString(UMDateTimeFormat))));
                }
                else
                {
                    var productSportTypeNamePart = UMInfo.SportType == 0 ? _cacheRefService.GetProductName(UMInfo.ProductId, language) : _cacheRefService.GetSportTypeName(UMInfo.SportType, language);
                    var settingTypePart = UMInfo.SettingType == 0 ? StaticResources.Get("CustomerSettings", language) : StaticResources.Get($"SettingType_{UMInfo.SettingType}", language);
                    UMSubMessages.Add(new SubMessageContent(string.Format(
                        StaticResources.Get("UMSubMsg", language), 
                        productSportTypeNamePart, 
                        settingTypePart, 
                        UMInfo.BeginTime?.ToString(UMDateTimeFormat),
                        UMInfo.EndTime?.ToString(UMDateTimeFormat))));
                }
            }

            return UMSubMessages.Count == 0 ? null : new CustomerSettingResult(SuccessButDoNothingBecauseUM, StaticResources.Get("UMMainMsg", language), UMSubMessages);
        }
    }
