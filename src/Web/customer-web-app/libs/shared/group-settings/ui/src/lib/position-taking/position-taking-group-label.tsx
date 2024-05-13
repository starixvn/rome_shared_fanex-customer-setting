import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { GroupItem } from '@customer-web-app/shared-group-settings-types';

import { CopyPTICon, CopyStatusPT, isCopySuccess, renderPTLabel } from './utils';

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

  const copyIcon = isFirstGroup && (
    <CopyPTICon
      id={SportsBookCopyAllId}
      isCopy={copyIconId === 0}
      isCopySuccess={isCopySuccess(stateCopiedGroup)}
      handleCopyClick={() => handleCopyPTs(firstGroupItem, SportsBookCopyAllId, () => true)}
    />
  );

  return renderPTLabel(
    firstGroupItem.sportTypeName,
    isMultipleMode ?? false,
    group.every((item) => item.isMultipleModeChecked === true),
    copyIcon,
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
