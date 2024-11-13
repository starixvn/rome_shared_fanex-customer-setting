using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Fanex.Customer.Client.Configurations;
using Fanex.Customer.Client.Constants;
using Fanex.Customer.Client.Extensions;
using Fanex.Customer.Client.Models;
using Fanex.Customer.Client.Params;

namespace Fanex.Customer.Client.Services
{
    public interface ICustomerService
    {
        Task<bool> IsCustomerEnabled(int custId);

        Task<CreditInfo> GetCreditInfo(string username);

        Task<bool> CheckExistedCustomer(string username);

        Task<string> GetSuggestUsername(string actorName, byte roleId);

        Task<string> GetSuggestDirectMemberUsername(string uplineUsername);

        Task<IEnumerable<SuggestDirectMember>> SearchSuggestDirectMembers(int uplineCustId, string searchUserName);

        Task<int> CheckDirectMemberUserName(int uplineCustId, string userName);

        Task<Downline> GetDownlineByUsername(int custId, int subAccountId, string username);

        Task<CopyCustomerResult> CopyCustomer(CopyCustomerParam param);

        Task<CopyMultipleCustomersResult> CopyMultipleCustomers(CopyMultipleCustomersParam param);

        Task<Result> CheckAllLimitedRules(int custId, byte custLevel, string actorUsername, string language = Languages.English);

        Task<string> GetGlobalSetting(string siteLabel, string site, int currency, int systemId, int productGroupId = 1, string language = Languages.English);

        Task<string> GetCustomerSetting(int custId, byte roleId, int systemId, byte uplineRoleId = 0, string language = Languages.English, bool isGetUpline = false, int productGroupId = 1, string siteLabel = "", bool isMultipleMode = false, string groupName="", bool isCopy=false);

        Task<string> CreateCustomer(CreateCustomerParam param);

        Task<string> UpdateCustomer(UpdateCustomerParam param);

        Task<string> UpdateMultipleCustomers(UpdateMultipleCustomerParam param);

        Task<string> GetPTTemplates(int custId);

        Task<string> CreatePTTemplate(CreatePTTemplateParam param);

        Task<string> GetPTSettings(int custId, byte roleId, string site, int currency, int systemId, string language = Languages.English);

        Task<string> EditPTTemplate(EditPTTemplateParam param);

        Task<string> DeletePTTemplate(DeletePTTemplateParam param);

        Task<CustomerInfo> GetCustomerInfo(int custId);

        Task<Result> UpdateCustomerInfo(UpdateCustomerInfoParam param);

        Task<string> GetCustomerCurrentWinLossAmounts(int custId, int sportType, int betType, string language = "en-US");

        Task<string> GetPTSettingInformation(int? custId, bool isTest, string site, string language = "en-US");

        Task<string> GetProductCategories(int? custId, string site, string siteLabel, int currency, string language = "en-US");

        Task<string> GetCommissionHint(int custId, string site, string language = "en-US");

        Task<string> GetMaxPayoutSettingInformation(string siteName, string siteLabel, string language = "en-US");

        Task<List<UpdateCustomerResult>> CopySupersWinLossLimit(CopySupersWinLossLimitParam param);

        Task<AccountInfo> GetAccountInfo(int custId);
        Task<string> GetWinLossLimitInformation(int? custId, string site, string siteLabel, int currency, string language = "en-US");
    }

    public class CustomerService : ICustomerService
    {
        private const int DefaultTimeOut = 10;
        public readonly HttpClient httpClient;
        private readonly CustomerClientConfiguration configuration;

        public CustomerService(CustomerClientConfiguration configuration = null)
        {
            this.configuration = configuration ?? CustomerClientManager.ClientConfigs;
            httpClient = new HttpClient
            {
                BaseAddress = new Uri(this.configuration.ServiceUri),
                Timeout = TimeSpan.FromMinutes(DefaultTimeOut)
            };
        }

        public Task<Result> CheckAllLimitedRules(int custId, byte custLevel, string actorUsername, string language = Languages.English)
            => httpClient.PostAsJsonAsync<Result>("rules/CheckAllLimitedRules", new { custId, custLevel, actorUsername, language });

        public Task<bool> CheckExistedCustomer(string username)
            => httpClient.GetAsJsonAsync<bool>($"customer/{username}/exist");

        public Task<CopyCustomerResult> CopyCustomer(CopyCustomerParam param)
        {
            param.SystemId = configuration.SystemId;
            return httpClient.PostAsJsonAsync<CopyCustomerResult>("customer/copy-single", param);
        }

