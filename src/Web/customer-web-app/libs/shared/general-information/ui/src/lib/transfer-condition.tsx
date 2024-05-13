import classNames from 'classnames';
import { ChangeEvent, forwardRef, useCallback, useEffect, useImperativeHandle, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { ValidationRefType } from '@customer-web-app/share-models';
import { UserLevel } from '@customer-web-app/shared-constants';
import { GeneralInfo } from '@customer-web-app/shared-general-information-state';
import { scrollToBLock } from '@customer-web-app/shared-group-settings-ui';
import { Block, BlockRefType } from '@customer-web-app/shared/base/block';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import styles from './transfer-condition.module.scss';

export const TransferOptions = {
  Daily: '00000000',
};

interface WeeklyOptions {
  [index: string]: boolean;
  mon: boolean;
  tue: boolean;
  wed: boolean;
  thurs: boolean;
  fri: boolean;
  sat: boolean;
  sun: boolean;
}

const defaultWeek = {
  sun: false,
  mon: false,
  tue: false,
  wed: false,
  thurs: false,
  fri: false,
  sat: false,
};

const TooltipSelectAtLeastDay = {
  contentKey: 'chooseOneDay',
  id: 'anchor-tooltip-uuid-01',
  autoHide: false,
};

const TooltipCheckAllOptions = {
  contentKey: 'chooseDaily',
  id: 'anchor-tooltip-uuid-02',
  autoHide: true,
};

export interface TransferConditionProps {
  generalInfo: GeneralInfo;
  onGeneralInfoChange: (data: Partial<GeneralInfo>) => void;
  children?: React.ReactNode;
}

export const TransferCondition = forwardRef<ValidationRefType, TransferConditionProps>(function GeneralInformation(
  { generalInfo, onGeneralInfoChange, children },
  ref
) {
  const { t } = useTranslation();

  const disabled = generalInfo.isAutomateTransfer;
  const [weekly, setWeekly] = useState<boolean>(false);
  const [days, setDays] = useState<WeeklyOptions>(defaultWeek);
  const [openTooltip, setOpenTooltip] = useState(false);
  const [contentTooltip, setContentTooltip] = useState<string>('');
  const [targetTooltip, setTargetTooltip] = useState<string>('');
  const timeoutRef = useRef<NodeJS.Timeout | null>(null);
  const blockRef = useRef<BlockRefType>(null);

  const TooltipDisableTransferCondition = {
    contentKey:
      generalInfo.uplineRoleId === UserLevel.Super
        ? t('TransferConditionByCompany').toString()
        : t('TransferConditionByUpline').toString(),
    id: 'anchor-tooltip-uuid-02',
    autoHide: true,
  };

  useImperativeHandle(ref, () => {
    const validate = () => {
      const isWeeklyOptionWithoutSelectedDays = '00000001';
      if (generalInfo.transferOption === isWeeklyOptionWithoutSelectedDays) {
        showTooltip(TooltipSelectAtLeastDay);
        blockRef.current?.expand();
        return false;
      }
      return true;
    };

    const expandBlock = () => {
      blockRef.current?.expand();
    };

    return {
      validate,
      scrollToBlock: () => scrollToBLock('transfer-condition'),
      expandBlock,
    };
  });

  const showTooltip = useCallback((tooltip: { contentKey: string; id: string; autoHide: boolean }) => {
    setTargetTooltip(tooltip.id);
    setContentTooltip(tooltip.contentKey);
    setOpenTooltip(true);

    // Clear the previous timeout if it exists
    if (timeoutRef.current) {
      clearTimeout(timeoutRef.current);
    }

    // Start a new timeout if isAutoHide is true
    if (tooltip.autoHide) {
      timeoutRef.current = setTimeout(() => {
        setOpenTooltip(false);
      }, 3000);
    }
  }, []);

  useEffect(() => {
    if (disabled) {
      setTargetTooltip(TooltipDisableTransferCondition.id);
      setContentTooltip(TooltipDisableTransferCondition.contentKey);
    }
  }, [TooltipDisableTransferCondition.contentKey, TooltipDisableTransferCondition.id, disabled]);

  const updateGlobalState = (values: WeeklyOptions, isWeekly: boolean) => {
    const daysArray = Object.values(values);
    const weeklyOptions = daysArray.map((day) => (day ? '1' : '0')).join('');
    onGeneralInfoChange({ transferOption: `${weeklyOptions}${isWeekly ? '1' : '0'}` });
  };

  const onClickCheckboxWeekly = () => {
    setWeekly(true);
    updateGlobalState(days, true);
  };

  const onClickCheckboxDaily = () => {
    setWeekly(false);
    onGeneralInfoChange({ transferOption: TransferOptions.Daily });
    if (openTooltip) {
      setOpenTooltip(false);
    }
  };

  const handleDayChange = (event: ChangeEvent<HTMLInputElement>) => {
    const { name, checked } = event.target;
    const options = {
      ...days,
      [name]: checked,
    };

    const checkAllOptions = !Object.values(options).includes(false);
    if (checkAllOptions) {
      showTooltip(TooltipCheckAllOptions);
      return;
    }
    const unCheckAllOptions = !Object.values(options).includes(true);
    if (unCheckAllOptions) {
      showTooltip(TooltipSelectAtLeastDay);
    } else {
      setOpenTooltip(false);
    }

    setDays(options);
    updateGlobalState(options, true);
  };

  return (
    <Block id="transfer-condition" ref={blockRef} title={t('accBalanceTransferCondition')} isPartialUpdate={false}>
      <div
        className={classNames(styles.container, { [styles.disabled]: disabled })}
        onMouseEnter={() => disabled && setOpenTooltip(true)}
        onMouseLeave={() => disabled && setOpenTooltip(false)}
        aria-hidden
      >
        <div
          aria-hidden
          className={classNames(styles.box, 'daily', { [styles.active]: !weekly })}
          onClick={() => !disabled && onClickCheckboxDaily()}
          onBlur={() => setOpenTooltip(false)}
        >
          <label className={styles['btn-box']} htmlFor="daily" id={TooltipCheckAllOptions.id}>
            <span className={`icon ${styles.icon} ic-time`} />
            <span className={styles.text}>{t('daily')}</span>
          </label>
        </div>
        <div
          aria-hidden
          className={classNames(styles.box, 'weekly', { [styles.active]: weekly })}
          onClick={() => !disabled && onClickCheckboxWeekly()}
          onBlur={() => setOpenTooltip(false)}
        >
          <label className={styles['btn-box']} htmlFor="weekly">
            <span className={`icon ${styles.icon} ic-viewlog`} />
            <span className={styles.text}>{t('weekly')}</span>
          </label>
          <ul className={styles['list-day']}>
            {['mon', 'tue', 'wed', 'thurs', 'fri', 'sat', 'sun'].map((day) => (
              <li key={day}>
                <label htmlFor={day}>
                  <input
                    id={day}
                    type="checkbox"
                    name={day}
                    disabled={!weekly}
                    checked={days[day]}
                    onChange={handleDayChange}
                  />
                  <span id={day === 'mon' ? TooltipSelectAtLeastDay.id : ''}>{t(day)}</span>
                </label>
              </li>
            ))}
          </ul>
        </div>

        <WrapperTooltip
          anchorSelect={`#${targetTooltip}`}
          isOpen={openTooltip}
          content={t(contentTooltip).toString()}
          type="error"
        />
      </div>
    </Block>
  );
});
