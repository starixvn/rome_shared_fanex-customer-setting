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
import { CopyStatus, PositionTakingBlockProps, PTRowProps } from './types';
import { useTranslation } from 'react-i18next';
import {
  CopyStatusPT,
  groupSportsBookPTBySportTypeId,
  isCopyingPTItem,
  renderNoneBlockColumn,
  usePTCopy,
} from './utils';

function SuperPositionTakingRow({
  item,
  label,
  copyStatus,
  isReadOnly,
}: Readonly<PTRowProps<SuperPositionTakingGroupItem>>) {
  const { isMultipleMode, handleSettingItemChange, productGroupId } = useGroupSettingContext();

  const handlePTChange = (data: { [x: string]: number | undefined }) => {
    handleSettingItemChange?.(
      Number(productGroupId),
      GroupTypeIds.PositionTaking,
      item.sportTypeId,
      item.betTypeId,
      data
    );
  };

  const isMultipleEdit = isMultipleMode ?? false;
  const isDisable = isReadOnly || isMultipleEdit;
  const copyRowStyle = { isCopy: copyStatus === CopyStatus.Success, 'isCopy fail': copyStatus === CopyStatus.Fail };
  return (
    <BlockRow label={label ?? item.betTypeName}>
      <BlockColumn>
        <div className={classNames('center', copyRowStyle)}>
          <PTSelect
            min={0}
            max={item.upline.defaultMaxPT}
            step={item.step}
            value={item.s4}
            isDisable={isDisable}
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
            isDisable={isDisable}
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
            isDisable={isDisable}
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
            isDisable={isDisable}
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
        ? { ...item, s4, m4, a4, n4, copyStatus: CopyStatus.Success }
        : { ...item, copyStatus: CopyStatus.Fail };
    }

    return { ...item, copyStatus: CopyStatus.NotCopy };
  });
};

type PositionTakingRowProps = {
  isReadOnly: boolean;
  productGroupId: number;
  ptItems: SuperPositionTakingGroupItem[];
  copyIconId: number | undefined;
  stateCopiedGroup: CopyStatusPT[];
  handleCopyPTs: (
    itemCopied: GroupItem,
    iconId: number | undefined,
    isBelongCopiedGroup?: (item: GroupItem) => boolean
  ) => void;
};

function PositionTakingRows({
  isReadOnly,
  productGroupId,
  ptItems,
  stateCopiedGroup,
  copyIconId,
  handleCopyPTs,
}: Readonly<PositionTakingRowProps>) {
  const ptGroups =
    productGroupId === ProductGroupIds.SportsBook ? Object.values(groupSportsBookPTBySportTypeId(ptItems)) : [ptItems];
  return (ptGroups as SuperPositionTakingGroupItem[][]).map((group, index) => {
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
      ></GroupLabel>
    );

    return (
      <div className={styles['block-pt']} key={`${firstGroupItem.sportTypeId}${firstGroupItem.betTypeId}`}>
        {productGroupId === ProductGroupIds.SportsBook && (
          <div className={styles['row-empty']}>
            <BlockRow label={groupPTLabel}>
              {renderNoneBlockColumn(`${firstGroupItem.sportTypeId}${firstGroupItem.betTypeId}`, 4)}
            </BlockRow>
          </div>
        )}
        {group?.map((item, index) => {
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
          return (
            <SuperPositionTakingRow
              isReadOnly={isReadOnly}
              key={`${item.sportTypeId}-${item.betTypeId}-${item.isLive}`}
              item={item}
              label={ptLabel}
              copyStatus={isCopyingPTItem(stateCopiedGroup, item)}
            />
          );
        })}
      </div>
    );
  });
}

export default function SuperPositionTaking({ title, isReadOnly }: Readonly<PositionTakingBlockProps>) {
  const { 
    productGroups, handleUpdateSettingItems, productGroupId, isEditMode, updateGroupSetting,
    isMultipleMode
  } =
    useGroupSettingContext();
  const groupId = Number(productGroupId);
  const superPTs = getSettingItems<SuperPositionTakingGroupItem>(productGroups, groupId, GroupTypeIds.PositionTaking);
  const { stateCopiedGroup, copyIconId, handleCopy } = usePTCopy();

  const handleCopyPTs = (
    itemCopied: GroupItem,
    iconId: number | undefined,
    isBelongCopiedGroup?: (item: GroupItem) => boolean
  ) => {
    const newPTs = copySuperPTValues(superPTs, itemCopied, isBelongCopiedGroup);
    if (newPTs.length > 0) {
      handleCopy(newPTs, iconId);
      handleUpdateSettingItems?.(groupId, GroupTypeIds.PositionTaking, newPTs as []);
    }
  };

  const { t } = useTranslation();

  const handleSelectAll = (isChecked: boolean) => {
    const newPTs = superPTs.map((item) => ({
      ...item,
      isMultipleModeChecked: isChecked,
      isUpdateMinPtOnly: false,
    }));
    handleUpdateSettingItems?.(groupId, GroupTypeIds.PositionTaking, newPTs);
  };

  return (
    <Block
      title={title}
      isPartialUpdate={isEditMode === true && !isReadOnly}
      onSaveClick={() => updateGroupSetting?.(GroupTypeIds.PositionTaking)}
    >
      <BlockRow 
        isHeaderRow
        label={
          isMultipleMode &&
          superPTs.length > 1 &&
          !isReadOnly && (
            <div style={{ display: 'flex', marginLeft: '2px' }}>
              <input
                type="checkbox"
                style={{ top: '0' }}
                onChange={(e) => handleSelectAll(e.target.checked)}
                checked={superPTs.every((item) => item.isMultipleModeChecked === true)}
              />
              <div>{t('SelectAll')}</div>
            </div>
          )
        }
      >
        {['Super Max PT', 'Master Max PT', 'Agent Max PT', 'Super Min PT'].map((name) => (
          <BlockColumn key={name}>{name}</BlockColumn>
        ))}
      </BlockRow>
      <PositionTakingRows
        isReadOnly={isReadOnly}
        productGroupId={groupId}
        ptItems={superPTs}
        copyIconId={copyIconId}
        stateCopiedGroup={stateCopiedGroup}
        handleCopyPTs={handleCopyPTs}
      />
    </Block>
  );
}