        public Task<CopyMultipleCustomersResult> CopyMultipleCustomers(CopyMultipleCustomersParam param)
        {
            param.SystemId = configuration.SystemId;
            return httpClient.PostAsJsonAsync<CopyMultipleCustomersResult>("customer/copy-multiple", param);
        }

        public Task<string> CreateCustomer(CreateCustomerParam param)
            => httpClient.PostAsStringAsync("customer", new
            {
                context = new
                {
                    creatorId = param.CreatorId,
                    subAccId = param.SubAccId,
                    ip = param.IP,
                    systemId = configuration.SystemId,
                    language = param.Language,
                    mode = param.Mode,
                    custDBGroupId = param.CustDBGroupID,
                    site = param.Site,
                    siteLabel = param.SiteLabel,
                    uiBehavior = !string.IsNullOrWhiteSpace(param.UiBehavior) ? JsonNode.Parse(param.UiBehavior) : null
                },
                settings = new
                {
                    group = param.Group,
                    generalInfo = param.GeneralInfo,
                    productGroups = JsonNode.Parse(param.ProductGroups)
                }
            });

        public Task<string> UpdateCustomer(UpdateCustomerParam param)
            => httpClient.PutAsStringAsync("customer", new
            {
                context = new
                {
                    actorId = param.ActorId,
                    subAccId = param.SubAccId,
                    custId = param.CustId,
                    userName = param.UserName,
                    roleId = param.RoleId,
                    uplineRoleId = param.UplineRoleId,
                    ip = param.Ip,
                    custDBGroupId = param.CustDBGroupID,
                    site = param.Site,
                    siteLabel = param.SiteLabel,
                    systemId = configuration.SystemId,
                    language = param.Language
                },
                settings = new
                {
                    group = param.Group,
                    generalInfo = param.GeneralInfo,
                    productGroups = JsonNode.Parse(param.ProductGroups)
                }
            });

        public Task<string> UpdateMultipleCustomers(UpdateMultipleCustomerParam param)
            => httpClient.PutAsStringAsync("customers", new
            {
                context = new
                {
                    actorId = param.ActorId,
                    subAccId = param.SubAccId,
                    custId = param.CustId,
                    userName = param.UserName,
                    roleId = param.RoleId,
                    uplineRoleId = param.UplineRoleId,
                    ip = param.Ip,
                    custDBGroupId = param.CustDBGroupID,
                    site = param.Site,
                    siteLabel = param.SiteLabel,
                    systemId = configuration.SystemId,
                    language = param.Language,
                    customerIds = param.CustomerIds,
                    customerNames = param.CustomerNames
                },
                settings = new
                {
                    group = param.Group,
                    productGroups = JsonNode.Parse(param.ProductGroups)
                }
            });


        public Task<string> CreatePTTemplate(CreatePTTemplateParam param)
            => httpClient.PostAsStringAsync("customer/pt-templates", param);

        public Task<string> DeletePTTemplate(DeletePTTemplateParam param)
            => httpClient.PutAsStringAsync("customer/pt-templates/delete", param);

        public Task<string> EditPTTemplate(EditPTTemplateParam param)
            => httpClient.PutAsStringAsync("customer/pt-templates", param);

        public Task<CreditInfo> GetCreditInfo(string username)
            => httpClient.GetAsJsonAsync<CreditInfo>($"customer/{username}/credit");

        public Task<CustomerInfo> GetCustomerInfo(int custId)
            => httpClient.GetAsJsonAsync<CustomerInfo>($"customer/{custId}");

        public Task<Result> UpdateCustomerInfo(UpdateCustomerInfoParam param)
            => httpClient.PutAsJsonAsync<Result>("customer/info", param);

        public Task<string> GetCustomerCurrentWinLossAmounts(int custId, int sportType, int betType, string language = "en-US")
            => httpClient.GetStringAsync($"customer/current-winloss-amount?custId={custId}&sportType={sportType}&betType={betType}&language={language}");

        public Task<string> GetCustomerSetting(int custId, byte roleId, int systemId, byte uplineRoleId = 0, string language = Languages.English, bool isGetUpline = false, int productGroupId = 1, string siteLabel = "", bool isMultipleMode = false, string groupName="", bool isCopy = false)
            => httpClient.GetStringAsync($"customer?CustId={custId}&RoleId={roleId}&SystemId={systemId}&UplineRoleId={uplineRoleId}&Language={language}&IsGetUpline={isGetUpline}&ProductGroupId={productGroupId}&SiteLabel={siteLabel}&IsMultipleMode={isMultipleMode}&GroupName={groupName}&IsCopy={isCopy}");

