import classNames from 'classnames';
import { forwardRef, useEffect, useImperativeHandle, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { ValidationRefType } from '@customer-web-app/share-models';
import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import {
    GroupItem, SuperEditMemberPositionTakingGroupItem, SuperEditMemberPositionTakingProps
} from '@customer-web-app/shared-group-settings-types';
import { Block, BlockColumn, BlockRefType, BlockRow } from '@customer-web-app/shared/base/block';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import scrollToBLock from '../scroll-helper';
import { ValidatePTResult } from './master-position-taking';
import { GroupLabel } from './position-taking-group-label';
import PTLabel from './position-taking-label';
import PTSelect from './pt-select';
import styles from './pt-select.module.scss';
import { PositionTakingBlockProps } from './types';
import {
    CopyStatus, CopyStatusPT, groupSportsBookPTBySportTypeId, isCopyingPTItem, PTRowProps, renderNoneBlockColumn,
    usePTCopy
} from './utils';

function minS1(item: SuperEditMemberPositionTakingProps) {
  const { m1, a1, n4 } = item;
  return Math.max(0, n4 - m1 - a1);
}

function maxS1(item: SuperEditMemberPositionTakingProps) {
  const { n4, m1, a1, s4, m3 } = item;
  return Math.max(n4 - m1 - a1, s4 - m3);
}

function minUS1(item: SuperEditMemberPositionTakingProps) {
  const { m1, a1, n4 } = item;

  return Math.max(0, n4 - m1 - a1);
}

function maxUS1(item: SuperEditMemberPositionTakingProps) {
  const { s4 } = item;
  return s4;
}

function SuperEditMemberPositionTakingRow({
  item,
  label,
  copyStatus,
}: Readonly<PTRowProps<SuperEditMemberPositionTakingGroupItem>>) {
  const { handleSettingItemChange, productGroupId } = useGroupSettingContext();

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
        {item.iS1 ? (
          <div id={`uS1-${item.sportTypeId}-${item.betTypeId}`} className={classNames('center', copyRowStyle)}>
            <PTSelect
              min={minUS1(item)}
              max={maxUS1(item)}
              step={item.step}
              value={item.uS1}
              isDisable={!item.isMultipleModeChecked}
              onChange={(e) => {
                handlePTChange({ uS1: Number(e.target.value) });
              }}
            />
          </div>
        ) : (
          <div id={`s1-${item.sportTypeId}-${item.betTypeId}`} className={classNames('center', copyRowStyle)}>
            <PTSelect
              min={minS1(item)}
              max={maxS1(item)}
              step={item.step}
              value={item.s1}
              isDisable={!item.isMultipleModeChecked || item.iS3}
              onChange={(e) => {
                handlePTChange({ s1: Number(e.target.value) });
              }}
            />
          </div>
        )}
      </BlockColumn>
      <BlockColumn>
        <div className={classNames('center copy-checkbox', copyRowStyle)}>
          <input
            type="checkbox"
            checked={item.iS1}
            disabled={!item.isMultipleModeChecked}
            onChange={(e) => {
              handlePTChange({
                iS1: e.target.checked,
                uS1: e.target.checked && item.uS1 < minUS1(item) ? minUS1(item) : item.uS1,
              });
            }}
          />
        </div>
      </BlockColumn>
      <BlockColumn>
        <div className={classNames('center', copyRowStyle)}>
          <div>{item.m1 / 100}</div>
        </div>
      </BlockColumn>
      <BlockColumn>
        <div className={classNames('center', copyRowStyle)}>
          <div>{item.a1 / 100}</div>
        </div>
      </BlockColumn>
    </BlockRow>
  );
}

