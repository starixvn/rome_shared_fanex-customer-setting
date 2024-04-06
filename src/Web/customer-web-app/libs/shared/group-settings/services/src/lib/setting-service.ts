import { CreateCustomerResult, Result, UpdateCustomerResult } from '@customer-web-app/share-models';
import { axiosInstance, getHeadersForPostRequest } from '@customer-web-app/shared-api-helper';
import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { GeneralInfo } from '@customer-web-app/shared-general-information-state';
import { CurrentWinLossAmount, ProductGroup } from '@customer-web-app/shared-group-settings-types';

export interface GroupSettingsResponse {
  generalInfo: GeneralInfo;
  productGroups: ProductGroup[];
}

export const SettingService = {
  GetGlobalSetting: async (siteLabel: string, site: string, currency: string): Promise<GroupSettingsResponse> => {
    const response = await axiosInstance.get<GroupSettingsResponse>(
      `Customer/GetGlobalSetting?siteLabel=${siteLabel}&site=${site}&currency=${currency}`
    );
    const { generalInfo, productGroups } = response.data;

    const newProductGroups = productGroups.map((group) => {
      const groupTypes = group.groupTypes.map((type) => {
        if (type.id === GroupTypeIds.ParlayMaxPayout) {
          const groupItems = type.groupItems.map((item: any) => ({
            ...item,
            isMultipleModeChecked: true,
            upline: { ...item },
            original: {
              ...item,
            },
            isAllowUnlimitCombo2: item.combo2 === 0,
            isAllowUnlimitCombo35: item.combo35 === 0,
            isAllowUnlimitComboOver6: item.comboOver6 === 0,
            isAllowUnlimitSingleMatchParlay: item.singleMatchParlay === 0,
          }));
          return { ...type, groupItems };
        }
        const groupItems = type.groupItems.map((item) => ({
          ...item,
          isMultipleModeChecked: true,
          upline: { ...item },
        }));
        return { ...type, groupItems };
      });
      return { ...group, groupTypes };
    });

    return { generalInfo, productGroups: newProductGroups };
  },
  CheckExistedCustomer: async (userName: string): Promise<boolean> => {
    const response = await axiosInstance.get<boolean>(`Customer/CheckExistedCustomer?userName=${userName}`);
    return response.data;
  },
  GetCustomerSetting: async (
    custId: number,
    roleId: number,
    uplineRoleId = 0,
    isGetUpLine = false,
    siteLabel = ''
  ): Promise<GroupSettingsResponse> => {
    const response = await axiosInstance.get<GroupSettingsResponse>(
      `Customer/GetCustomerSetting?custId=${custId}&roleId=${roleId}&uplineRoleId=${uplineRoleId}&isGetUpline=${isGetUpLine}&systemId=${
        appConfig.systemId
      }&siteLabel=${siteLabel}&productGroupId=${params.productGroupId ?? ProductGroupIds.SportsBook}`
    );
    return response.data;
  },
  CreateCustomer: async (generalInfo: GeneralInfo, productGroups: ProductGroup[]) => {
    const { custId, ...otherInfo } = generalInfo;
    return await axiosInstance
      .post<CreateCustomerResult>(
        'Customer/Create',
        {
          generalInfo: { ...otherInfo },
          productGroups: JSON.stringify(productGroups),
        },
        {
          headers: getHeadersForPostRequest(),
        }
      )
      .then((response) => response.data)
      .catch((error) => {
        if (error.response?.status === 401 || error.response?.status === 403) {
          return Result.UnAuthorizedResult;
        }

        // Something unexpected
        throw error;
      });
  },
  UpdateCustomer: (
    generalInfo: GeneralInfo | null,
    productGroups: ProductGroup[] | [],
    custId: number,
    roleId?: number,
    upLineRoleId?: number
  ) =>
    axiosInstance
      .post<UpdateCustomerResult>(
        'Customer/Update',
        {
          custId,
          roleId,
          upLineRoleId,
          generalInfo,
          productGroups: JSON.stringify(productGroups),
          productGroupId: params.productGroupId,
        },
        {
          headers: getHeadersForPostRequest(),
        }
      )
      .then((response) => response.data)
      .catch((error) => {
        if (error.response?.status === 401 || error.response?.status === 403) {
          return Result.UnAuthorizedResult;
        }

        // Something unexpected
        throw error;
      }),
  UpdateMultipleCustomers: (
    productGroups: ProductGroup[] | [],
    custId: number,
    customerIds: string,
    customerNames: string,
    roleId: number,
    uplineRoleId: number
  ) =>
    axiosInstance
      .post<UpdateCustomerResult[]>(
        'Customer/UpdateMultipleCustomers',
        {
          custId,
          roleId,
          uplineRoleId,
          customerIds,
          customerNames,
          productGroups: JSON.stringify(productGroups),
          productGroupId: params.productGroupId,
        },
        {
          headers: getHeadersForPostRequest(),
        }
      )
      .then((response) => response.data)
      .catch((error) => {
        if (error.response?.status === 401 || error.response?.status === 403) {
          return Result.UnAuthorizedResult;
        }

        // Something unexpected
        throw error;
      }),
  UpdateSuperBetSetting: (productGroups: ProductGroup[] | [], custId: number, roleId: number, uplineRoleId: number) =>
    axiosInstance
      .post<UpdateCustomerResult>(
        'Customer/EditSuperBetSetting',
        {
          custId,
          roleId,
          uplineRoleId,
          productGroups: JSON.stringify(productGroups),
          productGroupId: params.productGroupId,
        },
        {
          headers: getHeadersForPostRequest(),
        }
      )
      .then((response) => response.data)
      .catch((error) => {
        if (error.response?.status === 401 || error.response?.status === 403) {
          return Result.UnAuthorizedResult;
        }

        // Something unexpected
        throw error;
      }),
  UpdateMultipleSuperBetSetting: (
    productGroups: ProductGroup[] | [],
    custId: number,
    customerIds: string,
    customerNames: string,
    roleId: number,
    uplineRoleId: number
  ) =>
    axiosInstance
      .post<UpdateCustomerResult>(
        'Customer/EditMultipleSuperBetSetting',
        {
          custId,
          roleId,
          customerIds,
          customerNames,
          uplineRoleId,
          productGroups: JSON.stringify(productGroups),
          productGroupId: params.productGroupId,
        },
        {
          headers: getHeadersForPostRequest(),
        }
      )
      .then((response) => response.data)
      .catch((error) => {
        if (error.response?.status === 401 || error.response?.status === 403) {
          return Result.UnAuthorizedResult;
        }

        // Something unexpected
        throw error;
      }),
  GetCustomerCurrentWinLossAmount: async (custId: number, sportType: number, betType: number) => {
    const response = await axiosInstance.get<CurrentWinLossAmount[]>(
      `Customer/GetCurrentWinLossAmount?custId=${custId}&sportType=${sportType}&betType=${betType}`
    );
    return response.data;
  },
};
