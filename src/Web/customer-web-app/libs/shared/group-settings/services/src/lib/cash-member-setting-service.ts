import { CashServiceStatus, CreateCustomerResult, UsernameSuggestion } from '@customer-web-app/share-models';
import { axiosInstance, getHeadersForPostRequest } from '@customer-web-app/shared-api-helper';
import { GroupSettingsResponse } from './setting-service';
import { ProductGroupIds } from '@customer-web-app/shared-constants';
import { GeneralInfo } from '@customer-web-app/shared-general-information-state';
import { ProductGroup } from '@customer-web-app/shared-group-settings-types';

export const CashMemberSettingService = {
  EnableCashService: async () => {
    const response = await axiosInstance.post(
      `Customer/EnableCashService`,
      {
        headers: getHeadersForPostRequest(),
      }
    );
    return response.data;
  },
  GetCashServiceStatus: async () => {
    const response = await axiosInstance.get<CashServiceStatus>(
      `Customer/GetCashServiceStatus`,
    );
    return response.data;
  },
  UpdateCashServiceStatus: async (status: boolean) => {
    const response = await axiosInstance.post(
      `Customer/UpdateCashServiceStatus?status=${status}`,
    );
    return response.data;
  },

  GetCashMemberUserNameSuggestion: async (query: string) => {
    const response = await axiosInstance.get<UsernameSuggestion[]>(
      `Customer/SearchSuggestCashMembers?searchUsername=${query}&uplineCustId=${params.uplineId}`
    );
    return response.data;
  },
  GetCashMember: async (username: string) => {
    const response = await axiosInstance.get(
      `Customer/CheckCashMemberUserName?searchUsername=${username}&uplineCustId=${params.uplineId}`
    );
    return response.data;
  },
  GetCountryCodes: async () => {
    const response = await axiosInstance.get(
      `Customer/GetCountryCodes`
    );
    return response.data;
  },
   GetBanks: async () => {
    const response = await axiosInstance.get(
      `Customer/GetBanks`
    );
    return response.data;
  },
  GetCashCustomerSetting: async (
    custId: number,
    roleId: number,
    uplineRoleId = 0,
    agentId = 0,
    isGetUpLine = false,
    siteLabel = '',
    isMultipleMode = false,
    groupName = '',
    isCopy = false
  ): Promise<GroupSettingsResponse> => {
    const response = await axiosInstance.get<GroupSettingsResponse>(
      `Customer/GetCashCustomerSetting?custId=${custId}&roleId=${roleId}&uplineRoleId=${uplineRoleId}&isGetUpline=${isGetUpLine}&systemId=${appConfig.systemId
      }&siteLabel=${siteLabel}&productGroupId=${params.productGroupId ?? ProductGroupIds.SportsBook
      }&isMultipleMode=${isMultipleMode}&groupName=${groupName}&isCopy=${isCopy}&agentId=${agentId}`
    );
    return response.data;
  },
  CreateCashMember: async (
    generalInfo: GeneralInfo,
    group: string,
    productGroups: ProductGroup[],
    isDirectMember = false,
    numberOfAccounts = 1
  ) => {
    const { custId, ...otherInfo } = generalInfo;
    return await axiosInstance
      .post<CreateCustomerResult>(
        'Customer/CreateCashCustomer',
        {
          generalInfo: { ...otherInfo },
          group,
          productGroups: JSON.stringify(productGroups),
          isDirectMember,
          numberOfAccounts,
        },
        {
          headers: getHeadersForPostRequest(),
        }
      )
      .then((response) => response.data)
      .catch((error) => {
        if (error.response?.status === 401 || error.response?.status === 403) {
          return CreateCustomerResult.UnAuthorizedResult;
        }

        // Something unexpected
        throw error;
      });
  },
};