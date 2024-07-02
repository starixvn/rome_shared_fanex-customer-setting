import classNames from 'classnames';
import { forwardRef, useEffect, useImperativeHandle, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { ValidationRefType } from '@customer-web-app/share-models';
import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import {
  GroupItem,
  MasterPositionTakingGroupItem,
  MasterPositionTakingProps,
} from '@customer-web-app/shared-group-settings-types';
import { Block, BlockColumn, BlockRefType, BlockRow } from '@customer-web-app/shared/base/block';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import { scrollToBLock } from '../scroll-helper';
import { GroupLabel } from './position-taking-group-label';
import PTLabel from './position-taking-label';
import PTSelect from './pt-select';
import styles from './pt-select.module.scss';
import { CopyStatus, PositionTakingBlockProps, PTRowProps } from './types';
import {
  CopyStatusPT,
  groupSportsBookPTBySportTypeId,
  isCopyingPTItem,
  renderNoneBlockColumn,
  usePTCopy,
} from './utils';

function minS3(masterPositionTakingGroupItem: MasterPositionTakingProps) {
  const { n4, m4 } = masterPositionTakingGroupItem;
  return Math.max(0, n4 - m4);
}

export function maxS3(masterPositionTakingGroupItem: MasterPositionTakingProps) {
  const { s4 } = masterPositionTakingGroupItem;
  return s4;
}

function minM3(masterPositionTakingGroupItem: MasterPositionTakingProps, s3: number) {
  const { n4 } = masterPositionTakingGroupItem;
  return Math.max(0, n4 - s3);
}

function maxM3(masterPositionTakingGroupItem: MasterPositionTakingProps, s3: number) {
  const { s4, m4 } = masterPositionTakingGroupItem;
  return Math.min(s4 - s3, m4);
}

function minUS3(masterPositionTakingGroupItem: MasterPositionTakingProps) {
  const { n4, m4 } = masterPositionTakingGroupItem;
  return Math.max(0, n4 - m4);
}

function maxUS3(masterPositionTakingGroupItem: MasterPositionTakingProps) {
  const { s4 } = masterPositionTakingGroupItem;
  return s4;
}

function minM3Auto(masterPositionTakingGroupItem: MasterPositionTakingProps, uS3: number) {
  const { n4 } = masterPositionTakingGroupItem;
  return Math.max(0, n4 - uS3);
}

function maxM3Auto(masterPositionTakingGroupItem: MasterPositionTakingProps) {
  const { s4, m4 } = masterPositionTakingGroupItem;
  return Math.min(s4, m4);
}

function minN3() {
  return 0;
}

function maxN3(m3: number) {
  return m3;
}

function MasterPositionTakingRow({
  item,
  label,
  copyStatus,
  isReadOnly,
}: Readonly<PTRowProps<MasterPositionTakingGroupItem>>) {
  const { isMultipleMode, handleSettingItemChange, productGroupId } = useGroupSettingContext();

  const handlePTChange = (data: { [x: string]: number | boolean }) => {
    handleSettingItemChange?.(
      Number(productGroupId),
      GroupTypeIds.PositionTaking,
      item.sportTypeId,
      item.betTypeId,
      data
    );
  };

  const copyRowStyle = { isCopy: copyStatus === CopyStatus.Success, 'isCopy fail': copyStatus === CopyStatus.Fail };
  return (
    <BlockRow label={label ?? item.betTypeName}>
      <BlockColumn>
        {item.iS3 ? (
          <div id={`uS3-${item.sportTypeId}-${item.betTypeId}`} className={classNames('center', copyRowStyle)}>
            <PTSelect
              min={minUS3(item.upline)}
              max={maxUS3(item.upline)}
              step={item.step}
              value={item.uS3}
              isDisable={isReadOnly || !item.isMultipleModeChecked}
              onChange={(e) => {
                const uS3 = Number(e.target.value);
                const masterMaxPT = maxM3Auto(item);
                const newM3 = item.m3 > masterMaxPT ? masterMaxPT : item.m3;
                const m3 = Math.max(newM3, minM3Auto({ ...item }, uS3));
                const n3 = item.n3 > m3 ? m3 : item.n3;
                handlePTChange({ uS3, m3, n3 });
              }}
            />
          </div>
        ) : (
          <div id={`s3-${item.sportTypeId}-${item.betTypeId}`} className={classNames('center', copyRowStyle)}>
            <PTSelect
              min={minS3(item.upline)}
              max={maxS3(item.upline)}
              step={item.step}
              value={item.s3}
              isDisable={isReadOnly || !item.isMultipleModeChecked}
              onChange={(e) => {
                const s3 = Number(e.target.value);
                const masterMaxPT = maxM3(item, s3);
                const m3 = Math.max(masterMaxPT, minM3(item, s3));
                const n3 = item.n3 > m3 ? m3 : item.n3;
                handlePTChange({ s3, m3, n3 });
              }}
            />
          </div>
        )}
      </BlockColumn>
      <BlockColumn>
        <div className={classNames('center copy-checkbox', copyRowStyle)}>
          <input
            type="checkbox"
            checked={item.iS3}
            disabled={isReadOnly || !item.isMultipleModeChecked}
            onChange={(e) => {
              const isAuto = e.target.checked;
              const s3 = item.s3 > maxS3(item) ? maxS3(item) : item.s3;
              const uS3 = item.uS3 < minUS3(item) ? minUS3(item) : item.uS3;
              const newM3Auto = item.m3 < minM3Auto(item, uS3) ? minM3Auto(item, uS3) : item.m3;
              const newM3 = item.m3 > maxM3(item, s3) ? maxM3(item, s3) : Math.max(minM3(item, s3), item.m3);
              const m3 = isAuto ? newM3Auto : newM3;
              const n3 = item.n3 > m3 ? m3 : item.n3;
              isAuto ? handlePTChange({ iS3: isAuto, m3, n3, uS3 }) : handlePTChange({ iS3: isAuto, m3, n3, s3 });
            }}
          />
        </div>
      </BlockColumn>
      <BlockColumn>
        <div
          id={item.iS3 ? `m3Auto-${item.sportTypeId}-${item.betTypeId}` : `m3-${item.sportTypeId}-${item.betTypeId}`}
          className={classNames('center', copyRowStyle)}
        >
          <PTSelect
            min={item.iS3 ? minM3Auto(item.upline, item.uS3) : minM3(item.upline, item.s3)}
            max={item.iS3 ? maxM3Auto(item.upline) : maxM3(item.upline, item.s3)}
            step={item.step}
            value={item.m3}
            isDisable={isReadOnly || !item.isMultipleModeChecked}
            onChange={(e) => {
              const m3 = Number(e.target.value);
              const n3 = item.n3 > m3 ? m3 : item.n3;
              handlePTChange({ m3, n3 });
            }}
          />
        </div>
      </BlockColumn>
      <BlockColumn>
        <div className={classNames('center', copyRowStyle, { 'is-multiple-mode': isMultipleMode })}>
          {isMultipleMode && !isReadOnly && (
            <div className={styles['checkbox-multiple']}>
              <input
                type="checkbox"
                checked={item.isUpdateMinPtOnly || item.isMultipleModeChecked}
                onChange={(e) => handlePTChange({ isUpdateMinPtOnly: e.target.checked })}
                disabled={item.isMultipleModeChecked}
              />
            </div>
          )}
          <PTSelect
            min={minN3()}
            max={maxN3(item.m3)}
            step={item.step}
            value={item.n3}
            isDisable={isReadOnly || (!item.isMultipleModeChecked && !item.isUpdateMinPtOnly)}
            onChange={(e) => handlePTChange({ n3: Number(e.target.value) })}
          />
        </div>
      </BlockColumn>
    </BlockRow>
  );
}

const validateMasterPT = (currentPT: MasterPositionTakingGroupItem, newPT: MasterPositionTakingProps) => {
  const upLinePT = currentPT.upline;
  const { s3, uS3, iS3, m3, n3 } = newPT;
  const isValidS3 = s3 >= minS3(upLinePT) && s3 <= maxS3(upLinePT);
  const isValidUS3 = uS3 >= minUS3(upLinePT) && uS3 <= maxUS3(upLinePT);
  const isValidM3 = m3 >= minM3(upLinePT, s3) && m3 <= maxM3(upLinePT, s3);
  const isValidM3Auto = m3 >= minM3Auto(upLinePT, uS3) && m3 <= maxM3Auto(upLinePT);
  const isValidN3 = n3 >= minN3() && n3 <= maxN3(m3);
  return (iS3 ? isValidUS3 && isValidM3Auto : isValidS3 && isValidM3) && isValidN3;
};

const copyMasterPTValues = (
  positionTakings: GroupItem[],
  itemCopied: GroupItem,
  isBelongCopiedGroup?: (item: GroupItem) => boolean
) => {
  const newItemCopied = itemCopied as MasterPositionTakingGroupItem;
  const { s3, uS3, iS3, m3, n3, isMultipleModeChecked, isUpdateMinPtOnly } = newItemCopied;
  if (!validateMasterPT(newItemCopied, newItemCopied)) return [];
  return (positionTakings as MasterPositionTakingGroupItem[]).map((item) => {
    if (isBelongCopiedGroup?.(item)) {
      return validateMasterPT(item, newItemCopied)
        ? { ...item, s3, uS3, iS3, m3, n3, isMultipleModeChecked, isUpdateMinPtOnly, copyStatus: CopyStatus.Success }
        : { ...item, isMultipleModeChecked, copyStatus: CopyStatus.Fail };
    }

    return { ...item, copyStatus: CopyStatus.NotCopy };
  });
};

type PositionTakingRowProps = {
  isReadOnly: boolean;
  productGroupId: number;
  ptItems: MasterPositionTakingGroupItem[];
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
  copyIconId,
  stateCopiedGroup,
  handleCopyPTs,
}: Readonly<PositionTakingRowProps>) {
  const ptGroups =
    productGroupId === ProductGroupIds.SportsBook ? Object.values(groupSportsBookPTBySportTypeId(ptItems)) : [ptItems];
  return (ptGroups as MasterPositionTakingGroupItem[][]).map((group, index) => {
    const firstGroupItem = group[0];
    const groupPTLabel = (
      <GroupLabel
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
              group={group}
              itemIndex={index}
              item={item}
              copyIconId={copyIconId}
              stateCopiedGroup={stateCopiedGroup}
              handleCopyPTs={handleCopyPTs}
            />
          );
          return (
            <MasterPositionTakingRow
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

export type ValidatePTResult = {
  id: string;
  isValid: boolean;
  message: string;
  msgParams?: { [key: string]: number };
};

const validateMasterPositionTaking = (item: MasterPositionTakingGroupItem): ValidatePTResult => {
  const { s3, uS3, iS3, m3, upline } = item;

  if (iS3) {
    if (uS3 < minUS3(upline)) {
      return {
        id: `uS3-${item.sportTypeId}-${item.betTypeId}`,
        isValid: uS3 >= minUS3(upline),
        message: 'SuperAutoPTCannotBeLessThan',
        msgParams: { value: minUS3(upline) / 100 },
      };
    }

    if (m3 < minM3Auto(upline, uS3)) {
      return {
        id: `m3Auto-${item.sportTypeId}-${item.betTypeId}`,
        isValid: m3 >= minM3Auto(upline, uS3),
        message: 'MasterMaxPTCannotBeLessThan',
        msgParams: { value: minM3Auto(upline, uS3) / 100 },
      };
    }
  } else {
    if (s3 < minS3(upline)) {
      return {
        id: `s3-${item.sportTypeId}-${item.betTypeId}`,
        isValid: s3 >= minS3(upline),
        message: 'SuperPTCannotBeLessThan',
        msgParams: { value: minS3(upline) / 100 },
      };
    }

    if (m3 < minM3(upline, s3)) {
      return {
        id: `m3-${item.sportTypeId}-${item.betTypeId}`,
        isValid: m3 >= minM3(upline, s3),
        message: 'MasterMaxPTCannotBeLessThan',
        msgParams: { value: minM3(upline, s3) / 100 },
      };
    }
  }

  return { id: '', isValid: true, message: '' };
};

const MasterPTBlockId = 'master-position-taking';

const MasterPositionTaking = forwardRef<ValidationRefType, PositionTakingBlockProps>(function MasterPositionTaking(
  props,
  ref
) {
  const { t } = useTranslation();
  const { title } = props;
  const { productGroups, isMultipleMode, handleUpdateSettingItems, productGroupId, isEditMode, updateGroupSetting } =
    useGroupSettingContext();
  const groupId = Number(productGroupId);
  const masterPTs = getSettingItems<MasterPositionTakingGroupItem>(productGroups, groupId, GroupTypeIds.PositionTaking);
  const [validateResult, setValidateResult] = useState<ValidatePTResult | undefined>();
  const { stateCopiedGroup, copyIconId, handleCopy } = usePTCopy();
  const blockRef = useRef<BlockRefType>(null);

  const validateMasterPTs = (masterPTs: MasterPositionTakingGroupItem[]) => {
    let validateResult = true;
    for (const item of masterPTs) {
      if (item.isMultipleModeChecked) {
        const error = validateMasterPositionTaking(item);
        if (!error.isValid) {
          setValidateResult(error);
          validateResult = false;
          break;
        }
      }
    }

    return validateResult;
  };

  useEffect(() => {
    const validateResult = validateMasterPTs(masterPTs);
    if (validateResult) setValidateResult(undefined);
  }, [masterPTs]);

  useImperativeHandle(ref, () => {
    const validate = () => {
      const validateResult = validateMasterPTs(masterPTs);

      if (!validateResult) {
        blockRef.current?.expand();
      }

      if (validateResult) {
        setValidateResult(undefined);
      }

      return validateResult;
    };

    return {
      validate,
      scrollToBlock: () => scrollToBLock(MasterPTBlockId),
    };
  });

  const handleCopyPTs = (
    itemCopied: GroupItem,
    iconId: number | undefined,
    isBelongCopiedGroup?: (item: GroupItem) => boolean
  ) => {
    if (!itemCopied.isMultipleModeChecked) return;
    const newPTs = copyMasterPTValues(masterPTs, itemCopied, isBelongCopiedGroup);
    if (newPTs.length > 0) {
      handleCopy(newPTs, iconId);
      handleUpdateSettingItems?.(groupId, GroupTypeIds.PositionTaking, newPTs);
    }
  };

  const handleSelectAll = (isChecked: boolean) => {
    const newMasterPTs = masterPTs.map((item) => ({
      ...item,
      isMultipleModeChecked: isChecked,
      isUpdateMinPtOnly: false,
    }));
    handleUpdateSettingItems?.(groupId, GroupTypeIds.PositionTaking, newMasterPTs);
  };

  return (
    <Block
      id={MasterPTBlockId}
      title={title}
      ref={blockRef}
      isPartialUpdate={isEditMode === true && !props.isReadOnly}
      onSaveClick={() => updateGroupSetting?.(GroupTypeIds.PositionTaking)}
    >
      <BlockRow
        isHeaderRow
        label={
          isMultipleMode &&
          masterPTs.length > 1 &&
          !props.isReadOnly && (
            <div style={{ display: 'flex', marginLeft: '2px' }}>
              <input
                type="checkbox"
                style={{ top: '0' }}
                onChange={(e) => handleSelectAll(e.target.checked)}
                checked={masterPTs.every((item) => item.isMultipleModeChecked === true)}
              />
              <div>{t('SelectAll')}</div>
            </div>
          )
        }
      >
        {['SuperPT', 'IsAuto', 'MasterMaxPT', 'MasterMinPT'].map((name) => (
          <BlockColumn key={name}>{t(name)}</BlockColumn>
        ))}
      </BlockRow>
      <PositionTakingRows
        isReadOnly={props.isReadOnly}
        productGroupId={groupId}
        ptItems={masterPTs}
        copyIconId={copyIconId}
        stateCopiedGroup={stateCopiedGroup}
        handleCopyPTs={props.isReadOnly ? () => void 0 : handleCopyPTs}
      />
      {validateResult && !validateResult.isValid && (
        <WrapperTooltip
          type="error"
          anchorSelect={`#${validateResult.id}`}
          isOpen={!validateResult.isValid}
          content={t(validateResult.message, validateResult?.msgParams)}
        />
      )}
    </Block>
  );
});

export default MasterPositionTaking;
