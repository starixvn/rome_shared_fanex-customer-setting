import { GroupTypeIds, UserLevel } from '@customer-web-app/shared-constants';
import {
  AgentPositionTakingGroupItem,
  ColumnsParlayMaxPayout,
  MasterPositionTakingGroupItem,
  MemberPositionTakingGroupItem,
  ParlayMaxPayoutGroupItem,
  ProductGroup,
} from '@customer-web-app/shared-group-settings-types';
import { maxM2, maxS3 } from '@customer-web-app/shared-group-settings-ui';

export const handleInitProducts = (productGroups: ProductGroup[], uplineRoleId: number) => {
  return productGroups.map((group) => {
    const groupTypes = group.groupTypes
      ?.filter((t) => t !== null)
      .map((type) => {
        if (type && type.id === GroupTypeIds.PositionTaking) {
          if (uplineRoleId === UserLevel.Super) {
            const groupItems = (type.groupItems as MasterPositionTakingGroupItem[]).map((item) => ({
              ...item,
              isMultipleModeChecked: true,
              s3: maxS3(item),
              upline: { ...item },
            }));
            return { ...type, groupItems };
          }
          if (uplineRoleId === UserLevel.Master) {
            const groupItems = (type.groupItems as AgentPositionTakingGroupItem[]).map((item) => ({
              ...item,
              isMultipleModeChecked: true,
              m2: maxM2(item),
              upline: { ...item },
            }));
            return { ...type, groupItems };
          }

          const groupItems = (type.groupItems as MemberPositionTakingGroupItem[]).map((item) => ({
            ...item,
            isMultipleModeChecked: true,
            a1: item.a2,
            upline: { ...item },
          }));
          return { ...type, groupItems };
        }

        if (type.id === GroupTypeIds.ParlayMaxPayout) {
          const groupItems = (type.groupItems as ParlayMaxPayoutGroupItem[]).map((item) => {
            const upline = { ...item };
            const isCreateMember = uplineRoleId === UserLevel.Agent;
            if (isCreateMember) {
              ColumnsParlayMaxPayout.forEach((col) => {
                const { inputFieldName, checkBoxFieldName } = col;
                if (item[checkBoxFieldName]) {
                  upline[checkBoxFieldName] = false;
                  upline[inputFieldName] = 0;
                  item[checkBoxFieldName] = false;
                }
              });
            }
            return {
              ...item,
              isMultipleModeChecked: true,
              upline,
              original: { ...item },
            };
          });

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
};
