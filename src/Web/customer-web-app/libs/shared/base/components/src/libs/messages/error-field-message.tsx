import styles from './error-field-message.module.scss';

export const ErrorFieldMessage = ({ message, style }: { message: string, style?: React.CSSProperties }) => {
    return <div className={styles.errorFieldMessage} style={style}>{message}</div>;
};
