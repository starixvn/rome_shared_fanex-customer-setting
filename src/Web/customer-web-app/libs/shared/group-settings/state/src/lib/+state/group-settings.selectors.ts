import { GroupTypeIds, UserLevel } from '@customer-web-app/shared-constants';
import {
  AgentPositionTakingGroupItem,
  BetLimitGroupItem,
  CommissionGroupItem,
  MasterPositionTakingGroupItem,
  MemberPositionTakingGroupItem,
  ParlayMaxPayoutGroupItem,
  ProductGroup,
  ProductStatusGroupItem,
  SportsBookBetSettingGroupItem,
  SuperPositionTakingGroupItem,
  WinLossLimitGroupItem,
} from '@customer-web-app/shared-group-settings-types';

export interface GroupSettingsState {
  productGroups: ProductGroup[];
}

export const initialState: GroupSettingsState = {
  productGroups: [],
};

export const getGroupTypes = (productGroups: ProductGroup[] | undefined, groupId: number) =>
  productGroups?.find((g) => g.id === groupId)?.groupTypes.map((x) => x.id) ?? [];

export function getSettingItems<T>(productGroups: ProductGroup[] | undefined, groupId: number, groupTypeId: number) {
  return productGroups?.find((g) => g.id === groupId)?.groupTypes?.find((t) => t.id === groupTypeId)?.groupItems as T[];
}

export function getChanges(roleId: number, productGroups: ProductGroup[], groupTypeId?: number) {
  return productGroups.map((gr) => {
    const groupTypes = gr.groupTypes
      .filter((t) => (t.id === groupTypeId || !groupTypeId) && !t.isReadOnly)
      .map((type) => {
        const groupItems = type.groupItems.filter(
          (item) => item.isMultipleModeChecked && isChanged(item, type.id, roleId)
        );

        if (type.id === GroupTypeIds.BetLimit) {
          const newBetLimits = type.groupItems.filter((item) => {
            if (groupItems.find((i) => i.sportTypeId === item.sportTypeId)) {
              return item;
            }
          });
          return { ...type, groupItems: newBetLimits };
        }

        return { ...type, groupItems };
      });
    return { ...gr, groupTypes: groupTypes.filter((x) => x.groupItems.length > 0) };
  });
}

function isChanged<T>(item: T, groupTypeId: number, roleId: number) {
  if (groupTypeId === GroupTypeIds.ProductStatus) {
    const groupItem = item as ProductStatusGroupItem;
    return groupItem.isDisabled !== groupItem.original?.isDisabled;
  }

  if (groupTypeId === GroupTypeIds.SportsBookCommission) {
    const groupItem = item as CommissionGroupItem;
    return groupItem.discount !== groupItem.original?.discount;
  }

  if (groupTypeId === GroupTypeIds.SportsBookBetSetting) {
    const groupItem = item as SportsBookBetSettingGroupItem;
    return (
      groupItem.minBet !== groupItem.original?.minBet ||
      groupItem.maxBet !== groupItem.original.maxBet ||
      groupItem.maxPerMatch !== groupItem.original.maxPerMatch ||
      groupItem.maxPayoutPerMatch !== groupItem.original.maxPayoutPerMatch
    );
  }

  if (groupTypeId === GroupTypeIds.Commission) {
    const groupItem = item as CommissionGroupItem;
    return groupItem?.discount !== groupItem?.original?.discount;
  }

  if (groupTypeId === GroupTypeIds.WinLossLimit) {
    const groupItem = item as WinLossLimitGroupItem;
    return (
      groupItem.winLimit !== groupItem.original.winLimit ||
      groupItem.lossLimit !== groupItem.original.lossLimit ||
      groupItem.isAutoReset !== groupItem.original.isAutoReset
    );
  }

  if (groupTypeId === GroupTypeIds.BetLimit) {
    const groupItem = item as BetLimitGroupItem;
    return groupItem.isChecked !== groupItem.original?.isChecked;
  }

  if (groupTypeId === GroupTypeIds.ParlayMaxPayout) {
    const groupItem = item as ParlayMaxPayoutGroupItem;
    return (
      groupItem.singleMatchParlay !== groupItem.original?.singleMatchParlay ||
      groupItem.combo2 !== groupItem.original.combo2 ||
      groupItem.combo35 !== groupItem.original.combo35 ||
      groupItem.comboOver6 !== groupItem.original.comboOver6
    );
  }

  if (groupTypeId === GroupTypeIds.PositionTaking) {
    if (roleId === UserLevel.Super) {
      const groupItem = item as SuperPositionTakingGroupItem;
      return (
        groupItem.s4 !== groupItem.original?.s4 ||
        groupItem.m4 !== groupItem.original.m4 ||
        groupItem.a4 !== groupItem.original.a4 ||
        groupItem.n4 !== groupItem.original.n4
      );
    }
    if (roleId === UserLevel.Master) {
      const groupItem = item as MasterPositionTakingGroupItem;
      return (
        groupItem.s3 !== groupItem.original?.s3 ||
        groupItem.iS3 !== groupItem.original.iS3 ||
        groupItem.uS3 !== groupItem.original.uS3 ||
        groupItem.m3 !== groupItem.original.m3 ||
        groupItem.n3 !== groupItem.original.n3
      );
    }
    if (roleId === UserLevel.Agent) {
      const groupItem = item as AgentPositionTakingGroupItem;
      return (
        groupItem.iM2 !== groupItem.original?.iM2 ||
        groupItem.uM2 !== groupItem.original.uM2 ||
        groupItem.m2 !== groupItem.original.m2 ||
        groupItem.a2 !== groupItem.original.a2 ||
        groupItem.n2 !== groupItem.original.n2
      );
    }
    if (roleId === UserLevel.Member) {
      const groupItem = item as MemberPositionTakingGroupItem;
      return groupItem.a1 !== groupItem.original?.a1;
    }
  }

  return false;
}
