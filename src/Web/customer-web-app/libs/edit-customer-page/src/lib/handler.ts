import { GroupTypeIds, UserLevel } from '@customer-web-app/shared-constants';
import { GeneralInfo } from '@customer-web-app/shared-general-information-state';
import {
    AgentPositionTakingGroupItem, GroupSettingItem, GroupType, MasterEditMemberPositionTakingGroupItem,
    MasterPositionTakingGroupItem, ParlayMaxPayoutGroupItem, ProductGroup, ProductStatusGroupItem,
    SuperEditMemberPositionTakingGroupItem, WinLossLimitGroupItem
} from '@customer-web-app/shared-group-settings-types';

export const handleChangeItemValue = (
  productGroups: ProductGroup[],
  productGroupId: number,
  groupTypeId: number,
  sportTypeId: number,
  betTypeId: number,
  data: Partial<GroupSettingItem<unknown>>
) => {
  return productGroups.map((productGroup) => {
    if (productGroup.id === productGroupId) {
      const groupTypes = productGroup.groupTypes.map((type) => {
        if (type.id === groupTypeId) {
          const groupItems = type.groupItems.map((item) => {
            if (item.sportTypeId === sportTypeId && item.betTypeId === betTypeId) {
              return { ...item, ...data };
            }
            return item;
          });
          return { ...type, groupItems };
        }
        return type;
      });

      return { ...productGroup, groupTypes };
    }
    return productGroup;
  });
};

export const handleChangeGroupSettingValue = (
  productGroups: ProductGroup[],
  productGroupId: number,
  groupTypeId: number,
  groupItems: GroupSettingItem<unknown>[]
) => {
  return productGroups.map((productGroup) => {
    if (productGroup.id === productGroupId) {
      const groupTypes = productGroup.groupTypes.map((type) => {
        if (type.id === groupTypeId) {
          return { ...type, groupItems };
        }
        return type;
      });

      return { ...productGroup, groupTypes };
    }
    return productGroup;
  });
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

export const handleInitProducts = (productGroups: ProductGroup[]) => {
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
            original: { ...item },
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

export const isGeneralInfoChanged = (generalInfo: GeneralInfo, originalGeneralInfo: GeneralInfo | undefined) =>
  generalInfo.maxCredit !== originalGeneralInfo?.maxCredit ||
  generalInfo.memberMaxCredit !== originalGeneralInfo?.memberMaxCredit ||
  generalInfo.firstName !== originalGeneralInfo.firstName ||
  generalInfo.lastName !== originalGeneralInfo.lastName ||
  generalInfo.phone !== originalGeneralInfo.phone ||
  generalInfo.mobilePhone !== originalGeneralInfo.mobilePhone ||
  generalInfo.fax !== originalGeneralInfo.fax;

export const getUnCheckAutoPtItems = (
  productGroups: ProductGroup[],
  roleId: number,
  productGroupId: number,
  groupTypeId: number
) => {
  const ptGroupType = productGroups
    .find((productGroup) => productGroup.id === productGroupId)
    ?.groupTypes.find((groupType) => groupType.id === groupTypeId);

  if (!ptGroupType) return [];

  if (roleId === UserLevel.Master) {
    return (ptGroupType?.groupItems as MasterPositionTakingGroupItem[]).filter(
      (item) => item.original.iS3 && !item.iS3
    );
  }

  if (roleId === UserLevel.Agent) {
    return (ptGroupType?.groupItems as AgentPositionTakingGroupItem[]).filter((item) => item.original.iM2 && !item.iM2);
  }

  return [];
};

export const getNewIndirectEditCustomerSettings = (
  productGroups: ProductGroup[],
  uplineRoleId: number,
  groupTypeId?: number
) => {
  return productGroups.map((productGroup) => {
    const groupTypes = productGroup.groupTypes
      .filter((groupType) => groupType.id === groupTypeId || !groupTypeId)
      .map((filteredGroupType) => {
        const groupItems = filteredGroupType.groupItems.filter((groupItem) =>
          isIndirectEditSettingChanged(groupItem, filteredGroupType.id, uplineRoleId)
        );
        return { ...filteredGroupType, groupItems };
      });

    return { ...productGroup, groupTypes: groupTypes.filter((groupType) => groupType.groupItems.length > 0) };
  });
};

function isIndirectEditSettingChanged<T>(item: T, groupTypeId: number, uplineRoleId: number) {
  if (groupTypeId === GroupTypeIds.PositionTaking) {
    if (uplineRoleId === UserLevel.Super) {
      const groupItem = item as SuperEditMemberPositionTakingGroupItem;
      return (
        groupItem.s1 !== groupItem.upline.s1 ||
        groupItem.uS1 !== groupItem.upline.uS1 ||
        groupItem.iS1 !== groupItem.upline.iS1
      );
    }

    if (uplineRoleId === UserLevel.Master) {
      const groupItem = item as MasterEditMemberPositionTakingGroupItem;
      return (
        groupItem.m1 !== groupItem.upline.m1 ||
        groupItem.uM1 !== groupItem.upline.uM1 ||
        groupItem.iM1 !== groupItem.upline.iM1
      );
    }
  }

  return false;
}
