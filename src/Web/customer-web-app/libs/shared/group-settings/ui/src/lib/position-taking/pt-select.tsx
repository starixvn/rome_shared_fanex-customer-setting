import { ChangeEvent } from 'react';

import styles from './pt-select.module.scss';

interface Props {
  min: number;
  max: number;
  step: number;
  value: number;
  isDisable: boolean;
  onChange: (event: ChangeEvent<HTMLSelectElement>) => void;
}

export default function PTSelect({ min, max, step, value, isDisable, onChange }: Readonly<Props>) {
  let options = [];
  let pt = max;

  while (min <= pt) {
    options.push(pt);
    pt -= step;
  }

  options = options.includes(value) ? options : [...options, value].sort((a, b) => b - a);

  const invalid = (value > max || value < min) && !isDisable;

  return (
    <select
      value={value}
      disabled={isDisable}
      onChange={onChange}
      className={`${styles['pt-select']} ${invalid ? styles.invalid : ''}`}
    >
      {options.map((option) => (
        <option key={option} value={option}>
          {option / 100}
        </option>
      ))}
    </select>
  );
}
