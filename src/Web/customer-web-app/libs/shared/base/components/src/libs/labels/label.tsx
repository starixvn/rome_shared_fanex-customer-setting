import styles from './label.module.scss';

type LabelProps = {
    children: string | React.ReactNode;
    required?: boolean;
    className?: string;
}

export const Label = ({ children, required, className }: LabelProps) => {
    return (
        <label className={`${styles['label-component']} ${className || ''}`}>
        {children}
        {required && <span className={styles['required']}>*</span>}
      </label>
    )
}