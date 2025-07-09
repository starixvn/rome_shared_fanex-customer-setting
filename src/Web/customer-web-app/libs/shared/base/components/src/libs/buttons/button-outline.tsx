import styles from './button-outline.module.scss';

export const ButtonOutline: React.FC<{
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
        className={styles['button-outline']}
        onClick={onClick}
        disabled={disabled}
      >
        {text}
      </button>
    </div>
  );
}; 