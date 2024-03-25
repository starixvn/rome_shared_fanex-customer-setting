import classnames from 'classnames';
import { forwardRef, ReactNode, useImperativeHandle, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import styles from './block.module.scss';

export interface BlockProps {
  title: string | ReactNode;
  children?: ReactNode;
  id?: string;
}

export interface LabelProps {
  label?: string | ReactNode;
  isRequired?: boolean;
}

export interface BlockItemProps extends LabelProps {
  children?: ReactNode;
}

export interface BlockRowProps extends LabelProps {
  children?: ReactNode;
  isHeaderRow?: boolean;
}

export interface BlockRefType {
  expand: () => void;
}

export const Block = forwardRef<BlockRefType, BlockProps>(function Block(props, ref) {
  const { t } = useTranslation();
  const [collapse, setCollapse] = useState(false);

  useImperativeHandle(ref, () => ({
    expand() {
      if (collapse) {
        setCollapse(false);
      }
    },
  }));

  const handleHeaderClick = (e: React.MouseEvent<HTMLDivElement>) => {
    e.preventDefault();
    setCollapse(!collapse);
  };

  const tooltipContent = collapse ? t('ClickToExpand') : t('ClickToCollapse');
  return (
    <div id={props.id} className={styles['container']}>
      <div className={styles['header']} onClick={handleHeaderClick} aria-hidden>
        <span
          className={classnames('ic-arrow-down', { toggle: collapse })}
          data-tooltip-id={`${props.title}-tooltip`}
          data-tooltip-content={tooltipContent}
        />
        {props.title}
      </div>
      {!collapse && <div className={styles['content']}>{props.children}</div>}
      <WrapperTooltip id={`${props.title}-tooltip`} className="global-tooltip" />
    </div>
  );
});

export function Label({ label, isRequired = false }: Readonly<LabelProps>) {
  return (
    <div className={styles['label-text']}>
      {label}
      {isRequired && <span className="require">*</span>}
    </div>
  );
}

export function BlockItem({ label, children, isRequired = false }: Readonly<BlockItemProps>) {
  return (
    <div className={styles.item}>
      <Label label={label} isRequired={isRequired} />
      <div className={styles.control}>{children}</div>
    </div>
  );
}

export function BlockRow({ children, isHeaderRow = false, label }: Readonly<BlockRowProps>) {
  const className = classnames(styles['block-row'], {
    [styles['block-row-head']]: isHeaderRow,
  });

  return (
    <div className={className}>
      <div className={styles['block-label']}>
        <Label label={label} />
      </div>
      <div className={styles['block-value']}>{children}</div>
    </div>
  );
}

export function BlockColumn({ children }: Readonly<{ children: React.ReactNode | undefined }>) {
  return <div className={styles['block-column']}>{children}</div>;
}

export default Block;
