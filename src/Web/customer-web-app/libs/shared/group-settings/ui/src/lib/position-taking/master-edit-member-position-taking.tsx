import classNames from 'classnames';
import { forwardRef, useEffect, useImperativeHandle, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { ValidationRefType } from '@customer-web-app/share-models';
import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import {
  GroupItem,
  MasterEditMemberPositionTakingGroupItem,
  MasterEditMemberPositionTakingProps,
} from '@customer-web-app/shared-group-settings-types';
import { Block, BlockColumn, BlockRefType, BlockRow } from '@customer-web-app/shared/base/block';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import { scrollToBLock } from '../scroll-helper';
import { ValidatePTResult } from './master-position-taking';
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

function minM1(item: MasterEditMemberPositionTakingProps) {
  const { n3, a1 } = item;
  return Math.max(0, n3 - a1);
}

function maxM1(item: MasterEditMemberPositionTakingProps) {
  const { a1, m3 } = item;
  return m3 - a1;
}

function minUM1(item: MasterEditMemberPositionTakingProps) {
  const { n3, a1 } = item;
  return Math.max(0, n3 - a1);
}

function maxUM1(item: MasterEditMemberPositionTakingProps) {
  const { m3 } = item;
  return m3;
}

function MasterEditMemberPositionTakingRow({
  isReadOnly,
  item,
  label,
  copyStatus,
  isDisableMasterAutoPT,
}: Readonly<PTRowProps<MasterEditMemberPositionTakingGroupItem>>) {
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
        {item.iM1 ? (
          <div id={`uM1-${item.sportTypeId}-${item.betTypeId}`} className={classNames('center', copyRowStyle)}>
            <PTSelect
              min={minUM1(item)}
              max={maxUM1(item)}
              step={item.step}
              value={item.uM1}
              isDisable={isReadOnly || !item.isMultipleModeChecked}
              onChange={(e) => {
                handlePTChange({ uM1: Number(e.target.value) });
              }}
            />
          </div>
        ) : (
          <div id={`m1-${item.sportTypeId}-${item.betTypeId}`} className={classNames('center', copyRowStyle)}>
            <PTSelect
              min={minM1(item)}
              max={maxM1(item)}
              step={item.step}
              value={item.m1}
              isDisable={isReadOnly || !item.isMultipleModeChecked || item.iM2}
              onChange={(e) => {
                handlePTChange({ m1: Number(e.target.value) });
              }}
            />
          </div>
        )}
      </BlockColumn>
      {!isDisableMasterAutoPT && (
        <BlockColumn>
          <div className={classNames('center copy-checkbox', copyRowStyle)}>
            <input
              type="checkbox"
              checked={item.iM1}
              disabled={isReadOnly || !item.isMultipleModeChecked}
              onChange={(e) => {
                handlePTChange({
                  iM1: e.target.checked,
                  uM1: e.target.checked && item.uM1 < minUM1(item) ? minUM1(item) : item.uM1,
                });
              }}
            />
          </div>
        </BlockColumn>
      )}
      <BlockColumn>
        <div className={classNames('center', copyRowStyle)}>
          <div>{item.a1 / 100}</div>
        </div>
      </BlockColumn>
    </BlockRow>
  );
}

const validateMasterPT = (
  currentPT: MasterEditMemberPositionTakingGroupItem,
  newPT: MasterEditMemberPositionTakingProps
) => {
  const uplinePT = currentPT.upline;
  const { iM1, m1, uM1 } = newPT;
  const isValidM1 = m1 >= minM1(uplinePT) && m1 <= maxM1(uplinePT);
  const isValidUM1 = uM1 >= minUM1(uplinePT) && uM1 <= maxUM1(uplinePT);

  return iM1 ? isValidUM1 : isValidM1;
};

