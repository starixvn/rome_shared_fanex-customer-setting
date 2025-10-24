import classNames from 'classnames';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';

import { isSportsBookProduct, UserLevel, UserLevelName } from '@customer-web-app/shared-constants';
import {
  AgentPositionTakingGroupItem,
  GroupItem,
  MasterPositionTakingGroupItem,
} from '@customer-web-app/shared-group-settings-types';
import { BlockColumn, BlockRow } from '@customer-web-app/shared/base/block';
import { IconClose } from '@customer-web-app/shared/base/common-icon';
import { useTheme } from '@customer-web-app/shared/base/layout';
import NiceModal, { useModal } from '@ebay/nice-modal-react';

import PTSelect from './pt-select';
import styles from './set-member-pt-popup.module.scss';
import { CopyStatus } from './types';
import {
  CopyPTICon,
  groupSportsBookPTBySportTypeId,
  isCopySuccess,
  renderNoneBlockColumn,
  renderPTLabel,
  usePTCopy,
} from './utils';

interface SetMemberPtPopupProps<T> {
  roleId: number;
  productGroupId: number;
  groupItems: T[];
  setNewGroupItems: (groupItem: GroupItem[]) => void;
}

export const SetMemberPtPopup = NiceModal.create(
  ({ roleId, productGroupId, groupItems }: SetMemberPtPopupProps<GroupItem>) => {
    const { t } = useTranslation();
    const modal = useModal();
    const theme = useTheme();
    const [newGroupItems, setNewGroupItems] = useState(groupItems);

    return (
      <div className={styles.overlay} data-theme={theme}>
        <div className={styles.wrapper}>
          <div className={styles.content}>
            <div className={styles.title}>
              {t(`${UserLevelName[roleId]}SetMemberPtTitle`)}
              <span
                onClick={() => {
                  modal.resolve({
                    isCancel: true,
                    groupItems: [],
                  });
                  modal.remove();
                }}
                className={styles.close}
                aria-hidden
              >
                {IconClose}
              </span>
            </div>
            <div style={{ marginBottom: '10px' }}>{t('UpdateAllMemberPt')}</div>
            <div>
              <div className={styles.tableHead}>{t('PositionTaking')}</div>
              <div className={styles.table}>
                {roleId === UserLevel.Master ? (
                  <MasterNotTakeAutoPT
                    productGroupId={productGroupId}
                    groupItems={newGroupItems as MasterPositionTakingGroupItem[]}
                    setNewGroupItems={(groupItems) => setNewGroupItems(groupItems)}
                  />
                ) : (
                  <AgentNotTakeAutoPT
                    productGroupId={productGroupId}
                    groupItems={newGroupItems as AgentPositionTakingGroupItem[]}
                    setNewGroupItems={(groupItems) => setNewGroupItems(groupItems)}
                  />
                )}
              </div>
            </div>
            <div className={styles.button}>
              <button
                className={styles['btn-confirm']}
                style={{ display: 'block' }}
                onClick={() => {
                  modal.resolve({
                    isCancel: false,
                    groupItems: newGroupItems,
                  });
                  modal.remove();
                }}
              >
                {t('Yes')}
              </button>
              <button
                className={styles['btn-cancel']}
                style={{ display: 'block' }}
                onClick={() => {
                  modal.resolve({
                    isCancel: false,
                    groupItems: [],
                  });
                  modal.remove();
                }}
              >
                {t('No')}
              </button>
            </div>
          </div>
        </div>
      </div>
    );
  }
);

interface CopySuperPTOnMembersValue extends MasterPositionTakingGroupItem {
  copyStatus: CopyStatus;
}

const validateSuperPTOnMembers = (
  current: MasterPositionTakingGroupItem,
  itemCopied: MasterPositionTakingGroupItem
) => {
  const minSuperPTOnMembers = 0;
  const maxSuperPTOnMembers = current.upline.s4 - current.m3;
  return itemCopied.superPTOnMembers >= minSuperPTOnMembers && itemCopied.superPTOnMembers <= maxSuperPTOnMembers;
};

