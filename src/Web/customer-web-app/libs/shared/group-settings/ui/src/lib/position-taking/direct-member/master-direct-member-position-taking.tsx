import classNames from 'classnames';
import { useTranslation } from 'react-i18next';

import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import {
    GroupItem, MasterDirectMemberPositionTakingGroupItem, MasterDirectMemberPositionTakingProps
} from '@customer-web-app/shared-group-settings-types';
import { BlockColumn, BlockRow } from '@customer-web-app/shared/base/block';

import { GroupLabel } from '../position-taking-group-label';
import PTLabel from '../position-taking-label';
import PTSelect from '../pt-select';
import { CopyStatus } from '../types';
import { groupSportsBookPTBySportTypeId, isCopyingPTItem, usePTCopy } from '../utils';
import styles from './direct-member-position-taking.module.scss';
import { Props } from './types';

const validateSuperPT = (
  currentPT: MasterDirectMemberPositionTakingGroupItem,
  newPT: MasterDirectMemberPositionTakingProps
) => {
  const { m3, n3 } = currentPT;
  const { m1 } = newPT;

  return Math.max(0, n3) <= m1 && m1 <= m3;
};

export default function MasterDirectMemberPositionTaking({ isReadOnly, onPTChange }: Props) {
  const { t } = useTranslation();
  const { isMultipleMode, handleUpdateSettingItems, productGroupId, productGroups } = useGroupSettingContext();
  const ptItems = getSettingItems<MasterDirectMemberPositionTakingGroupItem>(
    productGroups,
    Number(productGroupId),
    GroupTypeIds.PositionTaking
  );

  const groups = (
    productGroupId === ProductGroupIds.SportsBook ? Object.values(groupSportsBookPTBySportTypeId(ptItems)) : [ptItems]
  ) as MasterDirectMemberPositionTakingGroupItem[][];

  const isMultipleEdit = isMultipleMode ?? false;
  const isDisable = isReadOnly || isMultipleEdit;

  const { stateCopiedGroup, copyIconId, handleCopy } = usePTCopy();

  const handleCopyPTs = (
    itemCopied: GroupItem,
    iconId: number | undefined,
    isBelongCopiedGroup?: (item: GroupItem) => boolean
  ) => {
    const newItemCopied = itemCopied as MasterDirectMemberPositionTakingGroupItem;
    const { m1 } = newItemCopied;
    const newPTs = ptItems.map((item) => {
      if (isBelongCopiedGroup?.(item)) {
        return validateSuperPT(item, newItemCopied)
          ? { ...item, m1, copyStatus: CopyStatus.Success }
          : { ...item, copyStatus: CopyStatus.Fail };
      }

      return { ...item, copyStatus: CopyStatus.NotCopy };
    });
    if (newPTs.length > 0) {
      handleCopy(newPTs, iconId);
      handleUpdateSettingItems?.(Number(productGroupId), GroupTypeIds.PositionTaking, newPTs as []);
    }
  };

  return (
    <>
      <BlockRow isHeaderRow>
        <BlockColumn>{t('MasterPT')}</BlockColumn>
      </BlockRow>
      {groups.map((group, index) => {
        const firstGroupItem = group[0];
        const groupPTLabel = (
          <GroupLabel
            isReadOnly={isReadOnly}
            copyIconId={copyIconId}
            firstGroupItem={firstGroupItem}
            group={group}
            handleCopyPTs={handleCopyPTs}
            isFirstGroup={index === 0}
            ptItems={ptItems}
            stateCopiedGroup={stateCopiedGroup}
          />
        );
        return (
          <div className={styles['block-pt']} key={index}>
            {productGroupId === ProductGroupIds.SportsBook && (
              <div className={styles['row-empty']}>
                <BlockRow label={groupPTLabel}>
                  <BlockColumn>{''}</BlockColumn>
                </BlockRow>
              </div>
            )}
            {group.map((item, index) => {
              const ptLabel = (
                <PTLabel
                  isReadOnly={isReadOnly}
                  group={group}
                  itemIndex={index}
                  item={item}
                  copyIconId={copyIconId}
                  stateCopiedGroup={stateCopiedGroup}
                  handleCopyPTs={handleCopyPTs}
                />
              );
              const copyStatus = isCopyingPTItem(stateCopiedGroup, item);

              return (
                <BlockRow key={item.betTypeId} label={ptLabel}>
                  <BlockColumn>
                    <div
                      className={classNames('center', {
                        isCopy: copyStatus === CopyStatus.Success,
                        'isCopy fail': copyStatus === CopyStatus.Fail,
                      })}
                    >
                      <PTSelect
                        min={Math.max(0, item.n3)}
                        max={item.m3}
                        value={item.m1}
                        step={item.step}
                        isDisable={isDisable}
                        onChange={(e) => {
                          const m1 = Number(e.target.value);
                          onPTChange(item.sportTypeId, item.betTypeId, { m1 });
                        }}
                      />
                    </div>
                  </BlockColumn>
                </BlockRow>
              );
            })}
          </div>
        );
      })}
    </>
  );
}