const validateMasterPT = (
  currentPT: SuperEditMemberPositionTakingGroupItem,
  newPT: SuperEditMemberPositionTakingProps
) => {
  const upLinePT = currentPT.upline;
  const { s1, uS1, iS1 } = newPT;
  const isValidS1 = s1 >= minS1(upLinePT) && s1 <= maxS1(upLinePT);
  const isValidUS1 = uS1 >= minUS1(upLinePT) && uS1 <= maxUS1(upLinePT);
  return iS1 ? isValidUS1 : isValidS1;
};

const copyMasterPTValues = (
  positionTakings: GroupItem[],
  itemCopied: GroupItem,
  isBelongCopiedGroup?: (item: GroupItem) => boolean
) => {
  const newItemCopied = itemCopied as SuperEditMemberPositionTakingGroupItem;
  const { s1, uS1, iS1, isMultipleModeChecked } = newItemCopied;
  if (!validateMasterPT(newItemCopied, newItemCopied)) return [];
  return (positionTakings as SuperEditMemberPositionTakingGroupItem[]).map((item) => {
    if (isBelongCopiedGroup?.(item)) {
      if (validateMasterPT(item, newItemCopied)) {
        return !item.iS1 && item.iS3 && !iS1
          ? { ...item, isMultipleModeChecked, copyStatus: CopyStatus.NotCopy }
          : { ...item, s1: item.iS3 ? item.s1 : s1, uS1, iS1, isMultipleModeChecked, copyStatus: CopyStatus.Success };
      }
      return {
        ...item,
        isMultipleModeChecked,
        copyStatus: !item.iS1 && item.iS3 ? CopyStatus.NotCopy : CopyStatus.Fail,
      };
    }

    return { ...item, copyStatus: CopyStatus.NotCopy };
  });
};

type PositionTakingRowProps = {
  productGroupId: number;
  ptItems: SuperEditMemberPositionTakingGroupItem[];
  copyIconId: number | undefined;
  stateCopiedGroup: CopyStatusPT[];
  handleCopyPTs: (
    itemCopied: GroupItem,
    iconId: number | undefined,
    isBelongCopiedGroup?: (item: GroupItem) => boolean
  ) => void;
};

