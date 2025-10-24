import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { BlockItem } from '@customer-web-app/shared/base/block';

import CommissionLabel from '../commission-label';
import CommissionSelect from '../commission-select';
import styles from '../commission.module.scss';
import SelectAllCommissionCheckbox from '../select-all-commission-checkbox';
import { Props } from '../types';

export default function DefaultCommission({
  commissions,
  isReadOnly,
  handleCommissionChange,
  handleSelectAll,
}: Readonly<Props>) {
  const { isMultipleMode } = useGroupSettingContext();

  return (
    <div className={styles.container}>
      {isMultipleMode && !isReadOnly && commissions.length > 1 && (
        <div className={styles['select-all-commission']}>
          <SelectAllCommissionCheckbox
            checked={commissions.every((item) => item.isMultipleModeChecked === true)}
            handleChange={handleSelectAll}
          />
        </div>
      )}
      {commissions?.map(({ sportTypeId, betTypeId, displayName, discount, upline, isMultipleModeChecked, tooltipMessage }) => {
        return (
          <BlockItem
            key={`${sportTypeId}${betTypeId}`}
            label={
              <CommissionLabel
                displayName={displayName as string}
                isSelected={isMultipleModeChecked}
                isReadOnly={isReadOnly}
                tooltipMessage={tooltipMessage}
                sportTypeId={sportTypeId}
                betTypeId={betTypeId}
                handleSelectCommission={(isChecked) =>
                  handleCommissionChange(sportTypeId, betTypeId, { isMultipleModeChecked: isChecked })
                }
              />
            }
          >
            <CommissionSelect
              max={upline.discount ?? 0}
              value={discount ?? 0}
              isDisable={!isMultipleModeChecked || isReadOnly}
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
