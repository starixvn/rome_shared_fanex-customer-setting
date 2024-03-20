import styles from './shared-block-ui.module.scss';

export function BlockUI() {
  return (
    <div className={styles['container']}>
      <div className="block-loading" />
    </div>
  );
}

export default BlockUI;
