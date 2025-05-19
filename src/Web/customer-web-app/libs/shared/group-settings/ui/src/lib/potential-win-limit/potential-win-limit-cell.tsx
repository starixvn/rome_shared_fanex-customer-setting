import classNames from 'classnames';
import { ChangeEvent, useContext } from 'react';
import { useTranslation } from 'react-i18next';
import { NumericFormat } from 'react-number-format';

import { MaxNumberInputLength } from '@customer-web-app/shared-constants';
import { NumberHelper } from '@customer-web-app/shared-helpers';

import { ValidationContext } from '../bet-setting/validation-context';
import { Operator } from '../operator';
import styles from './potential-win-limit.module.scss';

interface Props {
  currentValue: number;
  currentValueUnlimited: boolean;
  maxValue: number;
  originalIsUplineUnlimited: boolean;
  isDisable: boolean;
  componentId: string;
  onChange: (value: number | undefined) => void;
  onChangeUnlimited: (valueCheckBox: boolean | undefined, value: number | undefined) => void;
  isError?: boolean;
  onFocus: () => void;
  onBlur: () => void;
}

export function PotentialWinLimitCell(props: Readonly<Props>) {
  
  const { t } = useTranslation();
  const unlimitedText = t('unlimited');
  const {
    onChange,
    originalIsUplineUnlimited,
    maxValue,
    isDisable,
    componentId,
    onChangeUnlimited,
    isError,
    onFocus,
    onBlur,
    currentValue,
    currentValueUnlimited,
  } = props;
  const { validationState } = useContext(ValidationContext);

  const handleChange = (event: ChangeEvent<HTMLInputElement>) => {
    event.preventDefault();
    const newValue = NumberHelper.parseNumber(event.target.value);
    onChange(newValue);
  };

  return (
    <>
    <div style={{display: 'inline-block', margin: '0 auto'}} >
      <div className={classNames({ error: isError })}>
          {currentValueUnlimited ? (
            <input type="text" value={unlimitedText} disabled />
          ) : (
            <NumericFormat
              key={componentId}
              id={componentId}
              disabled={isDisable}
              data-tooltip-id={componentId}
              data-tooltip-html={validationState?.message}
              className="number"
              maxLength={MaxNumberInputLength}
              value={currentValue}
              allowNegative={false}
              onChange={handleChange}
              //decimalScale={currentValue === ParlayMaxPayoutMissConfigValue ? 1 : 0}
              decimalScale={0}
              thousandSeparator=","
              onFocus={onFocus}
              onBlur={onBlur}
            />
          )}
        
      {maxValue !== 0 && (
        <div className={styles['max-value']}>{` ${Operator['<=']} ${NumberHelper.formatWithCommas(maxValue)}`}</div>
      )}
      <span>
            (1 - {t('unlimited')})
      </span>
      </div>      
      <div className={classNames(styles.inline, styles.blockLeft)}>
        <input
          id={`unlimited-input-${componentId}`}
          type="checkbox"
          checked={currentValueUnlimited}
          onChange={(e) => {
            const isUnlimited = e.target.checked;
            onChangeUnlimited(isUnlimited, isUnlimited ? 0 : currentValue);
          }}
          disabled={isDisable}
        />
        <label htmlFor={`unlimited-input-${componentId}`} className="unselectable">
          {unlimitedText}
        </label>
      </div>   
    </div>   
    </>
  );
}
