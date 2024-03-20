import classNames from 'classnames';

import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import {
  GroupItem,
  SuperPositionTakingGroupItem,
  SuperPositionTakingProps,
} from '@customer-web-app/shared-group-settings-types';
import { Block, BlockColumn, BlockRow } from '@customer-web-app/shared/base/block';

import { GroupLabel } from './position-taking-group-label';
import PTLabel from './position-taking-label';
import PTSelect from './pt-select';
import styles from './pt-select.module.scss';
import { CopiedPTItem, isCopyingPTItem, PTRowProps, renderNoneBlockColumn, usePTCopy } from './utils';

function SuperPositionTakingRow({ item, label, isCopy }: Readonly<PTRowProps<SuperPositionTakingGroupItem>>) {
  const { isMultipleMode, handleSettingItemChange } = useGroupSettingContext();

  const handlePTChange = (data: { [x: string]: number | undefined }) => {
    handleSettingItemChange?.(
      ProductGroupIds.SportsBook,
      GroupTypeIds.PositionTaking,
      item.sportTypeId,
      item.betTypeId,
      data
    );
  };

  const isMultipleEdit = isMultipleMode ?? false;
  const copyRowStyle = { isCopy: isCopy, 'isCopy fail': isCopy === false };
  return (
    <BlockRow label={label ?? item.betTypeName}>
      <BlockColumn>
        <div className={classNames('center', copyRowStyle)}>
          <PTSelect
            min={0}
            max={item.upline.defaultMaxPT}
            step={item.step}
            value={item.s4}
            isDisable={isMultipleEdit}
            onChange={(e) => {
              const newSuperMaxPT = Number(e.target.value);
              const newPTValue = {
                s4: newSuperMaxPT,
                m4: item.m4 > newSuperMaxPT ? newSuperMaxPT : item.m4,
                a4: item.a4 > newSuperMaxPT ? newSuperMaxPT : item.a4,
                n4: item.n4 > newSuperMaxPT ? newSuperMaxPT : item.n4,
              };
              handlePTChange(newPTValue);
            }}
          />
        </div>
      </BlockColumn>
      <BlockColumn>
        <div className={classNames('center', copyRowStyle)}>
          <PTSelect
            min={0}
            max={item.s4}
            value={item.m4}
            step={item.step}
            isDisable={isMultipleEdit}
            onChange={(e) => {
              const newMasterMaxPT = Number(e.target.value);
              const newPTValue = {
                m4: newMasterMaxPT,
                a4: item.a4 > newMasterMaxPT ? newMasterMaxPT : item.a4,
              };
              handlePTChange(newPTValue);
            }}
          />
        </div>
      </BlockColumn>
      <BlockColumn>
        <div className={classNames('center', copyRowStyle)}>
          <PTSelect
            min={0}
            max={item.m4}
            step={item.step}
            value={item.a4}
            isDisable={isMultipleEdit}
            onChange={(e) => {
              handlePTChange({ a4: Number(e.target.value) });
            }}
          />
        </div>
      </BlockColumn>
      <BlockColumn>
        <div className={classNames('center', copyRowStyle)}>
          <PTSelect
            min={0}
            max={item.s4}
            step={item.step}
            value={item.n4}
            isDisable={isMultipleEdit}
            onChange={(e) => handlePTChange({ n4: Number(e.target.value) })}
          />
        </div>
      </BlockColumn>
    </BlockRow>
  );
}

const validateSuperPT = (currentPT: SuperPositionTakingGroupItem, newPT: SuperPositionTakingProps) => {
  const upLinePT = currentPT.upline;
  const { s4, m4, a4, n4 } = newPT;
  const isValidS4 = s4 >= 0 && s4 <= upLinePT.defaultMaxPT;
  const isValidM4 = m4 >= 0 && m4 <= upLinePT.defaultMaxPT;
  const isValidA4 = a4 >= 0 && a4 <= upLinePT.defaultMaxPT;
  const isValidN4 = n4 >= 0 && n4 <= upLinePT.defaultMaxPT;
  return isValidS4 && isValidM4 && isValidA4 && isValidN4;
};

const copySuperPTValues = (
  positionTakings: GroupItem[],
  itemCopied: GroupItem,
  isBelongCopiedGroup?: (item: GroupItem) => boolean
) => {
  const newItemCopied = itemCopied as SuperPositionTakingGroupItem;
  const { s4, m4, a4, n4 } = newItemCopied;
  return (positionTakings as SuperPositionTakingGroupItem[]).map((item) => {
    if (isBelongCopiedGroup?.(item)) {
      return validateSuperPT(item, newItemCopied)
        ? { ...item, s4, m4, a4, n4, isCopied: true }
        : { ...item, isCopied: false };
    }

    return { ...item, isCopied: undefined };
  });
};

