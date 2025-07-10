import classNames from 'classnames';
import styles from './button-icon.module.scss';

export const ButtonIcon: React.FC<{
    onClick: () => void;
    disabled?: boolean;
    type?: 'button' | 'submit' | 'reset';
    icon?: React.ReactNode;
    className?: string;
}> = ({ onClick, disabled, type, icon, className }) => {
    return (
        <button
            type={type || 'button'}
            className={classNames(styles.buttonIcon, className, {
                [styles.disabled]: disabled,
            })}
            onClick={onClick}
            disabled={disabled}
        >
            {icon}
        </button>
    );
};