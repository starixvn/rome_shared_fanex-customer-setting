import { GroupTypeIds, UserLevel } from '@customer-web-app/shared-constants';
import {
    MasterDirectMemberPositionTakingGroupItem, ParlayMaxPayoutGroupItem, ProductGroup,
    ProductStatusGroupItem, SuperDirectMemberPositionTakingGroupItem
} from '@customer-web-app/shared-group-settings-types';

import { ProductGroupsAction, ProductGroupsActionTypes } from './product-groups.actions';

export interface GroupSettingsState {
  productGroups: ProductGroup[];
}

export const initialState: GroupSettingsState = {
  productGroups: [],
};

const initProductGroups = (productGroups: ProductGroup[]) => {
  return productGroups.map((group) => {
    const groupTypes = group.groupTypes
      ?.filter((t) => t !== null)
      .map((type) => {
        switch (type.id) {
          case GroupTypeIds.ParlayMaxPayout: {
            const groupItems = (type.groupItems as ParlayMaxPayoutGroupItem[]).map((item) => ({
              ...item,
              isMultipleModeChecked: true,
              original: { ...item },
              isAllowUnlimitCombo2: item.combo2 === 0,
              isAllowUnlimitCombo35: item.combo35 === 0,
              isAllowUnlimitComboOver6: item.comboOver6 === 0,
              isAllowUnlimitSingleMatchParlay: item.singleMatchParlay === 0,
            }));

            return { ...type, groupItems };
          }
          case GroupTypeIds.ProductStatus: {
            const groupItems = (type.groupItems as ProductStatusGroupItem[]).map((item) => {
              if (item.parentSportType && item.parentBetType) {
                const parentItem = type.groupItems.find(
                  (x) => x.sportTypeId === item.parentSportType && x.betTypeId === item.parentBetType
                ) as ProductStatusGroupItem;

                if (parentItem.isDisabled) {
                  return { ...item, isMultipleModeChecked: true, original: { ...item }, readonly: true };
                }
              }

              return { ...item, isMultipleModeChecked: true, original: { ...item } };
            });

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

export function productGroupsReducer(
  state: GroupSettingsState = initialState,
  action: ProductGroupsAction
): GroupSettingsState {
  switch (action.type) {
    case ProductGroupsActionTypes.Init: {
      const productGroups = initProductGroups(action.payload);

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
