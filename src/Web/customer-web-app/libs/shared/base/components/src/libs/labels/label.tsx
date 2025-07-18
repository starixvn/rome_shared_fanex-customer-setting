import styles from './label.module.scss';

type LabelProps = {
    children: string | React.ReactNode;
    required?: boolean;
    className?: string;
    style?: React.CSSProperties;
}

export const Label = ({ children, required, className, style }: LabelProps) => {
    return (
        <label className={`${styles['label-component']} ${className || ''}`} style={style}>
        {children}
        {required && <span className={styles['required']}>*</span>}
      </label>
    )
}