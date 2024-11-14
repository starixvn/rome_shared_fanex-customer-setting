import classNames from 'classnames';

import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';

import styles from './product-tab.module.scss';

const cx = classNames.bind(styles);

export function ProductTab() {
  const { productGroups, productGroupId, handleChangeProductGroupId } = useGroupSettingContext();
  const currentProductGroupId = Number(productGroupId) || 1;

  const handleClick = (groupId: number) => {
    handleChangeProductGroupId && handleChangeProductGroupId(groupId);
  };

  return (
    <div className={styles['nav-product']}>
      {productGroups &&
        productGroups.map((group) => (
          <div
            key={group.id}
            className={cx(styles.item, { [styles.active]: group.id === currentProductGroupId })}
            aria-hidden="true"
            onClick={() => handleClick(group.id)}
          >
            {group.name}
          </div>
        ))}
    </div>
  );
}
