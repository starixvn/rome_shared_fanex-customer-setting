import { useTranslation } from 'react-i18next';

import { BetTypeIds, SportTypeIds, UserLevel } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { BlockColumn, BlockRow } from '@customer-web-app/shared/base/block';

import CommissionLabel from '../commission-label';
import CommissionSelect from '../commission-select';
import SelectAllCommissionCheckbox from '../select-all-commission-checkbox';
import { Props } from '../types';

export default function DirectMemberCommission({
  commissions,
  handleCommissionChange,
  isReadOnly,
  handleSelectAll,
}: Readonly<Props>) {
  const { t } = useTranslation();
  const { generalInfo, isMultipleMode } = useGroupSettingContext();
  const commissionGroup = generalInfo?.group;
  const asianHDPCommValue = generalInfo?.commissions?.find((c) => commissionGroup?.includes(c.name))?.discount ?? 0;
  return (
    <>
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
        <BlockColumn>
          {params.uplineRoleId === UserLevel.Super ? t('lblCommissionForSuper') : t('lblCommissionForMaster')}
        </BlockColumn>
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
            <BlockColumn>
              <div className="show-value">{upline.discount}</div>
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