type PositionTakingRowProps = {
  ptItems: SuperPositionTakingGroupItem[];
  copyIconId: number | undefined;
  stateCopiedGroup: CopiedPTItem[];
  handleCopyPTs: (
    itemCopied: GroupItem,
    iconId: number | undefined,
    isBelongCopiedGroup?: (item: GroupItem) => boolean
  ) => void;
};

function PositionTakingRows({
  ptItems,
  stateCopiedGroup,
  copyIconId,
  handleCopyPTs,
}: Readonly<PositionTakingRowProps>) {
  let isFirstGroup = true;
  let sportTypeId = -1;
  return ptItems.map((ptItem) => {
    if (sportTypeId !== ptItem.sportTypeId) {
      sportTypeId = ptItem.sportTypeId;
      const group = ptItems.filter((item) => item.sportTypeId === ptItem.sportTypeId);
      const firstGroupItem = group[0];
      const groupPTLabel = (
        <GroupLabel
          copyIconId={copyIconId}
          firstGroupItem={firstGroupItem}
          group={group}
          handleCopyPTs={handleCopyPTs}
          isFirstGroup={isFirstGroup}
          ptItems={ptItems}
          stateCopiedGroup={stateCopiedGroup}
        ></GroupLabel>
      );
      isFirstGroup = false;

      return (
        <div className={styles['block-pt']} key={`${ptItem.sportTypeId}${ptItem.betTypeId}`}>
          <div className={styles['row-empty']}>
            <BlockRow label={groupPTLabel}>
              {renderNoneBlockColumn(`${ptItem.sportTypeId}${ptItem.betTypeId}`, 4)}
            </BlockRow>
          </div>
          {group?.map((item, index) => {
            const ptLabel = (
              <PTLabel
                group={group}
                itemIndex={index}
                item={item}
                copyIconId={copyIconId}
                stateCopiedGroup={stateCopiedGroup}
                handleCopyPTs={handleCopyPTs}
              />
            );
            return (
              <SuperPositionTakingRow
                key={`${item.sportTypeId}-${item.betTypeId}-${item.isLive}`}
                item={item}
                label={ptLabel}
                isCopy={isCopyingPTItem(stateCopiedGroup, item)}
              />
            );
          })}
        </div>
      );
    }

    return null;
  });
}

export type GroupSettingBlockProps = {
  title: React.ReactNode;
  isReadOnly: boolean;
  isDisableMasterAutoPT?: boolean;
};

export default function SuperPositionTaking({ title }: Readonly<GroupSettingBlockProps>) {
  const { productGroups, handleUpdateSettingItems } = useGroupSettingContext();
  const superPTs = getSettingItems<SuperPositionTakingGroupItem>(productGroups, 1, GroupTypeIds.PositionTaking);
  const { stateCopiedGroup, setStateCopiedGroup, timeId, setCopyIconId, copyIconId } = usePTCopy();

  const handleCopyPTs = (
    itemCopied: GroupItem,
    iconId: number | undefined,
    isBelongCopiedGroup?: (item: GroupItem) => boolean
  ) => {
    const newPTs = copySuperPTValues(superPTs, itemCopied, isBelongCopiedGroup);
    const newStateCopiedGroup = newPTs.map((item) => {
      return { betTypeId: item.betTypeId, sportTypeId: item.sportTypeId, isCopied: item.isCopied };
    });
    if (JSON.stringify(newStateCopiedGroup) !== JSON.stringify(stateCopiedGroup)) {
      if (timeId) clearTimeout(timeId.current);
      setStateCopiedGroup(newStateCopiedGroup);
      setCopyIconId(iconId);
      timeId.current = setTimeout(() => {
        setStateCopiedGroup([]);
        setCopyIconId(undefined);
      }, 3000);
    }
    handleUpdateSettingItems?.(1, GroupTypeIds.PositionTaking, newPTs as []);
  };

  return (
    <Block title={title}>
      <BlockRow isHeaderRow>
        {['Super Max PT', 'Master Max PT', 'Agent Max PT', 'Super Min PT'].map((name) => (
          <BlockColumn key={name}>{name}</BlockColumn>
        ))}
      </BlockRow>
      <PositionTakingRows
        ptItems={superPTs}
        copyIconId={copyIconId}
        stateCopiedGroup={stateCopiedGroup}
        handleCopyPTs={handleCopyPTs}
      />
    </Block>
  );
}
