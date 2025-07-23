import React from 'react';
import styles from './button-cancel.module.scss';

export const ButtonCancel: React.FC<{
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
        type={type}
        className={styles.buttonCancel}
        onClick={onClick}
        disabled={disabled}
      >
        {text}
      </button>
    </div>
  );
}; 