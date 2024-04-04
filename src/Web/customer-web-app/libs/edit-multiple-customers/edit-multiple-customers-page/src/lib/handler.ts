import { GroupTypeIds, UserLevel } from '@customer-web-app/shared-constants';
import {
    AgentPositionTakingGroupItem, BetLimitGroupItem, MasterPositionTakingGroupItem, ProductGroup,
    ProductStatusGroupItem, WinLossLimitGroupItem
} from '@customer-web-app/shared-group-settings-types';

export function getNewMultipleEditCustomerSettings(
  productGroups: ProductGroup[],
  roleId: number,
  groupTypeId?: number
) {
  return productGroups.map((gr) => {
    const groupTypes = gr.groupTypes
      .filter((t) => t.id === groupTypeId || !groupTypeId)
      .map((type) => {
        if (type.id === GroupTypeIds.PositionTaking) {
          if (roleId === UserLevel.Master) {
            const groupItems = (type.groupItems as MasterPositionTakingGroupItem[]).filter(
              (item) => item.isMultipleModeChecked || item.isUpdateMinPtOnly
            );
            return { ...type, groupItems };
          }

          if (roleId === UserLevel.Agent) {
            const groupItems = (type.groupItems as AgentPositionTakingGroupItem[]).filter(
              (item) => item.isMultipleModeChecked || item.isUpdateMinPtOnly
            );
            return { ...type, groupItems };
          }

          const groupItems = type.groupItems.filter((item) => item.isMultipleModeChecked);
          return { ...type, groupItems };
        }

        const groupItems = type.groupItems.filter((item) => item.isMultipleModeChecked);
        return { ...type, groupItems };
      });
    return { ...gr, groupTypes: groupTypes.filter((x) => x.groupItems.length > 0) };
  });
}

export const handleInitProducts = (productGroups: ProductGroup[]) => {
  return productGroups.map((group) => {
    const groupTypes = group.groupTypes
      .filter((t) => t !== null)
      .map((type) => {
        if (type.id === GroupTypeIds.PositionTaking) {
          return {
            ...type,
            groupItems: type.groupItems.map((item) => ({
              ...item,
              isMultipleModeChecked: false,
              upline: { ...item },
              original: { ...item },
            })),
          };
        }

        if (type.id === GroupTypeIds.ProductStatus) {
          const groupItems = (type.groupItems as ProductStatusGroupItem[]).map((item: ProductStatusGroupItem) => ({
            ...item,
            isMultipleModeChecked: true,
            original: { ...item },
          }));

          return { ...type, groupItems };
        }

        if (type.id === GroupTypeIds.WinLossLimit) {
          const groupItems = (type.groupItems as WinLossLimitGroupItem[]).map((item: WinLossLimitGroupItem) => ({
            ...item,
            isMultipleModeChecked: true,
            original: { ...item },
            lossLimitValue: item.lossLimit !== 0 ? item.lossLimit : undefined,
          }));

          return { ...type, groupItems };
        }

        if (type.id === GroupTypeIds.BetLimit) {
          const groupItems = (type.groupItems as BetLimitGroupItem[]).map((item: BetLimitGroupItem) => ({
            ...item,
            isMultipleModeChecked: true,
            original: { ...item },
          }));

          return { ...type, groupItems };
        }

        if (type.id === GroupTypeIds.SportsBookBetSetting) {
          const groupItems = type.groupItems.map((item: any) => {
            if (params.isCopyBetSetting !== undefined && !params.isCopyBetSetting) {
              item.minBet = 0;
              item.maxBet = 0;
              item.maxPerMatch = 0;
              item.maxPayoutPerMatch = 0;
            }

            return {
              ...item,
              original: { ...item },
              isMultipleModeChecked: false,
            };
          });

          return { ...type, groupItems };
        }

        if (type.id === GroupTypeIds.ParlayMaxPayout) {
          const groupItems = type.groupItems.map((item: any) => {
            if (params.isCopyBetSetting !== undefined && !params.isCopyBetSetting) {
              item.combo2 = item.upline.combo2;
              item.combo35 = item.upline.combo35;
              item.comboOver6 = item.upline.comboOver6;
              item.singleMatchParlay = item.upline.singleMatchParlay;
            }
            return {
              ...item,
              isMultipleModeChecked: false,
              original: { ...item },
              isAllowUnlimitCombo2: item.combo2 === 0,
              isAllowUnlimitCombo35: item.combo35 === 0,
              isAllowUnlimitComboOver6: item.comboOver6 === 0,
              isAllowUnlimitSingleMatchParlay: item.singleMatchParlay === 0,
            };
          });

          return { ...type, groupItems };
        }

        return {
          ...type,
          groupItems: type.groupItems.map((item) => ({
            ...item,
            original: { ...item },
            isMultipleModeChecked: false,
          })),
        };
      });
    return { ...group, groupTypes };
  });
};
