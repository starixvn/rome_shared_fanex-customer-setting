import styles from './button-full.module.scss';

export const ButtonFull: React.FC<{
    text: string;
    onClick: () => void;
    type?: 'button' | 'submit' | 'reset';
    className?: string;
    disabled?: boolean;
}> = ({
    text,
    onClick,  
    type,
    className = '',
    disabled = false,
  }) => {
    return (
      <div className={`${className}`}>
        <button
          type={type || 'button'}
          className={styles['button-full']}
          onClick={onClick}
          disabled={disabled}
        >
          {text}
        </button>
      </div>
    );
  }; 