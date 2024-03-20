import { useEffect } from 'react';
import { useTranslation } from 'react-i18next';

import { BetTypeIds, GroupTypeIds, ProductGroupIds, SportTypeIds, UserLevel } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import { CommissionGroupItem } from '@customer-web-app/shared-group-settings-types';
import { Block, BlockColumn, BlockItem, BlockRow } from '@customer-web-app/shared/base/block';

import { GroupSettingBlockProps } from '../../position-taking/super-position-taking';
import CommissionSelect from '../commission-select';
import styles from './sportsbook-commission.module.scss';

const renderBlockRowTitle = (
  isChecked: boolean,
  blockRowTitle: string | undefined,
  onChange: (isChecked: boolean) => void
) => {
  return (
    <div>
      <input type="checkbox" onChange={(e) => onChange(e.target.checked)} checked={isChecked} />
      <span>{blockRowTitle}</span>
    </div>
  );
};

export default function SportsBookCommission({ title, isReadOnly }: Readonly<GroupSettingBlockProps>) {
  const { productGroups, handleSettingItemChange, handleUpdateSettingItems } = useGroupSettingContext();
  const commissions = getSettingItems<CommissionGroupItem>(productGroups, 1, GroupTypeIds.SportsBookCommission);

  const handleCommissionChange = (sportTypeId: number, betTypeId: number, data: Partial<CommissionGroupItem>) => {
    handleSettingItemChange?.(
      ProductGroupIds.SportsBook,
      GroupTypeIds.SportsBookCommission,
      sportTypeId,
      betTypeId,
      data
    );
  };

  const handleSelectAll = (isChecked: boolean) => {
    const newCommissions = commissions.map((item) => ({ ...item, isMultipleModeChecked: isChecked }));
    handleUpdateSettingItems?.(ProductGroupIds.SportsBook, GroupTypeIds.SportsBookCommission, newCommissions);
  };

  const props = { commissions, handleCommissionChange, isReadOnly, handleSelectAll };

  return (
    <Block title={title}>
      {params.roleId === UserLevel.Member || params.uplineRoleId === UserLevel.Agent ? (
        <MemberSportsBookCommission {...props} />
      ) : (
        <SMASportsBookCommission {...props} />
      )}
    </Block>
  );
}

interface Props {
  isReadOnly: boolean;
  commissions: CommissionGroupItem[];
  handleCommissionChange: (sportTypeId: number, betTypeId: number, data: Partial<CommissionGroupItem>) => void;
  handleSelectAll: (isChecked: boolean) => void;
}

function SMASportsBookCommission({
  commissions,
  isReadOnly,
  handleCommissionChange,
  handleSelectAll,
}: Readonly<Props>) {
  const { t } = useTranslation();
  const { isMultipleMode } = useGroupSettingContext();

  return (
    <div className={styles.container}>
      {isMultipleMode && !isReadOnly && (
        <div className={styles['select-all-commission']}>
          <input
            type="checkbox"
            style={{ top: '0' }}
            onChange={(e) => handleSelectAll(e.target.checked)}
            checked={commissions.every((item) => item.isMultipleModeChecked === true)}
          />
          <div>{t('SelectAll')}</div>
        </div>
      )}
      {commissions?.map(({ sportTypeId, betTypeId, displayName, discount, upline, isMultipleModeChecked }) => {
        return (
          <BlockItem
            key={`${sportTypeId}${betTypeId}`}
            label={
              isMultipleMode
                ? renderBlockRowTitle(isMultipleModeChecked, displayName, (isChecked) =>
                    handleCommissionChange(sportTypeId, betTypeId, { isMultipleModeChecked: isChecked })
                  )
                : displayName
            }
          >
            <CommissionSelect
              max={upline.discount ?? 0}
              value={discount ?? 0}
              isDisable={!isMultipleModeChecked}
              onChange={(e) => {
                handleCommissionChange?.(sportTypeId, betTypeId, { discount: Number(e.target.value) });
              }}
            />
          </BlockItem>
        );
      })}
    </div>
  );
}

function MemberSportsBookCommission({
  commissions,
  handleCommissionChange,
  isReadOnly,
  handleSelectAll,
}: Readonly<Props>) {
  const { t } = useTranslation();
  const { generalInfo, isMultipleMode } = useGroupSettingContext();
  const commissionGroup = generalInfo?.group;
  const asianHDPCommValue = generalInfo?.commissions?.find((c) => commissionGroup?.includes(c.name))?.discount ?? 0;

  useEffect(() => {
    if (generalInfo?.commissions) {
      handleCommissionChange(SportTypeIds.Soccer, BetTypeIds.Handicap, { discount: asianHDPCommValue });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [asianHDPCommValue]);

  return (
    <>
      <BlockRow
        isHeaderRow
        label={
          isMultipleMode &&
          !isReadOnly && (
            <div style={{ display: 'flex', marginLeft: '2px' }}>
              <input
                type="checkbox"
                style={{ top: '0' }}
                onChange={(e) => handleSelectAll(e.target.checked)}
                checked={commissions.every((item) => item.isMultipleModeChecked === true)}
              />
              <div>{t('SelectAll')}</div>
            </div>
          )
        }
      >
        {params.uplineRoleId === UserLevel.Super && <BlockColumn>{t('lblCommissionForMaster')}</BlockColumn>}
        <BlockColumn>{t('lblCommissionForAgent')}</BlockColumn>
        <BlockColumn>{t('lblCommissionForMember')}</BlockColumn>
      </BlockRow>
      {commissions?.map(({ sportTypeId, betTypeId, displayName, discount, upline, isMultipleModeChecked }) => {
        const isAsianHDP = sportTypeId === SportTypeIds.Soccer && betTypeId === BetTypeIds.Handicap;
        return (
          <BlockRow
            key={`${sportTypeId}-${betTypeId}`}
            label={
              isMultipleMode && !isReadOnly
                ? renderBlockRowTitle(isMultipleModeChecked, displayName, (isChecked) =>
                    handleCommissionChange(sportTypeId, betTypeId, { isMultipleModeChecked: isChecked })
                  )
                : displayName
            }
          >
            {params.uplineRoleId === UserLevel.Super && (
              <BlockColumn>
                <div className="show-value">{upline.upline.discount}</div>
              </BlockColumn>
            )}
            <BlockColumn>
              {isReadOnly ? (
                <div className="show-value">{upline.discount}</div>
              ) : (
                <div className={styles.blockCenter}>
                  <input
                    type="text"
                    value={isAsianHDP && generalInfo?.commissions !== null ? asianHDPCommValue : upline.discount}
                    disabled
                  />
                </div>
              )}
            </BlockColumn>
            <BlockColumn>
              {isReadOnly ? (
                <div className="show-value">{discount}</div>
              ) : (
                <CommissionSelect
                  max={isAsianHDP && generalInfo?.commissions !== null ? asianHDPCommValue : upline.discount ?? 0}
                  value={discount ?? 0}
                  isDisable={!isMultipleModeChecked}
                  onChange={(e) => {
                    handleCommissionChange?.(sportTypeId, betTypeId, { discount: Number(e.target.value) });
                  }}
                />
              )}
            </BlockColumn>
          </BlockRow>
        );
      })}
    </>
  );
}
