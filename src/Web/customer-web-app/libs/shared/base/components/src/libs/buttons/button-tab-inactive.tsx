import styles from './button-tab-inactive.module.scss';

export const ButtonTabInactive: React.FC<{
    text: string;
    onClick: () => void;
    type?: 'button' | 'submit' | 'reset';
}> = ({
    text,
    onClick,
    type,
  }) => {
    return (
      <button
        type={type || 'button'}
        className={styles['button-tab-inactive']}
        onClick={onClick}
      >
        {text}
      </button>    
    );
  };