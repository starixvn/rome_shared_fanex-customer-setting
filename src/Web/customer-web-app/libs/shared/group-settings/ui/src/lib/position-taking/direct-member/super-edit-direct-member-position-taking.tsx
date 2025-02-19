import classNames from 'classnames';
import { useTranslation } from 'react-i18next';

import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import {
  GroupItem,
  SuperEditDirectMemberPositionTakingGroupItem,
  SuperEditDirectMemberPositionTakingProps,
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
  currentPT: SuperEditDirectMemberPositionTakingGroupItem,
  newPT: SuperEditDirectMemberPositionTakingProps
) => {
  const { s4, n4, m1, m3 } = currentPT;
  const { s1, iS1, uS1 } = newPT;
  if (iS1) {
    return Math.max(0, n4 - m1) <= uS1 && uS1 <= s4;
  } else {
    return Math.max(0, n4 - m1) <= s1 && s1 <= Math.max(n4 - m1, s4 - m3);
  }
};

export default function SuperEditDirectMemberPositionTaking({ isReadOnly, onPTChange }: Props) {
  const { t } = useTranslation();
  const { productGroupId, productGroups, handleUpdateSettingItems } = useGroupSettingContext();
  const ptItems = getSettingItems<SuperEditDirectMemberPositionTakingGroupItem>(
    productGroups,
    Number(productGroupId),
    GroupTypeIds.PositionTaking
  );

  const groups = (
    productGroupId === ProductGroupIds.SportsBook ? Object.values(groupSportsBookPTBySportTypeId(ptItems)) : [ptItems]
  ) as SuperEditDirectMemberPositionTakingGroupItem[][];

  const { stateCopiedGroup, copyIconId, handleCopy } = usePTCopy();

  const handleCopyPTs = (
    itemCopied: GroupItem,
    iconId: number | undefined,
    isBelongCopiedGroup?: (item: GroupItem) => boolean
  ) => {
    const newItemCopied = itemCopied as SuperEditDirectMemberPositionTakingGroupItem;
    const { s1 } = newItemCopied;
    const newPTs = ptItems.map((item) => {
      if (isBelongCopiedGroup?.(item)) {
        return validateSuperPT(item, newItemCopied)
          ? { ...item, s1, copyStatus: CopyStatus.Success }
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
        <BlockColumn>{t('SuperPT')}</BlockColumn>
        <BlockColumn>{t('IsAuto')}</BlockColumn>
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
                  <BlockColumn>{''}</BlockColumn>
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
              const copyRowStyle = {
                isCopy: copyStatus === CopyStatus.Success,
                'isCopy fail': copyStatus === CopyStatus.Fail,
              };

              return (
                <BlockRow key={item.betTypeId} label={ptLabel}>
                  <BlockColumn>
                    <div
                      className={classNames('center', {
                        isCopy: copyStatus === CopyStatus.Success,
                        'isCopy fail': copyStatus === CopyStatus.Fail,
                      })}
                    >
                      {item.iS1 ? (
                        <PTSelect
                          min={Math.max(0, item.n4 - item.m1)}
                          max={Math.max(item.n4 - item.m1, item.s4 - item.m3)}
                          value={item.uS1}
                          step={item.step}
                          isDisable={isReadOnly || !item.isMultipleModeChecked}
                          onChange={(e) => {
                            const uS1 = Number(e.target.value);
                            onPTChange(item.sportTypeId, item.betTypeId, { uS1 });
                          }}
                        />
                      ) : (
                        <PTSelect
                          min={Math.max(0, item.n4 - item.m1)}
                          max={item.s4}
                          value={item.s1}
                          step={item.step}
                          isDisable={isReadOnly || item.iS3 || !item.isMultipleModeChecked}
                          onChange={(e) => {
                            const s1 = Number(e.target.value);
                            onPTChange(item.sportTypeId, item.betTypeId, { s1 });
                          }}
                        />
                      )}
                    </div>
                  </BlockColumn>
                  <BlockColumn>
                    <div className={classNames('center copy-checkbox', copyRowStyle)}>
                      <input
                        type="checkbox"
                        checked={item.iS1}
                        disabled={isReadOnly || !item.isMultipleModeChecked}
                        onChange={(e) => {
                          onPTChange(item.sportTypeId, item.betTypeId, {
                            iS1: e.target.checked,
                          });
                        }}
                      />
                    </div>
                  </BlockColumn>
                  <BlockColumn>
                    <div className="center">{item.m1}</div>
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
