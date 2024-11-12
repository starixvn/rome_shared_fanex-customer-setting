import styles from './no-of-accounts.module.scss';

export default function NoOfAccounts() {
  return (
    <select className={styles.dropdown}>
      {[...Array(15)].map((value, index) => (
        <option key={index} value={index + 1}>
          {index + 1}
        </option>
      ))}
    </select>
  );
}
