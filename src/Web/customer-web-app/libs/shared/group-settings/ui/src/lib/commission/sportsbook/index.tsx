import React, { useEffect } from 'react';
import { useTranslation } from 'react-i18next';

import { BetTypeIds, SportTypeIds, UserLevel } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { BlockColumn, BlockRow } from '@customer-web-app/shared/base/block';

import CommissionLabel from '../commission-label';
import CommissionSelect from '../commission-select';
import SelectAllCommissionCheckbox from '../select-all-commission-checkbox';
import { Props } from '../types';
import CommissionGroup from './commission-group';
import styles from './sportsbook-commission.module.scss';

export function MemberSportsBookCommission({
  commissions,
  handleCommissionChange,
  isReadOnly,
  handleSelectAll,
}: Readonly<Props>) {
  const { t } = useTranslation();
  const { generalInfo, isMultipleMode, group, handleMemberCommGroupChange, handleSelectCommGroup } =
    useGroupSettingContext();
  const asianHDPCommValue = generalInfo?.commissions?.find((c) => group?.includes(c.name))?.discount ?? 0;

  useEffect(() => {
    if (generalInfo?.commissions) {
      handleCommissionChange(SportTypeIds.Soccer, BetTypeIds.Handicap, { discount: asianHDPCommValue });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [asianHDPCommValue]);

  return (
    <>
      <BlockRow
        label={
          <>
            {isMultipleMode && !isReadOnly && (
              <input
                type="checkbox"
                onChange={(e) => {
                  const selected = e.target.checked;
                  handleCommissionChange(SportTypeIds.Soccer, BetTypeIds.Handicap, {
                    isMultipleModeChecked: selected,
                  });
                  handleSelectCommGroup?.(selected);
                }}
              />
            )}
            {t('Group')}
          </>
        }
      >
        <BlockColumn>
          {generalInfo && group && (
            <CommissionGroup
              site={generalInfo.site}
              currency={generalInfo.currency.toString()}
              value={group}
              onChange={(e) => {
                handleMemberCommGroupChange?.(e.target.value);
              }}
              isReadOnly={isReadOnly}
            />
          )}
        </BlockColumn>
        <BlockColumn>{''}</BlockColumn>
      </BlockRow>
      <BlockRow
        isHeaderRow
        label={
          isMultipleMode &&
          !isReadOnly && (
            <div style={{ display: 'flex', marginLeft: '2px' }}>
              <SelectAllCommissionCheckbox
                checked={commissions.every((item) => item.isMultipleModeChecked === true)}
                handleChange={handleSelectAll}
              />
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
              <CommissionLabel
                displayName={displayName as string}
                isSelected={isMultipleModeChecked}
                isReadOnly={isReadOnly}
                handleSelectCommission={(isChecked) =>
                  handleCommissionChange(sportTypeId, betTypeId, { isMultipleModeChecked: isChecked })
                }
              />
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