const copySuperPTOnMembers = (
  groupItems: MasterPositionTakingGroupItem[],
  itemCopied: MasterPositionTakingGroupItem,
  isBelongCopiedGroup?: (item: MasterPositionTakingGroupItem) => boolean
): CopySuperPTOnMembersValue[] => {
  return groupItems.map((item) => {
    if (isBelongCopiedGroup?.(item)) {
      return validateSuperPTOnMembers(item, itemCopied)
        ? { ...item, superPTOnMembers: itemCopied.superPTOnMembers, copyStatus: CopyStatus.Success }
        : { ...item, copyStatus: CopyStatus.Fail };
    }

    return { ...item, copyStatus: CopyStatus.NotCopy };
  });
};

const SetMemberPopupIconId = 100;

const MasterNotTakeAutoPT = ({
  productGroupId,
  groupItems,
  setNewGroupItems,
}: Partial<SetMemberPtPopupProps<MasterPositionTakingGroupItem>>) => {
  const { stateCopiedGroup, copyIconId, handleCopy } = usePTCopy();

  if (!groupItems) return null;

  const ptGroups = isSportsBookProduct(productGroupId)
    ? Object.values(groupSportsBookPTBySportTypeId(groupItems as []))
    : [groupItems];

  const handleCopySuperPTOnMembers = (newSuperPTOnMembers: CopySuperPTOnMembersValue[], iconCopyId: number) => {
    setNewGroupItems?.(newSuperPTOnMembers);
    handleCopy(newSuperPTOnMembers, iconCopyId);
  };

  const handleOnChange = (item: MasterPositionTakingGroupItem, value: number) => {
    const newGroupItems = groupItems.map((groupItem) => {
      if (groupItem.sportTypeId === item.sportTypeId && groupItem.betTypeId === item.betTypeId) {
        return {
          ...groupItem,
          superPTOnMembers: value,
          isUpdateSuperPTOnMembers: true,
        };
      }

      return groupItem;
    });
    setNewGroupItems?.(newGroupItems as GroupItem[]);
  };

  const renderGroupLabel = (isFirstGroup: boolean, group: MasterPositionTakingGroupItem[]) => {
    if (!isSportsBookProduct(productGroupId)) return null;
    const { sportTypeId, betTypeId, sportTypeName, betTypeName } = group[0];
    const copyIcon = isFirstGroup && groupItems?.length > 1 && (
      <CopyPTICon
        id={SetMemberPopupIconId}
        isCopy={copyIconId === SetMemberPopupIconId}
        isCopySuccess={isCopySuccess(stateCopiedGroup)}
        handleCopyClick={() => {
          const newSuperPTOnMembers = copySuperPTOnMembers(groupItems, group[0], () => true);
          handleCopySuperPTOnMembers(newSuperPTOnMembers, SetMemberPopupIconId);
        }}
      />
    );

    return (
      <BlockRow key={betTypeName} label={renderPTLabel({
        labelName: sportTypeName,
        isMultipleMode: false,
        isMultipleModeChecked: true,
        copyIcon,
        handleCheckMultiple: () => true,
        tooltipMessage: undefined,
        sportTypeId,
        betTypeId
      })}>
        {renderNoneBlockColumn(`${sportTypeId}${betTypeId}`, 1)}
      </BlockRow>
    );
  };

  const renderLabel = (item: MasterPositionTakingGroupItem, hasCopyIcon: boolean) => {
    const { sportTypeId, sportTypeName, betTypeName } = item;
    const labelName = isSportsBookProduct(productGroupId) ? betTypeName : sportTypeName;
    const copyIcon = hasCopyIcon && (
      <CopyPTICon
        id={sportTypeId + SetMemberPopupIconId}
        isCopy={copyIconId === sportTypeId + SetMemberPopupIconId}
        isCopySuccess={isCopySuccess(stateCopiedGroup, isSportsBookProduct(productGroupId) ? sportTypeId : undefined)}
        handleCopyClick={() => {
          const copySuperPTOnMemberValues = copySuperPTOnMembers(
            groupItems,
            item,
            isSportsBookProduct(productGroupId) ? (i) => i.sportTypeId === sportTypeId : () => true
          );
          handleCopySuperPTOnMembers(copySuperPTOnMemberValues, sportTypeId + SetMemberPopupIconId);
        }}
      />
    );

    return hasCopyIcon ? renderPTLabel({
      labelName,
      isMultipleMode: false,
      isMultipleModeChecked: true,
      copyIcon,
      handleCheckMultiple: () => true,
      tooltipMessage: undefined,
      sportTypeId: item.sportTypeId,
      betTypeId: item.betTypeId
    }) : labelName;
  };

  return (ptGroups as MasterPositionTakingGroupItem[][]).map((group, index) => {
    return (
      <div key={`${group[0].sportTypeId}`}>
        {renderGroupLabel(index === 0, group)}
        {group.map((item, index) => {
          const copyValue = stateCopiedGroup?.find(
            (i) => item.sportTypeId === i.sportTypeId && item.betTypeId === i.betTypeId
          );
          const copyRowStyle = {
            isCopy: copyValue?.copyStatus === CopyStatus.Success,
            'isCopy fail': copyValue?.copyStatus === CopyStatus.Fail,
          };
          return (
            <BlockRow
              key={`${item.sportTypeId}${item.betTypeId}`}
              label={renderLabel(item, index === 0 && group.length > 1)}
            >
              <BlockColumn>
                <div className={classNames('center', copyRowStyle)}>
                  <PTSelect
                    min={0}
                    max={item.upline.s4 - item.m3}
                    step={item.upline.step}
                    value={item.superPTOnMembers}
                    isDisable={false}
                    onChange={(e) => handleOnChange(item, Number(e.target.value))}
                  />
                </div>
              </BlockColumn>
            </BlockRow>
          );
        })}
      </div>
    );
  });
};

