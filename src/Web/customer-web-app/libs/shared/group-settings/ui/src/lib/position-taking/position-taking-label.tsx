import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { GroupItem } from '@customer-web-app/shared-group-settings-types';

import { CopiedPTItem, renderBlockRowTitle, renderPTLabel } from './utils';

type PTLabel<T> = {
  group: T[];
  itemIndex: number;
  item: T;
  copyIconId: number | undefined;
  stateCopiedGroup: CopiedPTItem[];
  handleCopyPTs: (itemCopied: T, iconId: number | undefined, isBelongCopiedGroup?: (item: T) => boolean) => void;
};

export default function PTLabel({
  group,
  itemIndex,
  item,
  copyIconId,
  stateCopiedGroup,
  handleCopyPTs,
}: PTLabel<GroupItem>) {
  const { isMultipleMode, handleSettingItemChange } = useGroupSettingContext();

  const isMultipleEdit = isMultipleMode ?? false;
  const firstGroupItem = group[0];
  return itemIndex === 0 && group.length > 1
    ? renderPTLabel(
        item.betTypeName,
        isMultipleEdit,
        item.isMultipleModeChecked,
        item.sportTypeId,
        copyIconId === item.sportTypeId,
        stateCopiedGroup.length > 0 &&
          stateCopiedGroup.every((i) =>
            i.sportTypeId === item.sportTypeId ? i.isCopied === true || i.isCopied === undefined : !i.isCopied
          ),
        (isChecked) =>
          handleSettingItemChange?.(
            ProductGroupIds.SportsBook,
            GroupTypeIds.PositionTaking,
            item.sportTypeId,
            item.betTypeId,
            {
              isMultipleModeChecked: isChecked,
              isUpdateMinPtOnly: false,
            } as Partial<GroupItem>
          ),
        () =>
          handleCopyPTs(
            firstGroupItem,
            firstGroupItem.sportTypeId,
            (item) => item.sportTypeId === firstGroupItem.sportTypeId
          )
      )
    : renderBlockRowTitle(item.betTypeName, isMultipleEdit, item.isMultipleModeChecked, (isChecked) =>
        handleSettingItemChange?.(
          ProductGroupIds.SportsBook,
          GroupTypeIds.PositionTaking,
          item.sportTypeId,
          item.betTypeId,
          { isMultipleModeChecked: isChecked }
        )
      );
}