const copyMasterPTValues = (
  positionTakings: GroupItem[],
  itemCopied: GroupItem,
  isBelongCopiedGroup?: (item: GroupItem) => boolean
) => {
  const newItemCopied = itemCopied as MasterEditMemberPositionTakingGroupItem;
  const { m1, uM1, iM1, isMultipleModeChecked } = newItemCopied;

  if (!validateMasterPT(newItemCopied, newItemCopied)) return [];

  return (positionTakings as MasterEditMemberPositionTakingGroupItem[]).map((item) => {
    if (isBelongCopiedGroup?.(item)) {
      if (validateMasterPT(item, newItemCopied)) {
        return !item.iM1 && item.iM2 && !iM1
          ? { ...item, isMultipleModeChecked, copyStatus: CopyStatus.NotCopy }
          : { ...item, m1: item.iM2 ? item.m1 : m1, uM1, iM1, isMultipleModeChecked, copyStatus: CopyStatus.Success };
      }
      return {
        ...item,
        isMultipleModeChecked,
        copyStatus: !item.iM1 && item.iM2 ? CopyStatus.NotCopy : CopyStatus.Fail,
      };
    }

    return { ...item, copyStatus: CopyStatus.NotCopy };
  });
};

type PositionTakingRowProps = {
  isReadOnly: boolean;
  productGroupId: number;
  ptItems: MasterEditMemberPositionTakingGroupItem[];
  copyIconId: number | undefined;
  stateCopiedGroup: CopyStatusPT[];
  handleCopyPTs: (
    itemCopied: GroupItem,
    iconId: number | undefined,
    isBelongCopiedGroup?: (item: GroupItem) => boolean
  ) => void;
  isDisableMasterAutoPT?: boolean;
};

function PositionTakingRows({
  isReadOnly,
  productGroupId,
  ptItems,
  copyIconId,
  stateCopiedGroup,
  isDisableMasterAutoPT,
  handleCopyPTs,
}: Readonly<PositionTakingRowProps>) {
  const ptGroups =
    productGroupId === ProductGroupIds.SportsBook ? Object.values(groupSportsBookPTBySportTypeId(ptItems)) : [ptItems];
  return (ptGroups as MasterEditMemberPositionTakingGroupItem[][]).map((group, index) => {
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
              {renderNoneBlockColumn(
                `${firstGroupItem.sportTypeId}${firstGroupItem.betTypeId}`,
                isDisableMasterAutoPT ? 2 : 3
              )}
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
            <MasterEditMemberPositionTakingRow
              isReadOnly={isReadOnly}
              key={`${item.sportTypeId}-${item.betTypeId}-${item.isLive}`}
              item={item}
              label={ptLabel}
              copyStatus={isCopyingPTItem(stateCopiedGroup, item)}
              isDisableMasterAutoPT={isDisableMasterAutoPT}
            />
          );
        })}
      </div>
    );
  });
}

const validateMasterEditMemberPositionTaking = (item: MasterEditMemberPositionTakingGroupItem): ValidatePTResult => {
  const { uM1, m1, iM1, iM2, upline } = item;

  if (iM1) {
    if (uM1 < minUM1(upline)) {
      return {
        id: `uM1-${item.sportTypeId}-${item.betTypeId}`,
        isValid: uM1 >= minUM1(upline),
        message: 'MasterAutoPTOnMemberCannotBeLessThan',
        msgParams: { value: minUM1(upline) / 100 },
      };
    }

    if (uM1 > maxUM1(upline)) {
      return {
        id: `uM1-${item.sportTypeId}-${item.betTypeId}`,
        isValid: uM1 <= maxUM1(upline),
        message: 'MasterAutoPTOnMemberCannotBeGreaterThanMasterMaxPT',
      };
    }
  } else {
    if (m1 < minM1(upline) && !iM2) {
      return {
        id: `m1-${item.sportTypeId}-${item.betTypeId}`,
        isValid: uM1 >= minM1(upline),
        message: 'MasterPTOnMemberCannotBeLessThan',
        msgParams: { value: minM1(upline) / 100 },
      };
    }

    if (m1 > maxM1(upline) && !iM2) {
      return {
        id: `m1-${item.sportTypeId}-${item.betTypeId}`,
        isValid: m1 <= maxM1(upline),
        message: 'MasterPTOnMemberCannotBeGreaterThan',
        msgParams: { value: maxM1(upline) / 100 },
      };
    }
  }

  return {
    id: '',
    isValid: true,
    message: '',
  };
};

