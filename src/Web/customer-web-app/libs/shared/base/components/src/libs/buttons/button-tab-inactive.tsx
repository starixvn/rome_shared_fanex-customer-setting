import styles from './button-tab-inactive.module.scss';

export const ButtonTabInactive: React.FC<{
    text: string;
    onClick: () => void;
    type?: 'button' | 'submit' | 'reset';
    style?: React.CSSProperties;
}> = ({
    text,
    onClick,
    type,
    style,
  }) => {
    return (
      <button
        type={type || 'button'}
        className={styles['button-tab-inactive']}
        onClick={onClick}
        style={style}
      >
        {text}
      </button>    
    );
  };