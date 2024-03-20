import React, {useState} from 'react';
import classNames from 'classnames';
import styles from './blockv2.module.scss';

interface IBlockV2 {
  title: string;
  colorPrimary?: boolean;
  isCollapse?: boolean;
  children?: React.ReactNode;
}

export function BlockV2({colorPrimary, children, title, isCollapse = true}: IBlockV2) {
  const [collapse, setCollapse] = useState(isCollapse);
 
  const onHeaderClick = (e: React.MouseEvent<HTMLDivElement>) => {
    e.preventDefault();
    setCollapse(!collapse);
  };
  return (
    <div className={styles['accordion']}>
      <div onClick={onHeaderClick} className={classNames(styles['accordion-header'], { [styles.colorPrimary]: colorPrimary }, {[styles.collapse]: !collapse } )} aria-hidden>
        <span
          className={classNames(styles.icon, {
            'icon-chevron-down': !collapse,
            'icon-chevron-right': collapse,
          })}
        />
        {title}
      </div>
      {!collapse && <div className={styles['accordion-content']}>{children}</div>}
    </div>
  );
}