        public Task<Downline> GetDownlineByUsername(int custId, int subAccountId, string username)
            => httpClient.GetAsJsonAsync<Downline>($"customer/{custId}/{subAccountId}/downlines/{username}");

        public Task<string> GetGlobalSetting(string siteLabel, string site, int currency, int systemId, int productGroupId = 0, string language = "en-US")
            => httpClient.GetStringAsync($"customer/globalsetting?siteLabel={siteLabel}&site={site}&systemId={systemId}&currency={currency}&productGroupId={productGroupId}&language={language}");

        public Task<string> GetPTSettings(int custId, byte roleId, string site, int currency, int systemId, string language = Languages.English)
            => httpClient.GetStringAsync($"customer/pt-settings?custId={custId}&roleId={roleId}&site={site}&systemId={systemId}&currency={currency}&language={language}");

        public Task<string> GetPTTemplates(int custId)
            => httpClient.GetStringAsync($"customer/{custId}/pt-templates");

        public Task<string> GetPTSettingInformation(int? custId, bool isTest, string site, string language = "en-US")
        {
            if (custId.HasValue)
            {
                return httpClient.GetStringAsync($"customer/pt-setting-information?custId={custId}&site={site}&isTest={isTest}&language={language}");
            }
            return httpClient.GetStringAsync($"customer/pt-setting-information?site={site}&isTest={isTest}&language={language}");
        }

        public Task<string> GetProductCategories(int? custId, string site, string siteLabel, int currencyId, string language = "en-US")
        {
            if (custId.HasValue)
            {
                return httpClient.GetStringAsync($"customer/product-categories?custId={custId}&site={site}&sitelabel={siteLabel}&currencyId={currencyId}&language={language}");
            }
            return httpClient.GetStringAsync($"customer/product-categories?&site={site}&sitelabel={siteLabel}&currencyId={currencyId}&language={language}");
        }

        public Task<string> GetMaxPayoutSettingInformation(string siteName, string siteLabel, string language = "en-US")
            => httpClient.GetStringAsync($"customer/max-payout-setting-information?siteName={siteName}&siteLabel={siteLabel}&language={language}");

        public Task<string> GetCommissionHint(int custId, string site, string language = "en-US")
            => httpClient.GetStringAsync($"customer/commission-hint?custId={custId}&site={site}&language={language}");

        public Task<string> GetSuggestUsername(string actorName, byte roleId)
            => httpClient.PostAsJsonAsync<string>("customer/suggest-username", new { actorName, roleId });

        public Task<string> GetSuggestDirectMemberUsername(string uplineUsername)
            => httpClient.GetAsJsonAsync<string>($"customer/{uplineUsername}/suggest-direct-member-username");

        public Task<IEnumerable<SuggestDirectMember>> SearchSuggestDirectMembers(int uplineCustId, string searchUserName)
            => httpClient.GetAsJsonAsync<IEnumerable<SuggestDirectMember>>($"api/customer/{uplineCustId}/suggest-direct-members/{searchUserName}");

        public Task<int> CheckDirectMemberUserName(int uplineCustId, string userName)
            => httpClient.GetAsJsonAsync<int>($"api/customer/{uplineCustId}/check-exist-direct-member/{userName}");

        public Task<bool> IsCustomerEnabled(int custId)
            => httpClient.GetAsJsonAsync<bool>($"customer/{custId}/IsEnabled");

        public Task<List<UpdateCustomerResult>> CopySupersWinLossLimit(CopySupersWinLossLimitParam param)
            => httpClient.PutAsJsonAsync<List<UpdateCustomerResult>>("customer/copy-supers-winloss-limit", param);

        public Task<AccountInfo> GetAccountInfo(int custId)
            => httpClient.GetAsJsonAsync<AccountInfo>($"account/{custId}");

        public Task<string> GetWinLossLimitInformation(int? custId, string site, string siteLabel, int currencyId, string language = "en-US")
        {
            if (custId.HasValue)
            {
                return httpClient.GetStringAsync($"customer/win-loss-limit?custId={custId}&site={site}&sitelabel={siteLabel}&currencyId={currencyId}&language={language}");
            }
            
            return httpClient.GetStringAsync($"customer/win-loss-limit?&site={site}&sitelabel={siteLabel}&currencyId={currencyId}&language={language}");
        }
    }
}