import classNames from 'classnames';

import { ProductGroup } from '@customer-web-app/shared-group-settings-types';

import styles from './product-tab.module.scss';

const cx = classNames.bind(styles);

type Props = {
  selectedProductGroupId: number;
  productGroups: ProductGroup[];
  handleChangeProductGroupId: (groupId: number) => void;
};

export function ProductTab({ selectedProductGroupId, productGroups, handleChangeProductGroupId }: Props) {
  const handleClick = (groupId: number) => {
    handleChangeProductGroupId(groupId);
  };

  return (
    <div className={styles['nav-product']}>
      {productGroups &&
        productGroups.map((group) => (
          <div
            key={group.id}
            className={cx(styles.item, { [styles.active]: group.id === selectedProductGroupId })}
            aria-hidden="true"
            onClick={() => handleClick(group.id)}
          >
            {group.name}
          </div>
        ))}
    </div>
  );
}