function PositionTakingRows({
  productGroupId,
  ptItems,
  copyIconId,
  stateCopiedGroup,
  handleCopyPTs,
}: Readonly<PositionTakingRowProps>) {
  const ptGroups =
    productGroupId === ProductGroupIds.SportsBook ? Object.values(groupSportsBookPTBySportTypeId(ptItems)) : [ptItems];
  return (ptGroups as SuperEditMemberPositionTakingGroupItem[][]).map((group, index) => {
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
            <SuperEditMemberPositionTakingRow
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

const validateSuperEditMemberPositionTaking = (item: SuperEditMemberPositionTakingGroupItem): ValidatePTResult => {
  const { iS1, uS1, s1, iS3, upline } = item;

  if (iS1) {
    if (uS1 < minUS1(upline)) {
      return {
        id: `uS1-${item.sportTypeId}-${item.betTypeId}`,
        isValid: uS1 >= minUS1(upline),
        message: 'SuperAutoPTOnMemberCannotBeLessThan',
        msgParams: { value: minUS1(upline) / 100 },
      };
    }

    if (uS1 > maxUS1(upline)) {
      return {
        id: `uS1-${item.sportTypeId}-${item.betTypeId}`,
        isValid: uS1 <= maxUS1(upline),
        message: 'SuperAutoPTOnMemberCannotBeGreaterThanSuperMaxPT',
      };
    }
  } else {
    if (s1 < minS1(upline) && !iS3) {
      return {
        id: `s1-${item.sportTypeId}-${item.betTypeId}`,
        isValid: s1 >= minS1(upline),
        message: 'SuperPTOnMemberCannotBeLessThan',
        msgParams: { value: minS1(upline) / 100 },
      };
    }

    if (s1 > maxS1(upline) && !iS3) {
      return {
        id: `s1-${item.sportTypeId}-${item.betTypeId}`,
        isValid: s1 <= maxS1(upline),
        message: 'SuperPTOnMemberCannotBeGreaterThan',
        msgParams: { value: maxS1(upline) / 100 },
      };
    }
  }

  return {
    id: '',
    isValid: true,
    message: '',
  };
};

const SuperEditMemberPTBlockId = 'super-edit-member-position-taking';

const SuperEditMemberPositionTaking = forwardRef<ValidationRefType, PositionTakingBlockProps>(
  function SuperEditMemberPositionTaking(props, ref) {
    const { t } = useTranslation();
    const { title } = props;
    const { productGroups, isMultipleMode, handleUpdateSettingItems, productGroupId, isEditMode, updateGroupSetting } =
      useGroupSettingContext();
    const groupId = Number(productGroupId);
    const superPTOnMembers = getSettingItems<SuperEditMemberPositionTakingGroupItem>(
      productGroups,
      groupId,
      GroupTypeIds.PositionTaking
    );
    const [validateResult, setValidateResult] = useState<ValidatePTResult | undefined>();
    const { stateCopiedGroup, copyIconId, handleCopy } = usePTCopy();
    const blockRef = useRef<BlockRefType>(null);

    const validateSuperEditMemberPTs = (superPTs: SuperEditMemberPositionTakingGroupItem[]) => {
      let validateResult = true;
      for (const item of superPTs) {
        if (item.isMultipleModeChecked) {
          const error = validateSuperEditMemberPositionTaking(item);
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
      const validateResult = validateSuperEditMemberPTs(superPTOnMembers);
      if (validateResult) setValidateResult(undefined);
    }, [superPTOnMembers]);

    useImperativeHandle(ref, () => {
      const validate = () => {
        const validateResult = validateSuperEditMemberPTs(superPTOnMembers);

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
        scrollToBlock: () => scrollToBLock(SuperEditMemberPTBlockId),
      };
    });

    const handleCopyPTs = (
      itemCopied: GroupItem,
      iconId: number | undefined,
      isBelongCopiedGroup?: (item: GroupItem) => boolean
    ) => {
      if (!itemCopied.isMultipleModeChecked) return;
      const newPTs = copyMasterPTValues(superPTOnMembers, itemCopied, isBelongCopiedGroup);
      if (newPTs.length > 0) {
        handleCopy(newPTs, iconId);
        handleUpdateSettingItems?.(groupId, GroupTypeIds.PositionTaking, newPTs);
      }
    };

    const handleSelectAll = (isChecked: boolean) => {
      const newSuperPTs = superPTOnMembers.map((item) => ({ ...item, isMultipleModeChecked: isChecked }));
      handleUpdateSettingItems?.(groupId, GroupTypeIds.PositionTaking, newSuperPTs);
    };

    return (
      <Block
        ref={blockRef}
        title={title}
        id={SuperEditMemberPTBlockId}
        isPartialUpdate={isEditMode === true && !props.isReadOnly}
        onSaveClick={() => updateGroupSetting?.(GroupTypeIds.PositionTaking)}
      >
        <BlockRow
          isHeaderRow
          label={
            isMultipleMode && (
              <div style={{ display: 'flex', marginLeft: '2px' }}>
                <input
                  type="checkbox"
                  style={{ top: '0' }}
                  onChange={(e) => handleSelectAll(e.target.checked)}
                  checked={superPTOnMembers.every((item) => item.isMultipleModeChecked === true)}
                />
                <div>{t('SelectAll')}</div>
              </div>
            )
          }
        >
          {['SuperPT', 'IsAuto', 'MasterPT', 'agentPT'].map((name) => (
            <BlockColumn key={name}>{t(name)}</BlockColumn>
          ))}
        </BlockRow>
        <PositionTakingRows
          productGroupId={groupId}
          ptItems={superPTOnMembers}
          copyIconId={copyIconId}
          stateCopiedGroup={stateCopiedGroup}
          handleCopyPTs={handleCopyPTs}
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
  }
);

export default SuperEditMemberPositionTaking;
