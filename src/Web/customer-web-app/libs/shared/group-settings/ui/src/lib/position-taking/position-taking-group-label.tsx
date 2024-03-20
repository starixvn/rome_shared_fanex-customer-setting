import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { GroupItem } from '@customer-web-app/shared-group-settings-types';

import { CopyStatus, CopyStatusPT, renderBlockRowTitle, renderPTLabel } from './utils';

type GroupPTLabel<T> = {
  isFirstGroup: boolean;
  firstGroupItem: T;
  ptItems: T[];
  group: T[];
  copyIconId: number | undefined;
  stateCopiedGroup: CopyStatusPT[];
  handleCopyPTs: (itemCopied: T, iconId: number | undefined, isBelongCopiedGroup?: (item: T) => boolean) => void;
};

export const SportsBookCopyAllId = 0;

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
        SportsBookCopyAllId,
        copyIconId === 0,
        stateCopiedGroup.length > 0 && !stateCopiedGroup.some((item) => item.copyStatus === CopyStatus.Fail),
        (isChecked) => {
          const groupItems = ptItems.map((item) => {
            if (item.sportTypeId === firstGroupItem.sportTypeId) {
              return { ...item, isMultipleModeChecked: isChecked };
            }

            return item;
          });
          handleUpdateSettingItems?.(ProductGroupIds.SportsBook, GroupTypeIds.PositionTaking, groupItems as []);
        },
        () => handleCopyPTs(firstGroupItem, SportsBookCopyAllId, () => true)
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
