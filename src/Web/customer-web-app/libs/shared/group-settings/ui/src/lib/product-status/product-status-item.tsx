import { ChangeEvent } from 'react';
import { useTranslation } from 'react-i18next';

import { SystemIds, UserLevel, UserLevelName } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { ProductStatusGroupItem } from '@customer-web-app/shared-group-settings-types';
import { BlockRow } from '@customer-web-app/shared/base/block';

import styles from './product-status.module.scss';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

type Props = {
  onChange: (sportTypeId: number, betTypeId: number, isDisabled: boolean) => void;
  data: ProductStatusGroupItem;
  isReadonly: boolean;
};

export function ProductStatusItem(props: Readonly<Props>) {
  const { t } = useTranslation();
  const { generalInfo } = useGroupSettingContext();
  const userLevel = t(UserLevelName[Number(generalInfo?.roleId)]);

  if (!props.data) {
    return (
      <div className={styles.container}>
        <BlockRow>
          <div className={styles.status}>
            <div className={styles.blockCenter}></div>
          </div>
        </BlockRow>
      </div>
    );
  }

  const { sportTypeName, sportTypeId, betTypeId, isDisabled, readonly, isDisabledUpline, tooltipMessage } = props.data;

  const handleOnChange = (e: ChangeEvent<HTMLInputElement>) => {
    props.onChange(sportTypeId, betTypeId, !e.target.checked);
  };

  let isReadOnly = props.isReadonly;

  if (!props.isReadonly) {
    isReadOnly = isDisabledUpline || readonly;

    if (
      (appConfig.systemId === SystemIds.Alpha || appConfig.systemId === SystemIds.InternalAlpha) &&
      generalInfo?.roleId !== UserLevel.Member
    ) {
      isReadOnly = isReadOnly || !isDisabled;
    }
  }

  return (
    <div className={styles.container}>
      <BlockRow
        label={
          tooltipMessage ? (
            <div style={{ justifyContent: 'space-between', width: '100%' }}>
              <span className={styles.sportTypeName}>{sportTypeName}</span>
              <span
                className="ic-info"
                id={`jackpot-tooltip-${sportTypeId}-${betTypeId}`}
                data-tooltip-id={`jackpot-tooltip-${sportTypeId}-${betTypeId}`}
                data-tooltip-place="top"
                data-tooltip-content={tooltipMessage}
              ></span>
              <WrapperTooltip type="default" id={`jackpot-tooltip-${sportTypeId}-${betTypeId}`} place="top" />
            </div>
          ) : (
            sportTypeName
          )
        }
      >
        <div className={styles.status}>
          <div className={styles.blockCenter}>
            <input
              id={`status-${sportTypeId}-${betTypeId}`}
              type="checkbox"
              checked={!isDisabled}
              onChange={handleOnChange}
              disabled={isReadOnly}
            />
            <label htmlFor={`status-${sportTypeId}-${betTypeId}`}>{userLevel}</label>
          </div>
        </div>
      </BlockRow>
    </div>
  );
}
