import classNames from 'classnames';
import { ChangeEvent } from 'react';
import { NumericFormat } from 'react-number-format';

import { NumberHelper } from '@customer-web-app/shared-helpers';

import { Operator } from '../operator';

const BetSettingDecimalScale = 2;

interface BetSettingCellProps {
  id: string;
  value?: number;
  isDisable: boolean;
  operator: Operator;
  limit?: number;
  maxLength?: number;
  isError?: boolean;
  isCopied?: boolean;
  onFocus: () => void;
  onBlur: () => void;
  onChange: (event: ChangeEvent<HTMLInputElement>) => void;
}

export default function BetSettingCell(props: Readonly<BetSettingCellProps>) {
  const { id, value, isDisable, operator, limit, onChange, isError, isCopied, onFocus, onBlur, maxLength } = props;

  return (
    <div className={classNames({ error: isError, copy: isCopied && !isError })}>
      <NumericFormat
        id={id}
        className="number"
        value={value}
        disabled={isDisable}
        allowNegative={false}
        decimalScale={BetSettingDecimalScale}
        thousandSeparator=","
        maxLength={maxLength}
        onChange={onChange}
        onFocus={onFocus}
        onBlur={onBlur}
      />
      {limit ? `${operator} ${NumberHelper.formatWithCommas(limit)}` : ''}
    </div>
  );
}
