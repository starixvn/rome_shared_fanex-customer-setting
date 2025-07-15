import styles from './error-field-message.module.scss';

export const ErrorFieldMessage = ({ message }: { message: string }) => {
    return <div className={styles.errorFieldMessage}>{message}</div>;
};
