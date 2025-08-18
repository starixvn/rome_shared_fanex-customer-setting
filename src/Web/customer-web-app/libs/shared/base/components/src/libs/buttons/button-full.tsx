import styles from './button-full.module.scss';

export const ButtonFull: React.FC<{
    text: string;
    onClick: () => void;
    type?: 'button' | 'submit' | 'reset';
    className?: string;
    disabled?: boolean;
    style?: React.CSSProperties;
    title?: string;
}> = ({
    text,
    onClick,  
    type,
    className = '',
    disabled = false,
    style,
    title,
  }) => {
    return (
      <div className={`${className}`} style={style}>
        <button
          type={type || 'button'}
          className={styles['button-full']}
          onClick={onClick}
          disabled={disabled}
          title={title}
        >
          {text}
        </button>
      </div>
    );
  }; 