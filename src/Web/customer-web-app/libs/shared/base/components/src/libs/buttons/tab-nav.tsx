import styles from './tab-nav.module.scss';

export const TabNav = ({ children }: { children: React.ReactNode }) => {
    return (
        <ul className={styles['tab-nav']}>
            {children}
        </ul>
    )
}