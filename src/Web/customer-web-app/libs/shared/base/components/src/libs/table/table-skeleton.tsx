import styles from './table-skeleton.module.scss';

export const TableSkeleton = ({ children }: { children: React.ReactNode }) => {
    return (
        <div className={styles['table-skeleton']}>
            <table>
                {children}
            </table>
        </div>
    )
}