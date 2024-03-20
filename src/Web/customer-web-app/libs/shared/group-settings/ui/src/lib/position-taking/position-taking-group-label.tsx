import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { GroupItem } from '@customer-web-app/shared-group-settings-types';

import { CopiedPTItem, renderBlockRowTitle, renderPTLabel } from './utils';

type GroupPTLabel<T> = {
  isFirstGroup: boolean;
  firstGroupItem: T;
  ptItems: T[];
  group: T[];
  copyIconId: number | undefined;
  stateCopiedGroup: CopiedPTItem[];
  handleCopyPTs: (itemCopied: T, iconId: number | undefined, isBelongCopiedGroup?: (item: T) => boolean) => void;
};

export function GroupLabel({
  isFirstGroup,
  firstGroupItem,
  group,
  copyIconId,
  stateCopiedGroup,
  ptItems,
  handleCopyPTs,
}: GroupPTLabel<GroupItem>) {
  const { isMultipleMode, handleUpdateSettingItems } = useGroupSettingContext();

  const isMultipleEdit = isMultipleMode ?? false;
  return isFirstGroup
    ? renderPTLabel(
        firstGroupItem.sportTypeName,
        isMultipleEdit,
        group.every((item) => item.isMultipleModeChecked === true),
        0,
        copyIconId === 0,
        stateCopiedGroup.length > 0 && !stateCopiedGroup.some((item) => item.isCopied === false),
        (isChecked) => {
          const groupItems = ptItems.map((item) => {
            if (item.sportTypeId === firstGroupItem.sportTypeId) {
              return { ...item, isMultipleModeChecked: isChecked };
            }

            return item;
          });
          handleUpdateSettingItems?.(ProductGroupIds.SportsBook, GroupTypeIds.PositionTaking, groupItems as []);
        },
        () => handleCopyPTs(firstGroupItem, 0, () => true)
      )
    : renderBlockRowTitle(
        firstGroupItem.sportTypeName,
        isMultipleEdit,
        group.every((item) => item.isMultipleModeChecked === true),
        (isChecked) => {
          const groupItems = ptItems.map((item) => {
            if (item.sportTypeId === firstGroupItem.sportTypeId) {
              return { ...item, isMultipleModeChecked: isChecked };
            }

            return item;
          });
          handleUpdateSettingItems?.(ProductGroupIds.SportsBook, GroupTypeIds.PositionTaking, groupItems as []);
        }
      );
}
