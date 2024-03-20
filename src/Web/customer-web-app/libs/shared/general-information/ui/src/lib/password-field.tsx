import classNames from 'classnames';
import { ChangeEvent, useState } from 'react';

import styles from './password-field.module.scss';

export interface PasswordFieldProps {
  isError?: boolean;
  onPasswordChange: (event: ChangeEvent<HTMLInputElement>) => void;
  onFocus?: () => void;
  onBlur?: () => void;
  value?: string;
}

export function PasswordField({ isError = false, onPasswordChange, onFocus, onBlur, value}: PasswordFieldProps) {
  const [showPassword, setShowPassword] = useState(false);

  return (
    <div className={styles.password}>
      <input
        className={classNames({ error: isError })}
        value={value ?? ""}
        maxLength={16}
        id="password"
        type={showPassword ? 'text' : 'password'}
        name="password"
        onChange={onPasswordChange}
        onFocus={onFocus}
        onBlur={onBlur}
        autoComplete="new-password"
      />
      <span
        className={!showPassword ? 'ic-eye-close' : 'ic-eye'}
        aria-hidden="true"
        onClick={() => setShowPassword(!showPassword)}
      ></span>
    </div>
  );
}
