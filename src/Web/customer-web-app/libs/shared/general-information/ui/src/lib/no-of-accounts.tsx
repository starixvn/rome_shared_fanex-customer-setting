import { ChangeEvent } from 'react';

import styles from './no-of-accounts.module.scss';

type Props = {
  value: number;
  onChange: (value: number) => void;
};

export default function NoOfAccounts({ value, onChange }: Props) {
  const handleChange = (event: ChangeEvent<HTMLSelectElement>) => {
    const value = Number(event.target.value);
    onChange(value);
  };

  return (
    <select className={styles.dropdown} value={value} onChange={handleChange}>
      {[...Array(30)].map((_, index) => (
        <option key={index} value={index + 1}>
          {index + 1}
        </option>
      ))}
    </select>
  );
}
