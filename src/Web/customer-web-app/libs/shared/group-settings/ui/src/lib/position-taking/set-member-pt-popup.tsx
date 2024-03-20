import classNames from 'classnames';
import { useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { UserLevel, UserLevelName } from '@customer-web-app/shared-constants';
import {
    AgentPositionTakingGroupItem, GroupItem, MasterPositionTakingGroupItem
} from '@customer-web-app/shared-group-settings-types';
import { BlockColumn, BlockRow } from '@customer-web-app/shared/base/block';
import { IconClose } from '@customer-web-app/shared/base/common-icon';
import { useTheme } from '@customer-web-app/shared/base/layout';
import NiceModal, { useModal } from '@ebay/nice-modal-react';

import PTSelect from './pt-select';
import styles from './set-member-pt-popup.module.scss';
import { renderNoneBlockColumn, renderPTLabel } from './utils';

interface SetMemberPtPopupProps<T> {
  roleId: number;
  groupItems: T[];
  setNewGroupItems: (groupItem: GroupItem[]) => void;
}

export const SetMemberPtPopup = NiceModal.create(({ roleId, groupItems }: SetMemberPtPopupProps<GroupItem>) => {
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
                  groupItems={newGroupItems as MasterPositionTakingGroupItem[]}
                  setNewGroupItems={(groupItems) => setNewGroupItems(groupItems)}
                />
              ) : (
                <AgentNotTakeAutoPT
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
});

interface CopySuperPTOnMembersValue extends MasterPositionTakingGroupItem {
  isCopied: boolean | undefined;
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
        ? { ...item, superPTOnMembers: itemCopied.superPTOnMembers, isCopied: true }
        : { ...item, isCopied: false };
    }

    return { ...item, isCopied: undefined };
  });
};

const MasterNotTakeAutoPT = ({
  groupItems,
  setNewGroupItems,
}: Partial<SetMemberPtPopupProps<MasterPositionTakingGroupItem>>) => {
  const [itemCopied, setItemCopied] = useState<CopySuperPTOnMembersValue[]>([]);
  const [copyIconId, setCopyIconId] = useState<number | undefined>();
  const timeId = useRef<NodeJS.Timeout>();

  const masterNotTakeAutoPTIConId = 100;
  let isFirstGroup = true;
  let sportTypeId = -1;
  return groupItems?.map((ptItem) => {
    if (sportTypeId !== ptItem.sportTypeId) {
      sportTypeId = ptItem.sportTypeId;
      const group = groupItems.filter((item) => item.sportTypeId === ptItem.sportTypeId);
      const firstGroupItem = group[0];

      const groupLabel =
        isFirstGroup && groupItems.length > 1
          ? renderPTLabel(
              firstGroupItem.sportTypeName,
              false,
              true,
              masterNotTakeAutoPTIConId,
              copyIconId === masterNotTakeAutoPTIConId,
              itemCopied.length > 0 && itemCopied.every((item) => item.isCopied),
              () => true,
              () => {
                const copySuperPTOnMemberValues = copySuperPTOnMembers(groupItems, firstGroupItem, () => true);
                if (timeId) clearTimeout(timeId.current);
                setNewGroupItems?.(copySuperPTOnMemberValues);
                setItemCopied(copySuperPTOnMemberValues);
                setCopyIconId(masterNotTakeAutoPTIConId);
                timeId.current = setTimeout(() => {
                  setItemCopied([]);
                  setCopyIconId(undefined);
                }, 3000);
              }
            )
          : firstGroupItem.sportTypeName;

      isFirstGroup = false;
      return (
        <div key={`${firstGroupItem.sportTypeId}${firstGroupItem.betTypeId}`}>
          <BlockRow key={firstGroupItem.betTypeName} label={groupLabel}>
            {renderNoneBlockColumn(`${ptItem.sportTypeId}${ptItem.betTypeId}`, 1)}
          </BlockRow>
          {group.map((item, index) => {
            const label =
              index === 0 && group.length > 1
                ? renderPTLabel(
                    item.betTypeName,
                    false,
                    true,
                    item.sportTypeId + masterNotTakeAutoPTIConId,
                    copyIconId === item.sportTypeId + masterNotTakeAutoPTIConId,
                    itemCopied.length > 0 &&
                      itemCopied.every((i) => (i.sportTypeId === item.sportTypeId ? i.isCopied : !i.isCopied)),
                    () => true,
                    () => {
                      const copySuperPTOnMemberValues = copySuperPTOnMembers(
                        groupItems,
                        item,
                        (i) => i.sportTypeId === item.sportTypeId
                      );
                      if (timeId) clearTimeout(timeId.current);
                      setNewGroupItems?.(copySuperPTOnMemberValues);
                      setItemCopied(copySuperPTOnMemberValues);
                      setCopyIconId(item.sportTypeId + masterNotTakeAutoPTIConId);
                      timeId.current = setTimeout(() => {
                        setItemCopied([]);
                        setCopyIconId(undefined);
                      }, 3000);
                    }
                  )
                : item.betTypeName;

            const copyValue = itemCopied?.find(
              (i) => item.sportTypeId === i.sportTypeId && item.betTypeId === i.betTypeId
            );
            const copyRowStyle = { isCopy: copyValue?.isCopied, 'isCopy fail': copyValue?.isCopied === false };
            return (
              <BlockRow key={`${item.sportTypeId}${item.betTypeId}`} label={label}>
                <BlockColumn>
                  <div className={classNames('center', copyRowStyle)}>
                    <PTSelect
                      min={0}
                      max={item.upline.s4 - item.m3}
                      step={item.upline.step}
                      value={item.superPTOnMembers}
                      isDisable={false}
                      onChange={(e) => {
                        const newGroupItems = groupItems.map((groupItem) => {
                          if (groupItem.sportTypeId === item.sportTypeId && groupItem.betTypeId === item.betTypeId) {
                            return {
                              ...groupItem,
                              superPTOnMembers: Number(e.target.value),
                              isUpdateSuperPTOnMembers: true,
                            };
                          }

                          return groupItem;
                        });
                        setNewGroupItems?.(newGroupItems as GroupItem[]);
                      }}
                    />
                  </div>
                </BlockColumn>
              </BlockRow>
            );
          })}
        </div>
      );
    }

    return null;
  });
};