const MasterEditMemberPTBlockId = 'master-edit-member-position-taking';

const MasterEditMemberPositionTaking = forwardRef<ValidationRefType, PositionTakingBlockProps>(
  function MasterEditMemberPositionTaking(props, ref) {
    const { t } = useTranslation();
    const { title, isDisableMasterAutoPT } = props;
    const { productGroups, isMultipleMode, handleUpdateSettingItems, productGroupId, isEditMode, updateGroupSetting } =
      useGroupSettingContext();
    const groupId = Number(productGroupId);
    const masterPTOnMembers = getSettingItems<MasterEditMemberPositionTakingGroupItem>(
      productGroups,
      groupId,
      GroupTypeIds.PositionTaking
    );
    const [validateResult, setValidateResult] = useState<ValidatePTResult | undefined>();
    const { stateCopiedGroup, copyIconId, handleCopy } = usePTCopy();
    const blockRef = useRef<BlockRefType>(null);

    const validateMasterEditMemberPTs = (masterPTs: MasterEditMemberPositionTakingGroupItem[]) => {
      let validateResult = true;
      for (const item of masterPTs) {
        if (item.isMultipleModeChecked) {
          const error = validateMasterEditMemberPositionTaking(item);
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
      const validateResult = validateMasterEditMemberPTs(masterPTOnMembers);
      if (validateResult) setValidateResult(undefined);
    }, [masterPTOnMembers]);

    useImperativeHandle(ref, () => {
      const validate = () => {
        const validateResult = validateMasterEditMemberPTs(masterPTOnMembers);

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
        scrollToBlock: () => scrollToBLock(MasterEditMemberPTBlockId),
      };
    });

    const handleCopyPTs = (
      itemCopied: GroupItem,
      iconId: number | undefined,
      isBelongCopiedGroup?: (item: GroupItem) => boolean
    ) => {
      if (!itemCopied.isMultipleModeChecked) return;
      const newPTs = copyMasterPTValues(masterPTOnMembers, itemCopied, isBelongCopiedGroup);
      if (newPTs.length > 0) {
        handleCopy(newPTs, iconId);
        handleUpdateSettingItems?.(groupId, GroupTypeIds.PositionTaking, newPTs);
      }
    };

    const handleSelectAll = (isChecked: boolean) => {
      const newMemberPTs = masterPTOnMembers.map((item) => ({ ...item, isMultipleModeChecked: isChecked }));
      handleUpdateSettingItems?.(groupId, GroupTypeIds.PositionTaking, newMemberPTs);
    };

    return (
      <Block
        title={title}
        ref={blockRef}
        id={MasterEditMemberPTBlockId}
        isPartialUpdate={isEditMode === true && !props.isReadOnly}
        onSaveClick={() => updateGroupSetting?.(GroupTypeIds.PositionTaking)}
      >
        <BlockRow
          isHeaderRow
          label={
            isMultipleMode &&
            masterPTOnMembers.length > 1 &&
            !props.isReadOnly && (
              <div style={{ display: 'flex', marginLeft: '2px' }}>
                <input
                  type="checkbox"
                  style={{ top: '0' }}
                  onChange={(e) => handleSelectAll(e.target.checked)}
                  checked={masterPTOnMembers.every((item) => item.isMultipleModeChecked === true)}
                />
                <div>{t('SelectAll')}</div>
              </div>
            )
          }
        >
          <BlockColumn>{t('MasterPT')}</BlockColumn>
          {!isDisableMasterAutoPT && <BlockColumn>{t('IsAuto')}</BlockColumn>}
          <BlockColumn>{t('agentPT')}</BlockColumn>
        </BlockRow>
        <PositionTakingRows
          isReadOnly={props.isReadOnly}
          productGroupId={groupId}
          ptItems={masterPTOnMembers}
          copyIconId={copyIconId}
          stateCopiedGroup={stateCopiedGroup}
          handleCopyPTs={handleCopyPTs}
          isDisableMasterAutoPT={isDisableMasterAutoPT}
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

export default MasterEditMemberPositionTaking;
