import { GroupTypeIds, UserLevel } from '@customer-web-app/shared-constants';
import {
    GroupType, MasterDirectMemberPositionTakingGroupItem, ParlayMaxPayoutGroupItem, ProductGroup,
    ProductStatusGroupItem, SuperDirectMemberPositionTakingGroupItem, WinLossLimitGroupItem
} from '@customer-web-app/shared-group-settings-types';

import { ProductGroupsAction, ProductGroupsActionTypes } from './product-groups.actions';

export interface GroupSettingsState {
  productGroups: ProductGroup[];
}

export const initialState: GroupSettingsState = {
  productGroups: [],
};

export const initProductStatusGroup = (groupType: GroupType) => {
  const groupItems = (groupType.groupItems as ProductStatusGroupItem[]).map((item) => {
    if (item.parentSportType && item.parentBetType) {
      const parentItem = groupType.groupItems.find(
        (x) => x.sportTypeId === item.parentSportType && x.betTypeId === item.parentBetType
      ) as ProductStatusGroupItem;

      if (parentItem.isDisabled) {
        return { ...item, isMultipleModeChecked: true, original: { ...item }, readonly: true };
      }
    }

    return { ...item, isMultipleModeChecked: true, original: { ...item } };
  });

  return { ...groupType, groupItems };
};

const initProductGroupsForCreate = (productGroups: ProductGroup[]) => {
  return productGroups.map((group) => {
    const groupTypes = group.groupTypes
      ?.filter((t) => t !== null)
      .map((type) => {
        switch (type.id) {
          case GroupTypeIds.ParlayMaxPayout: {
            const groupItems = (type.groupItems as ParlayMaxPayoutGroupItem[]).map((item) => {
              const original = { ...item };
              const upline = { ...item };
              const currentItem = { ...item };
              original.isAllowUnlimitSingleMatchParlay = false;
              original.isAllowUnlimitCombo2 = false;
              original.isAllowUnlimitCombo35 = false;
              original.isAllowUnlimitComboOver6 = false;
              upline.combo2 = item.isAllowUnlimitCombo2 ? 0 : upline.combo2;
              upline.singleMatchParlay = item.isAllowUnlimitSingleMatchParlay ? 0 : upline.singleMatchParlay;
              upline.combo35 = item.isAllowUnlimitCombo35 ? 0 : upline.combo35;
              upline.comboOver6 = item.isAllowUnlimitComboOver6 ? 0 : upline.comboOver6;
              currentItem.isAllowUnlimitSingleMatchParlay = false;
              currentItem.isAllowUnlimitCombo2 = false;
              currentItem.isAllowUnlimitCombo35 = false;
              currentItem.isAllowUnlimitComboOver6 = false;
              return {
                ...currentItem,
                isMultipleModeChecked: true,
                upline,
                original,
              };
            });

            return { ...type, groupItems };
          }
          case GroupTypeIds.ProductStatus: {
            return initProductStatusGroup({ ...type });
          }
          case GroupTypeIds.WinLossLimit: {
            const groupItems = (type.groupItems as WinLossLimitGroupItem[]).map((item) => ({
              ...item,
              isMultipleModeChecked: true,
              original: { ...item },
              lossLimitValue: item.lossLimit !== 0 ? item.lossLimit : undefined,
            }));

            return { ...type, groupItems };
          }
          case GroupTypeIds.PositionTaking: {
            if (params.uplineRoleId === UserLevel.Super) {
              const groupItems = (type.groupItems as SuperDirectMemberPositionTakingGroupItem[]).map((item) => ({
                ...item,
                isMultipleModeChecked: true,
                s1: item.s4,
                upline: { ...item },
              }));
              return { ...type, groupItems };
            }

            const groupItems = (type.groupItems as MasterDirectMemberPositionTakingGroupItem[]).map((item) => ({
              ...item,
              isMultipleModeChecked: true,
              m1: item.m3,
              upline: { ...item },
            }));

            return { ...type, groupItems };
          }
          default: {
            const groupItems = type.groupItems.map((item) => ({
              ...item,
              isMultipleModeChecked: true,
              upline: { ...item },
            }));

            return { ...type, groupItems };
          }
        }
      });

    return { ...group, groupTypes };
  });
};

const handleInitProductsForEdit = (productGroups: ProductGroup[], isMemberLevel: boolean) => {
  return productGroups?.map((group) => {
    const groupTypes = group.groupTypes
      .filter((t) => t != null)
      .map((type) => {
        if (type.id === GroupTypeIds.PositionTaking) {
          return {
            ...type,
            groupItems: type.groupItems.map((item) => ({
              ...item,
              isMultipleModeChecked: true,
              upline: { ...item },
              original: { ...item },
            })),
          };
        }

        if (type.id === GroupTypeIds.WinLossLimit) {
          const groupItems = (type.groupItems as WinLossLimitGroupItem[]).map((item) => ({
            ...item,
            isMultipleModeChecked: true,
            original: { ...item },
            lossLimitValue: item.lossLimit !== 0 ? item.lossLimit : undefined,
          }));

          return { ...type, groupItems };
        }

        if (type.id === GroupTypeIds.ParlayMaxPayout) {
          const groupItems = (type.groupItems as ParlayMaxPayoutGroupItem[]).map((item) => ({
            ...item,
            isMultipleModeChecked: true,
            original: {
              ...item,
              isAllowUnlimitCombo2: isMemberLevel ? false : item.isAllowUnlimitCombo2,
              isAllowUnlimitCombo35: isMemberLevel ? false : item.isAllowUnlimitCombo35,
              isAllowUnlimitComboOver6: isMemberLevel ? false : item.isAllowUnlimitComboOver6,
              isAllowUnlimitSingleMatchParlay: isMemberLevel ? false : item.isAllowUnlimitSingleMatchParlay,
            },
            isAllowUnlimitCombo2: item.combo2 === 0,
            isAllowUnlimitCombo35: item.combo35 === 0,
            isAllowUnlimitComboOver6: item.comboOver6 === 0,
            isAllowUnlimitSingleMatchParlay: item.singleMatchParlay === 0,
          }));

          return { ...type, groupItems };
        }

        if (type.id === GroupTypeIds.ProductStatus) {
          return initProductStatusGroup({ ...type });
        }

        return {
          ...type,
          groupItems: type.groupItems.map((item) => ({
            ...item,
            original: { ...item },
            isMultipleModeChecked: true,
          })),
        };
      });

    return { ...group, groupTypes };
  });
};

export function productGroupsReducer(
  state: GroupSettingsState = initialState,
  action: ProductGroupsAction
): GroupSettingsState {
  switch (action.type) {
    case ProductGroupsActionTypes.InitCreate: {
      const productGroups = initProductGroupsForCreate(action.payload);

      return { ...state, productGroups };
    }
    case ProductGroupsActionTypes.InitEdit: {
      const productGroups = handleInitProductsForEdit(action.payload, true);
      return { ...state, productGroups };
    }
    case ProductGroupsActionTypes.Update: {
      return { ...state, productGroups: action.payload };
    }
    default: {
      return state;
    }
  }
}
