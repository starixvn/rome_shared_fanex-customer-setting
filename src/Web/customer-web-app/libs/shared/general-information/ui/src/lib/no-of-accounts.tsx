import { ChangeEvent } from 'react';

import styles from './no-of-accounts.module.scss';

type Props = {
  value: number;
  onChange: (event: ChangeEvent<HTMLSelectElement>) => void;
};

export default function NoOfAccounts({ value, onChange }: Props) {
  return (
    <select className={styles.dropdown} value={value} onChange={onChange}>
      {[...Array(30)].map((_, index) => (
        <option key={index} value={index + 1}>
          {index + 1}
        </option>
      ))}
    </select>
  );
}
