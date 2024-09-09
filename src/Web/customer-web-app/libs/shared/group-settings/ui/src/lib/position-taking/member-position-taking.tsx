import classNames from 'classnames';
import { forwardRef, useEffect, useImperativeHandle, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { ValidationRefType } from '@customer-web-app/share-models';
import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import {
  GroupItem,
  MemberPositionTakingGroupItem,
  MemberPositionTakingProps,
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

function MemberPositionTakingRow({
  item,
  label,
  copyStatus,
  isReadOnly,
}: Readonly<PTRowProps<MemberPositionTakingGroupItem>>) {
  const { handleSettingItemChange, productGroupId } = useGroupSettingContext();

  const handlePTChange = (data: { [x: string]: number | undefined }) => {
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
        <div id={`a1-${item.sportTypeId}-${item.betTypeId}`} className={classNames('center', copyRowStyle)}>
          <PTSelect
            min={item.upline.n2}
            max={item.upline.a2}
            step={item.step}
            value={item.a1}
            isDisable={isReadOnly || !item.isMultipleModeChecked}
            onChange={(e) => handlePTChange({ a1: Number(e.target.value) })}
          />
        </div>
      </BlockColumn>
    </BlockRow>
  );
}

const validateMemberPT = (currentPT: MemberPositionTakingGroupItem, newPT: MemberPositionTakingProps) => {
  const uplinePT = currentPT.upline;
  const { a1 } = newPT;

  return a1 >= uplinePT.n2 && a1 <= uplinePT.a2;
};

const copyMemberPTs = (
  positionTakings: GroupItem[],
  itemCopied: GroupItem,
  isBelongCopiedGroup?: (item: GroupItem) => boolean
) => {
  const newItemCopied = itemCopied as MemberPositionTakingGroupItem;
  if (!validateMemberPT(newItemCopied, newItemCopied)) return [];
  const { a1, isMultipleModeChecked } = newItemCopied;
  return (positionTakings as MemberPositionTakingGroupItem[]).map((item) => {
    if (isBelongCopiedGroup?.(item)) {
      return validateMemberPT(item, newItemCopied)
        ? { ...item, a1, isMultipleModeChecked, copyStatus: CopyStatus.Success }
        : { ...item, isMultipleModeChecked, copyStatus: CopyStatus.Fail };
    }
    return { ...item, copyStatus: CopyStatus.NotCopy };
  });
};

type PositionTakingRowProps = {
  isReadOnly: boolean;
  productGroupId: number;
  ptItems: MemberPositionTakingGroupItem[];
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

  return (ptGroups as MemberPositionTakingGroupItem[][]).map((group, index) => {
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
              {renderNoneBlockColumn(`${firstGroupItem.sportTypeId}${firstGroupItem.betTypeId}`, 1)}
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
            <MemberPositionTakingRow
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
const validateMemberPositionTaking = (item: MemberPositionTakingGroupItem): ValidatePTResult => {
  const { a1, upline } = item;

  if (a1 > upline.a2) {
    return {
      id: `a1-${item.sportTypeId}-${item.betTypeId}`,
      isValid: a1 <= upline.a2,
      message: 'AgentPTCannotBeGreaterThanAgentMaxPT',
      msgParams: { value: upline.a2 / 100 },
    };
  }

  return {
    id: '',
    isValid: true,
    message: '',
  };
};

const MemberPTBLockId = 'member-position-taking';

const MemberPositionTaking = forwardRef<ValidationRefType, PositionTakingBlockProps>(function MemberPositionTaking(
  props,
  ref
) {
  const { t } = useTranslation();
  const { title } = props;
  const { productGroups, isMultipleMode, handleUpdateSettingItems, productGroupId, isEditMode, updateGroupSetting } =
    useGroupSettingContext();
  const groupId = Number(productGroupId);
  const memberPTs = getSettingItems<MemberPositionTakingGroupItem>(productGroups, groupId, GroupTypeIds.PositionTaking);
  const [validateResult, setValidateResult] = useState<ValidatePTResult | undefined>();
  const { stateCopiedGroup, copyIconId, handleCopy } = usePTCopy();
  const blockRef = useRef<BlockRefType>(null);

  const validateMemberPTs = (memberPTs: MemberPositionTakingGroupItem[]) => {
    let validateResult = true;
    for (const item of memberPTs) {
      if (item.isMultipleModeChecked) {
        const error = validateMemberPositionTaking(item);
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
    const validateResult = validateMemberPTs(memberPTs);
    if (validateResult) setValidateResult(undefined);
  }, [memberPTs]);

  useImperativeHandle(ref, () => {
    const validate = () => {
      const validateResult = validateMemberPTs(memberPTs);

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
      scrollToBlock: () => scrollToBLock(MemberPTBLockId),
    };
  });

  const handleCopyPTs = (
    itemCopied: GroupItem,
    iconId: number | undefined,
    isBelongCopiedGroup?: (item: GroupItem) => boolean
  ) => {
    if (!itemCopied.isMultipleModeChecked) return;
    const newPTs = copyMemberPTs(memberPTs, itemCopied, isBelongCopiedGroup);
    if (newPTs.length > 0) {
      handleCopy(newPTs, iconId);
      handleUpdateSettingItems?.(groupId, GroupTypeIds.PositionTaking, newPTs);
    }
  };

  const handleSelectAll = (isChecked: boolean) => {
    const newMemberPTs = memberPTs.map((item) => ({ ...item, isMultipleModeChecked: isChecked }));
    handleUpdateSettingItems?.(groupId, GroupTypeIds.PositionTaking, newMemberPTs);
  };

  return (
    <div id={MemberPTBLockId} className={styles['short-block']}>
      <Block
        ref={blockRef}
        title={title}
        isPartialUpdate={isEditMode === true && !props.isReadOnly}
        onSaveClick={() => updateGroupSetting?.(GroupTypeIds.PositionTaking)}
      >
        <BlockRow
          isHeaderRow
          label={
            isMultipleMode &&
            memberPTs.length > 1 &&
            !props.isReadOnly && (
              <div style={{ display: 'flex', marginLeft: '2px' }}>
                <input
                  type="checkbox"
                  style={{ top: '0' }}
                  onChange={(e) => handleSelectAll(e.target.checked)}
                  checked={memberPTs.every((item) => item.isMultipleModeChecked === true)}
                />
                <div>{t('SelectAll')}</div>
              </div>
            )
          }
        >
          <BlockColumn>{t('agentPT')}</BlockColumn>
        </BlockRow>
        <PositionTakingRows
          isReadOnly={props.isReadOnly}
          productGroupId={groupId}
          ptItems={memberPTs}
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
    </div>
  );
});

export default MemberPositionTaking;
