import styles from './shared-block-ui.module.scss';
import { useTheme } from '@customer-web-app/shared/base/layout';
export function BlockUI() {
  const theme = useTheme();
  return (
    <div className={styles['container']} data-theme={theme}>
      <div className="block-loading" />
    </div>
  );
}

export default BlockUI;