interface CopyMasterPTOnMembersValue extends AgentPositionTakingGroupItem {
  isCopied: boolean | undefined;
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
        ? { ...item, masterPTOnMembers: itemCopied.masterPTOnMembers, isCopied: true }
        : { ...item, isCopied: false };
    }

    return { ...item, isCopied: undefined };
  });
};

const AgentNotTakeAutoPT = ({
  groupItems,
  setNewGroupItems,
}: Partial<SetMemberPtPopupProps<AgentPositionTakingGroupItem>>) => {
  const [itemCopied, setItemCopied] = useState<CopyMasterPTOnMembersValue[]>([]);
  const [copyIconId, setCopyIconId] = useState<number | undefined>();
  const timeId = useRef<NodeJS.Timeout>();

  const agentNotTakeAutoPTIConId = 101;
  let isFirstGroup = true;
  let sportTypeId = -1;
  return groupItems?.map((ptItem) => {
    if (sportTypeId !== ptItem.sportTypeId) {
      sportTypeId = ptItem.sportTypeId;
      const group = groupItems.filter((item) => item.sportTypeId === ptItem.sportTypeId);
      const firstGroupItem = group[0];

      const groupLabel =
        isFirstGroup && groupItems.length > 1
          ? renderPTLabel(
              firstGroupItem.sportTypeName,
              false,
              true,
              agentNotTakeAutoPTIConId,
              copyIconId === agentNotTakeAutoPTIConId,
              itemCopied.length > 0 && itemCopied.every((item) => item.isCopied),
              () => true,
              () => {
                const copyMasterPTOnMemberValues = copyMasterPTOnMembers(groupItems, firstGroupItem, () => true);
                setNewGroupItems?.(copyMasterPTOnMemberValues);
                if (timeId) clearTimeout(timeId.current);
                setItemCopied(copyMasterPTOnMemberValues);
                setCopyIconId(agentNotTakeAutoPTIConId);
                timeId.current = setTimeout(() => {
                  setItemCopied([]);
                  setCopyIconId(undefined);
                }, 3000);
              }
            )
          : firstGroupItem.sportTypeName;

      isFirstGroup = false;
      return (
        <div key={`${firstGroupItem.sportTypeId}${firstGroupItem.betTypeId}`}>
          <BlockRow key={firstGroupItem.betTypeName} label={groupLabel}>
            {renderNoneBlockColumn(`${ptItem.sportTypeId}${ptItem.betTypeId}`, 1)}
          </BlockRow>
          {group.map((item, index) => {
            const label =
              index === 0 && group.length > 1
                ? renderPTLabel(
                    item.betTypeName,
                    false,
                    true,
                    item.sportTypeId + agentNotTakeAutoPTIConId,
                    copyIconId === item.sportTypeId + agentNotTakeAutoPTIConId,
                    itemCopied.length > 0 &&
                      itemCopied.every((i) => (i.sportTypeId === item.sportTypeId ? i.isCopied : !i.isCopied)),
                    () => true,
                    () => {
                      const copyMasterPTOnMemberValues = copyMasterPTOnMembers(
                        groupItems,
                        item,
                        (i) => i.sportTypeId === item.sportTypeId
                      );
                      setNewGroupItems?.(copyMasterPTOnMemberValues);
                      if (timeId) clearTimeout(timeId.current);
                      setItemCopied(copyMasterPTOnMemberValues);
                      setCopyIconId(item.sportTypeId + agentNotTakeAutoPTIConId);
                      timeId.current = setTimeout(() => {
                        setItemCopied([]);
                        setCopyIconId(undefined);
                      }, 3000);
                    }
                  )
                : item.betTypeName;

            const copyValue = itemCopied?.find(
              (i) => item.sportTypeId === i.sportTypeId && item.betTypeId === i.betTypeId
            );
            const copyRowStyle = { isCopy: copyValue?.isCopied, 'isCopy fail': copyValue?.isCopied === false };
            return (
              <BlockRow key={`${item.sportTypeId}${item.betTypeId}`} label={label}>
                <BlockColumn>
                  <div className={classNames('center', copyRowStyle)}>
                    <PTSelect
                      min={0}
                      max={item.upline.m3 - item.a2}
                      step={item.upline.step}
                      value={item.masterPTOnMembers}
                      isDisable={false}
                      onChange={(e) => {
                        const newGroupItems = groupItems.map((groupItem) => {
                          if (groupItem.sportTypeId === item.sportTypeId && groupItem.betTypeId === item.betTypeId) {
                            return {
                              ...groupItem,
                              masterPTOnMembers: Number(e.target.value),
                              isUpdateMasterPTOnMembers: true,
                            };
                          }

                          return groupItem;
                        });
                        setNewGroupItems?.(newGroupItems as GroupItem[]);
                      }}
                    />
                  </div>
                </BlockColumn>
              </BlockRow>
            );
          })}
        </div>
      );
    }

    return null;
  });
};
