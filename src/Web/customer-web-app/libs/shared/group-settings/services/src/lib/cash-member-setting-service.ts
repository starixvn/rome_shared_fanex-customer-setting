

import { UsernameSuggestion } from '@customer-web-app/share-models';
import { axiosInstance } from '@customer-web-app/shared-api-helper';
import axios from 'axios';
import { GroupSettingsResponse } from './setting-service';
import { ProductGroupIds } from '@customer-web-app/shared-constants';

export const CashMemberSettingService = {
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
    GetBanks: async () => {
        const response = await axios.get("/cash-service/Payments/GetBanksByTransactionType", {
            baseURL: "http://localhost:5100",
        });
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
          `Customer/GetCashCustomerSetting?custId=${custId}&roleId=${roleId}&uplineRoleId=${uplineRoleId}&isGetUpline=${isGetUpLine}&systemId=${
            appConfig.systemId
          }&siteLabel=${siteLabel}&productGroupId=${
            params.productGroupId ?? ProductGroupIds.SportsBook
          }&isMultipleMode=${isMultipleMode}&groupName=${groupName}&isCopy=${isCopy}&agentId=${agentId}`
        );
        return response.data;
      },
};