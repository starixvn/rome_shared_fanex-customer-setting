import { SettingService } from '@customer-web-app/shared-group-settings-services';
import styles from './mobifone.module.scss';
import { useEffect, useState } from 'react';
import classNames from 'classnames';
import { inputNumberOnly } from '@customer-web-app/shared-helpers';

export interface MobifoneProps {
  readonly id: string;
  readonly phoneNumber: string;
  readonly onChangePhoneNumber: (phoneNumber: string) => void;
  readonly isError?: boolean;
  readonly onFocus: () => void;
  readonly onBlur: () => void;
}

export function Mobifone({
  id,
  phoneNumber,
  onChangePhoneNumber,
  isError = false,
  onFocus,
  onBlur,
}: MobifoneProps) {
  const [countryCode, setCountryCode] = useState<string>(phoneNumber?.split('-')[0] || '');

  useEffect(() => {
    SettingService.GetCountryCodes().then((res) => {
      countryCode == '' && setCountryCode(res[0]);
    });
  }, []);

  const handleMobifoneCodeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = inputNumberOnly(e.target.value);
    setCountryCode(value);
    onChangePhoneNumber(`${value}-${phoneNumber.split('-')[1] ?? ''}`);
  }

  const handleMobifoneNumberChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = inputNumberOnly(e.target.value);
    onChangePhoneNumber(`${countryCode}-${value}`);
  }

  return (
    <div className={styles.phoneCodeContainer}>
      <input
        id="phoneCode"
        key="phoneCode"
        type="text"
        value={`+${countryCode}`}
        onChange={handleMobifoneCodeChange}
        className={classNames([styles.phoneCode, isError && styles.error])}
        onFocus={onFocus} onBlur={onBlur}
        maxLength={4}
        autoFocus
      />
      <input
        id="phoneNumber"
        key="phoneNumber"
        type="text"
        maxLength={45}
        className={classNames([styles.phoneNumber, isError && styles.error])}
        onFocus={onFocus} onBlur={onBlur}
        value={phoneNumber.split('-')[1] ?? ''}
        onChange={handleMobifoneNumberChange}
      />
    </div>
  );
}