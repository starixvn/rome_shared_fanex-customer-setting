import styles from './title.module.scss';
import classNames from 'classnames';

export const Title = ({ children, className }: { children: React.ReactNode, className?: string }) => {
    return (
      <div className={classNames(styles.title, className)}>
        {children}
      </div>
    );
  } 