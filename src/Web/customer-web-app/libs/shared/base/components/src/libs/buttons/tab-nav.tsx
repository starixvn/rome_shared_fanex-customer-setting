import styles from './tab-nav.module.scss';

export const TabNav = ({ children, style }: { children: React.ReactNode, style?: React.CSSProperties }) => {
    return (
        <ul className={styles['tab-nav']} style={style}>
            {children}
        </ul>
    )
}