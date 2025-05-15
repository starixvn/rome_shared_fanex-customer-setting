
import { SettingService } from '@customer-web-app/shared-group-settings-services';
import styles from './mobifone.module.scss';
import { useEffect, useState } from 'react';
import classNames from 'classnames';

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
  const [countryCodes, setCountryCodes] = useState<string[]>([]);

  useEffect(() => {
    SettingService.GetCountryCodes().then((res) => {
      countryCode == '' && setCountryCode(res[0]);
      setCountryCodes(res);
    });
  }, []);

  return (
    <div>
      <select
        className={classNames([styles.phoneCode, isError && styles.error])}
        value={countryCode}
        onChange={(e) => {
          setCountryCode(e.target.value);
          onChangePhoneNumber(`${e.target.value}-${phoneNumber.split('-')[1] ?? ''}`);
        }}
      >
        {countryCodes.map((code) => (
          <option key={code} value={code}>
            +{code}
          </option>
        ))}
      </select>
      <input
        onFocus={onFocus}
        onBlur={onBlur}
        id={id}
        type="text"
        className={classNames([styles.phoneNumber, isError && styles.error])}
        value={phoneNumber.split('-')[1] ?? ''}
        onChange={e => onChangePhoneNumber(`${countryCode}-${e.target.value}`)}
      />
    </div>
  );
}