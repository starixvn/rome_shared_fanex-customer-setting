import styles from './button-tab-active.module.scss';

export const ButtonTabActive: React.FC<{
    text: string;
    onClick?: () => void;
    type?: 'button' | 'submit' | 'reset';
}> = ({
    text,
    onClick,
    type,
  }) => {
    return (
      <button
        type={type || 'button'}
        className={styles['button-tab-active']}
        onClick={onClick}
      >
        {text}
      </button>
    );
  };