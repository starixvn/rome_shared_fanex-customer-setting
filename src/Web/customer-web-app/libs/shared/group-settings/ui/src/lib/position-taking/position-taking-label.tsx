import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { GroupItem } from '@customer-web-app/shared-group-settings-types';

import { CopyPTICon, CopyStatusPT, isCopySuccess, renderPTLabel } from './utils';

type PTLabel<T> = {
  isReadOnly: boolean;
  group: T[];
  itemIndex: number;
  item: T;
  copyIconId: number | undefined;
  stateCopiedGroup: CopyStatusPT[];
  handleCopyPTs: (itemCopied: T, iconId: number | undefined, isBelongCopiedGroup?: (item: T) => boolean) => void;
};

export default function PTLabel({
  isReadOnly,
  group,
  itemIndex,
  item,
  copyIconId,
  stateCopiedGroup,
  handleCopyPTs,
}: PTLabel<GroupItem>) {
  const { isMultipleMode, productGroupId, handleSettingItemChange } = useGroupSettingContext();

  const firstGroupItem = group[0];
  const isSportBookProductGroup = Number(productGroupId) === ProductGroupIds.SportsBook;
  const displayName = isSportBookProductGroup ? item.betTypeName : item.sportTypeName;
  const isBelongCopiedGroup = isSportBookProductGroup
    ? (item: GroupItem) => item.sportTypeId === firstGroupItem.sportTypeId
    : () => true;

  const copyIcon = itemIndex === 0 && group.length > 1 && !isReadOnly && (
    <CopyPTICon
      id={item.sportTypeId}
      isCopy={copyIconId === item.sportTypeId}
      isCopySuccess={isCopySuccess(stateCopiedGroup, isSportBookProductGroup ? item.sportTypeId : undefined)}
      handleCopyClick={() => handleCopyPTs(firstGroupItem, firstGroupItem.sportTypeId, isBelongCopiedGroup)}
    />
  );

  return renderPTLabel({
    labelName: displayName,
    isMultipleMode: isMultipleMode ?? false,
    isMultipleModeChecked: item.isMultipleModeChecked,
    copyIcon,
    handleCheckMultiple: (isChecked) =>
      handleSettingItemChange?.(Number(productGroupId), GroupTypeIds.PositionTaking, item.sportTypeId, item.betTypeId, {
        isMultipleModeChecked: isChecked,
        isUpdateMinPtOnly: false,
      } as Partial<GroupItem>),
    tooltipMessage: item.tooltipMessage,
    sportTypeId: item.sportTypeId,
    betTypeId: item.betTypeId
  });
}
