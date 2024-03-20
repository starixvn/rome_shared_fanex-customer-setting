import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { GroupItem } from '@customer-web-app/shared-group-settings-types';

import { CopyStatus, CopyStatusPT, renderBlockRowTitle, renderPTLabel } from './utils';

type PTLabel<T> = {
  group: T[];
  itemIndex: number;
  item: T;
  copyIconId: number | undefined;
  stateCopiedGroup: CopyStatusPT[];
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
  const { isMultipleMode, productGroupId, handleSettingItemChange } = useGroupSettingContext();

  const isMultipleEdit = isMultipleMode ?? false;
  const firstGroupItem = group[0];
  const isSportBookProductGroup = Number(productGroupId) === ProductGroupIds.SportsBook;
  const displayName = isSportBookProductGroup ? item.betTypeName : item.sportTypeName;
  const isBelongCopiedGroup = isSportBookProductGroup
    ? (item: GroupItem) => item.sportTypeId === firstGroupItem.sportTypeId
    : () => true;
  const isCopySuccess = isSportBookProductGroup
    ? stateCopiedGroup.length > 0 &&
      stateCopiedGroup.every((i) =>
        i.sportTypeId === item.sportTypeId
          ? i.copyStatus === CopyStatus.Success || i.copyStatus === CopyStatus.NotCopy
          : true
      )
    : stateCopiedGroup.length > 0 && !stateCopiedGroup.some((i) => i.copyStatus === CopyStatus.Fail);

  return itemIndex === 0 && group.length > 1
    ? renderPTLabel(
        displayName,
        isMultipleEdit,
        item.isMultipleModeChecked,
        item.sportTypeId,
        copyIconId === item.sportTypeId,
        isCopySuccess,
        (isChecked) =>
          handleSettingItemChange?.(
            Number(productGroupId),
            GroupTypeIds.PositionTaking,
            item.sportTypeId,
            item.betTypeId,
            {
              isMultipleModeChecked: isChecked,
              isUpdateMinPtOnly: false,
            } as Partial<GroupItem>
          ),
        () => handleCopyPTs(firstGroupItem, firstGroupItem.sportTypeId, isBelongCopiedGroup)
      )
    : renderBlockRowTitle(displayName, isMultipleEdit, item.isMultipleModeChecked, (isChecked) =>
        handleSettingItemChange?.(
          params.productGroupId,
          GroupTypeIds.PositionTaking,
          item.sportTypeId,
          item.betTypeId,
          { isMultipleModeChecked: isChecked }
        )
      );
}