interface CopyMasterPTOnMembersValue extends AgentPositionTakingGroupItem {
  copyStatus: CopyStatus;
}

const validateMasterPTOnMembers = (current: AgentPositionTakingGroupItem, itemCopied: AgentPositionTakingGroupItem) => {
  const minMasterPTOnMembers = 0;
  const maxMasterPTOnMembers = current.upline.m3 - current.a2;
  return itemCopied.masterPTOnMembers >= minMasterPTOnMembers && itemCopied.masterPTOnMembers <= maxMasterPTOnMembers;
};

const copyMasterPTOnMembers = (
  groupItems: AgentPositionTakingGroupItem[],
  itemCopied: AgentPositionTakingGroupItem,
  isBelongCopiedGroup?: (item: AgentPositionTakingGroupItem) => boolean
): CopyMasterPTOnMembersValue[] => {
  return groupItems.map((item) => {
    if (isBelongCopiedGroup?.(item)) {
      return validateMasterPTOnMembers(item, itemCopied)
        ? { ...item, masterPTOnMembers: itemCopied.masterPTOnMembers, copyStatus: CopyStatus.Success }
        : { ...item, copyStatus: CopyStatus.Fail };
    }

    return { ...item, copyStatus: CopyStatus.NotCopy };
  });
};

const AgentNotTakeAutoPT = ({
  productGroupId,
  groupItems,
  setNewGroupItems,
}: Partial<SetMemberPtPopupProps<AgentPositionTakingGroupItem>>) => {
  const { stateCopiedGroup, copyIconId, handleCopy } = usePTCopy();

  if (!groupItems) return null;

  const ptGroups = isSportsBookProduct(productGroupId)
    ? Object.values(groupSportsBookPTBySportTypeId(groupItems as []))
    : [groupItems];

  const handleCopyValue = (newMasterPTOnMembers: CopyMasterPTOnMembersValue[], iconCopyId: number) => {
    setNewGroupItems?.(newMasterPTOnMembers);
    handleCopy(newMasterPTOnMembers, iconCopyId);
  };

  const handleOnChange = (item: AgentPositionTakingGroupItem, value: number) => {
    const newGroupItems = groupItems.map((groupItem) => {
      if (groupItem.sportTypeId === item.sportTypeId && groupItem.betTypeId === item.betTypeId) {
        return {
          ...groupItem,
          masterPTOnMembers: value,
          isUpdateMasterPTOnMembers: true,
        };
      }

      return groupItem;
    });
    setNewGroupItems?.(newGroupItems as GroupItem[]);
  };

  const renderGroupLabel = (isFirstGroup: boolean, group: AgentPositionTakingGroupItem[]) => {
    if (!isSportsBookProduct(productGroupId)) return null;
    const { sportTypeId, betTypeId, sportTypeName, betTypeName } = group[0];
    const copyIcon = isFirstGroup && groupItems?.length > 1 && (
      <CopyPTICon
        id={SetMemberPopupIconId}
        isCopy={copyIconId === SetMemberPopupIconId}
        isCopySuccess={isCopySuccess(stateCopiedGroup)}
        handleCopyClick={() => {
          const copyMasterPTOnMemberValues = copyMasterPTOnMembers(groupItems, group[0], () => true);
          handleCopyValue(copyMasterPTOnMemberValues, SetMemberPopupIconId);
        }}
      />
    );

    return (
      <BlockRow key={betTypeName} label={renderPTLabel({
        labelName: sportTypeName,
        isMultipleMode: false,
        isMultipleModeChecked: true,
        copyIcon,
        handleCheckMultiple: () => true,
        tooltipMessage: undefined,
        sportTypeId,
        betTypeId
      })}>
        {renderNoneBlockColumn(`${sportTypeId}${betTypeId}`, 1)}
      </BlockRow>
    );
  };

  const renderLabel = (item: AgentPositionTakingGroupItem, hasCopyIcon: boolean) => {
    const { sportTypeId, sportTypeName, betTypeName } = item;
    const labelName = isSportsBookProduct(productGroupId) ? betTypeName : sportTypeName;
    const copyIcon = hasCopyIcon && (
      <CopyPTICon
        id={sportTypeId + SetMemberPopupIconId}
        isCopy={copyIconId === sportTypeId + SetMemberPopupIconId}
        isCopySuccess={isCopySuccess(stateCopiedGroup, isSportsBookProduct(productGroupId) ? sportTypeId : undefined)}
        handleCopyClick={() => {
          const copyMasterPTOnMemberValues = copyMasterPTOnMembers(
            groupItems,
            item,
            isSportsBookProduct(productGroupId) ? (i) => i.sportTypeId === sportTypeId : () => true
          );
          handleCopyValue(copyMasterPTOnMemberValues, sportTypeId + SetMemberPopupIconId);
        }}
      />
    );
    return hasCopyIcon ? renderPTLabel({
      labelName,
      isMultipleMode: false,
      isMultipleModeChecked: true,
      copyIcon,
      handleCheckMultiple: () => true,
      tooltipMessage: undefined,
      sportTypeId: item.sportTypeId,
      betTypeId: item.betTypeId
    }) : labelName;
  };

  return (ptGroups as AgentPositionTakingGroupItem[][]).map((group, index) => {
    return (
      <div key={`${group[0].sportTypeId}`}>
        {renderGroupLabel(index === 0, group)}
        {group.map((item, index) => {
          const copyValue = stateCopiedGroup?.find(
            (i) => item.sportTypeId === i.sportTypeId && item.betTypeId === i.betTypeId
          );
          const copyRowStyle = {
            isCopy: copyValue?.copyStatus === CopyStatus.Success,
            'isCopy fail': copyValue?.copyStatus === CopyStatus.Fail,
          };
          return (
            <BlockRow
              key={`${item.sportTypeId}${item.betTypeId}`}
              label={renderLabel(item, index === 0 && group.length > 1)}
            >
              <BlockColumn>
                <div className={classNames('center', copyRowStyle)}>
                  <PTSelect
                    min={0}
                    max={item.upline.m3 - item.a2}
                    step={item.upline.step}
                    value={item.masterPTOnMembers}
                    isDisable={false}
                    onChange={(e) => handleOnChange(item, Number(e.target.value))}
                  />
                </div>
              </BlockColumn>
            </BlockRow>
          );
        })}
      </div>
    );
  });
};
