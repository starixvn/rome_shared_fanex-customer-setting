import classNames from 'classnames';
import styles from './table-skeleton.module.scss';

export const TableSkeleton = ({ children, className }: { children: React.ReactNode, className?: string }) => {
    return (
        <div className={classNames(styles['table-skeleton'], className)}>
            <table >
                {children}
            </table>
        </div>
    )
}