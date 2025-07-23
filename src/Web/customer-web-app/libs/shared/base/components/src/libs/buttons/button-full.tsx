import styles from './button-full.module.scss';

export const ButtonFull: React.FC<{
    text: string;
    onClick: () => void;
    type?: 'button' | 'submit' | 'reset';
    className?: string;
    disabled?: boolean;
    style?: React.CSSProperties;
}> = ({
    text,
    onClick,  
    type,
    className = '',
    disabled = false,
    style,
  }) => {
    return (
      <div className={`${className}`} style={style}>
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