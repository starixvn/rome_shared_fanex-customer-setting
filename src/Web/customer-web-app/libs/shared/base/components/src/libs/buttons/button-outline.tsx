import styles from './button-outline.module.scss';

export const ButtonOutline: React.FC<{
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
    <div className={`${className}`}>
      <button
        type={type || 'button'}
        className={styles['button-outline']}
        onClick={onClick}
        disabled={disabled}
        style={style}
      >
        {text}
      </button>
    </div>
  );
}; 