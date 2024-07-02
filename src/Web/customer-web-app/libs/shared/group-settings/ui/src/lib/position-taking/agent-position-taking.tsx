import classNames from 'classnames';
import { forwardRef, useEffect, useImperativeHandle, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { ValidationRefType } from '@customer-web-app/share-models';
import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import {
  AgentPositionTakingGroupItem,
  AgentPositionTakingProps,
  GroupItem,
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

function minM2(agentPositionTakingGroupItem: AgentPositionTakingProps) {
  const { n3, a4 } = agentPositionTakingGroupItem;
  return Math.max(0, n3 - a4);
}

export function maxM2(agentPositionTakingGroupItem: AgentPositionTakingProps) {
  const { m3 } = agentPositionTakingGroupItem;
  return m3;
}

function minA2(agentPositionTakingGroupItem: AgentPositionTakingProps, m2: number) {
  const { n3 } = agentPositionTakingGroupItem;
  return Math.max(0, n3 - m2);
}

function maxA2(agentPositionTakingGroupItem: AgentPositionTakingProps, m2: number) {
  const { m3, a4 } = agentPositionTakingGroupItem;
  return Math.min(m3 - m2, a4);
}

function minUM2(agentPositionTakingGroupItem: AgentPositionTakingProps) {
  const { n3, a4 } = agentPositionTakingGroupItem;
  return Math.max(0, n3 - a4);
}

function maxUM2(agentPositionTakingGroupItem: AgentPositionTakingProps) {
  const { m3 } = agentPositionTakingGroupItem;
  return m3;
}

function minA2Auto(agentPositionTakingGroupItem: AgentPositionTakingProps, uM2: number) {
  const { n3 } = agentPositionTakingGroupItem;
  return Math.max(0, n3 - uM2);
}

function maxA2Auto(agentPositionTakingGroupItem: AgentPositionTakingProps) {
  const { m3, a4 } = agentPositionTakingGroupItem;
  return Math.min(m3, a4);
}

function minN2() {
  return 0;
}

function maxN2(a2: number) {
  return a2;
}

function AgentPositionTakingRow({
  isReadOnly,
  item,
  label,
  copyStatus,
  isDisableMasterAutoPT,
}: Readonly<PTRowProps<AgentPositionTakingGroupItem>>) {
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
        {item.iM2 ? (
          <div id={`uM2-${item.sportTypeId}-${item.betTypeId}`} className={classNames('center', copyRowStyle)}>
            <PTSelect
              min={minUM2(item.upline)}
              max={maxUM2(item.upline)}
              step={item.step}
              value={item.uM2}
              isDisable={isReadOnly || !item.isMultipleModeChecked}
              onChange={(e) => {
                const uM2 = Number(e.target.value);
                const agentMaxPT = maxA2Auto(item);
                const newA2 = item.a2 > agentMaxPT ? agentMaxPT : item.a2;
                const a2 = Math.max(newA2, minA2(item, uM2));
                const n2 = item.n2 > a2 ? a2 : item.n2;
                handlePTChange({ uM2, a2, n2 });
              }}
            />
          </div>
        ) : (
          <div id={`m2-${item.sportTypeId}-${item.betTypeId}`} className={classNames('center', copyRowStyle)}>
            <PTSelect
              min={minM2(item.upline)}
              max={maxM2(item.upline)}
              step={item.step}
              value={item.m2}
              isDisable={isReadOnly || !item.isMultipleModeChecked}
              onChange={(e) => {
                const m2 = Number(e.target.value);
                const agentMaxPT = maxA2(item, m2);
                const a2 = Math.max(agentMaxPT, minA2(item, m2));
                const n2 = item.n2 > a2 ? a2 : item.n2;
                handlePTChange({ m2, a2, n2 });
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
              checked={item.iM2}
              disabled={isReadOnly || !item.isMultipleModeChecked}
              onChange={(e) => {
                const isAuto = e.target.checked;
                const m2 = item.m2 > maxM2(item) ? maxM2(item) : item.m2;
                const uM2 = item.uM2 < minUM2(item) ? minUM2(item) : item.uM2;
                const newA2Auto = item.a2 < minA2Auto(item, uM2) ? minA2Auto(item, uM2) : item.a2;
                const newA2 = item.a2 > maxA2(item, m2) ? maxA2(item, m2) : Math.max(minA2(item, m2), item.a2);
                const a2 = isAuto ? newA2Auto : newA2;
                const n2 = item.n2 > a2 ? a2 : item.n2;
                isAuto ? handlePTChange({ iM2: isAuto, a2, n2, uM2 }) : handlePTChange({ iM2: isAuto, a2, n2, m2 });
              }}
            />
          </div>
        </BlockColumn>
      )}

      <BlockColumn>
        <div
          id={item.iM2 ? `a2Auto-${item.sportTypeId}-${item.betTypeId}` : `a2-${item.sportTypeId}-${item.betTypeId}`}
          className={classNames('center', copyRowStyle)}
        >
          <PTSelect
            min={item.iM2 ? minA2Auto(item.upline, item.uM2) : minA2(item.upline, item.m2)}
            max={item.iM2 ? maxA2Auto(item.upline) : maxA2(item.upline, item.m2)}
            step={item.step}
            value={item.a2}
            isDisable={isReadOnly || !item.isMultipleModeChecked}
            onChange={(e) => {
              const a2 = Number(e.target.value);
              const n2 = item.n2 > a2 ? a2 : item.n2;
              handlePTChange({ a2, n2 });
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
            min={minN2()}
            max={maxN2(item.a2)}
            step={item.step}
            value={item.n2}
            isDisable={isReadOnly || (!item.isMultipleModeChecked && !item.isUpdateMinPtOnly)}
            onChange={(e) => handlePTChange({ n2: Number(e.target.value) })}
          />
        </div>
      </BlockColumn>
    </BlockRow>
  );
}

const validateAgentPT = (currentPT: AgentPositionTakingGroupItem, newPT: AgentPositionTakingProps) => {
  const uplinePT = currentPT.upline;
  const { m2, uM2, iM2, a2, n2 } = newPT;
  const isValidM2 = m2 >= minM2(uplinePT) && m2 <= maxM2(uplinePT);
  const isValidUM2 = uM2 >= minUM2(uplinePT) && uM2 <= maxUM2(uplinePT);
  const isValidA2 = a2 >= minA2(uplinePT, m2) && a2 <= maxA2(uplinePT, m2);
  const isValidA2Auto = a2 >= minA2Auto(uplinePT, uM2) && a2 <= maxA2Auto(uplinePT);
  const isValidN2 = n2 >= minN2() && n2 <= maxN2(a2);
  return (iM2 ? isValidUM2 && isValidA2Auto : isValidM2 && isValidA2) && isValidN2;
};

const copyAgentPTs = (
  positionTakings: GroupItem[],
  itemCopied: GroupItem,
  isBelongCopiedGroup?: (item: GroupItem) => boolean
) => {
  const newItemCopied = itemCopied as AgentPositionTakingGroupItem;
  if (!validateAgentPT(newItemCopied, newItemCopied)) return [];
  const { m2, uM2, iM2, a2, n2, isMultipleModeChecked, isUpdateMinPtOnly } = newItemCopied;
  return (positionTakings as AgentPositionTakingGroupItem[]).map((item) => {
    if (isBelongCopiedGroup?.(item)) {
      return validateAgentPT(item, newItemCopied)
        ? { ...item, m2, uM2, iM2, a2, n2, isMultipleModeChecked, isUpdateMinPtOnly, copyStatus: CopyStatus.Success }
        : { ...item, isMultipleModeChecked, copyStatus: CopyStatus.Fail };
    }
    return { ...item, copyStatus: CopyStatus.NotCopy };
  });
};

type PositionTakingRowProps = {
  isReadOnly: boolean;
  productGroupId: number;
  ptItems: AgentPositionTakingGroupItem[];
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
  return (ptGroups as AgentPositionTakingGroupItem[][]).map((group, index) => {
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
              {renderNoneBlockColumn(
                `${firstGroupItem.sportTypeId}${firstGroupItem.betTypeId}`,
                isDisableMasterAutoPT ? 3 : 4
              )}
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
            <AgentPositionTakingRow
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

const validateAgentPositionTaking = (item: AgentPositionTakingGroupItem): ValidatePTResult => {
  const { m2, uM2, iM2, a2, upline } = item;

  if (iM2) {
    if (uM2 < minUM2(upline)) {
      return {
        id: `uM2-${item.sportTypeId}-${item.betTypeId}`,
        isValid: uM2 >= minUM2(upline),
        message: 'MasterAutoPTCannotBeGreaterThanMasterMaxPT',
      };
    }

    if (a2 < minA2Auto(upline, uM2)) {
      return {
        id: `a2Auto-${item.sportTypeId}-${item.betTypeId}`,
        isValid: a2 >= minA2Auto(upline, uM2),
        message: 'AgentMaxPTCannotBeLessThan',
        msgParams: { value: minA2Auto(upline, uM2) / 100 },
      };
    }
  } else {
    if (m2 < minM2(upline)) {
      return {
        id: `m2-${item.sportTypeId}-${item.betTypeId}`,
        isValid: m2 >= minM2(upline),
        message: 'MasterPTCannotBeLessThan',
        msgParams: { value: minM2(upline) / 100 },
      };
    }

    if (a2 < minA2(upline, m2)) {
      return {
        id: `a2-${item.sportTypeId}-${item.betTypeId}`,
        isValid: a2 >= minA2(upline, m2),
        message: 'AgentMaxPTCannotBeLessThan',
        msgParams: { value: minA2(upline, m2) / 100 },
      };
    }
  }

  return {
    id: '',
    isValid: true,
    message: '',
  };
};

const AgentPTBlockId = 'agent-position-taking';

const AgentPositionTaking = forwardRef<ValidationRefType, PositionTakingBlockProps>(function AgentPositionTaking(
  { isReadOnly, title, isDisableMasterAutoPT },
  ref
) {
  const { t } = useTranslation();
  const { productGroups, isMultipleMode, handleUpdateSettingItems, productGroupId, isEditMode, updateGroupSetting } =
    useGroupSettingContext();
  const groupId = Number(productGroupId);
  const agentPTs = getSettingItems<AgentPositionTakingGroupItem>(productGroups, groupId, GroupTypeIds.PositionTaking);
  const [validateResult, setValidateResult] = useState<ValidatePTResult | undefined>();
  const { stateCopiedGroup, copyIconId, handleCopy } = usePTCopy();
  const blockRef = useRef<BlockRefType>(null);

  const validateAgentPTs = (agentPTs: AgentPositionTakingGroupItem[]) => {
    let validateResult = true;
    for (const item of agentPTs) {
      if (item.isMultipleModeChecked) {
        const error = validateAgentPositionTaking(item);
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
    const validateResult = validateAgentPTs(agentPTs);
    if (validateResult) setValidateResult(undefined);
  }, [agentPTs]);

  useImperativeHandle(ref, () => {
    const validate = () => {
      const validateResult = validateAgentPTs(agentPTs);

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
      scrollToBlock: () => scrollToBLock(AgentPTBlockId),
    };
  });

  const handleCopyPTs = (
    itemCopied: GroupItem,
    iconId: number | undefined,
    isBelongCopiedGroup?: (item: GroupItem) => boolean
  ) => {
    if (!itemCopied.isMultipleModeChecked) return;
    const newPTs = copyAgentPTs(agentPTs, itemCopied, isBelongCopiedGroup);
    if (newPTs.length > 0) {
      handleCopy(newPTs, iconId);
      handleUpdateSettingItems?.(groupId, GroupTypeIds.PositionTaking, newPTs);
    }
  };

  const handleSelectAll = (isChecked: boolean) => {
    const newAgentPTs = agentPTs.map((item) => ({
      ...item,
      isMultipleModeChecked: isChecked,
      isUpdateMinPtOnly: false,
    }));
    handleUpdateSettingItems?.(groupId, GroupTypeIds.PositionTaking, newAgentPTs);
  };

  return (
    <Block
      id={AgentPTBlockId}
      title={title}
      ref={blockRef}
      isPartialUpdate={isEditMode === true && !isReadOnly}
      onSaveClick={() => updateGroupSetting?.(GroupTypeIds.PositionTaking)}
    >
      <BlockRow
        isHeaderRow
        label={
          isMultipleMode &&
          agentPTs.length > 1 &&
          !isReadOnly && (
            <div style={{ display: 'flex', marginLeft: '2px' }}>
              <input
                type="checkbox"
                style={{ top: '0' }}
                onChange={(e) => handleSelectAll(e.target.checked)}
                checked={agentPTs.every((item) => item.isMultipleModeChecked === true)}
              />
              <div>{t('SelectAll')}</div>
            </div>
          )
        }
      >
        <BlockColumn>{t('MasterPT')}</BlockColumn>
        {!isDisableMasterAutoPT && <BlockColumn>{t('IsAuto')}</BlockColumn>}
        <BlockColumn>{t('AgentMaxPT')}</BlockColumn>
        <BlockColumn>{t('AgentMinPT')}</BlockColumn>
      </BlockRow>
      <PositionTakingRows
        isReadOnly={isReadOnly}
        productGroupId={groupId}
        ptItems={agentPTs}
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
});

export default AgentPositionTaking;
