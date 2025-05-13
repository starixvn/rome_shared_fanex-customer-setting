
import { SettingService } from '@customer-web-app/shared-group-settings-services';
import styles from './mobifone.module.scss';
import { useEffect, useState } from 'react';
import classNames from 'classnames';

export interface MobifoneProps {
  readonly phoneNumber: string;
  readonly onChangePhoneNumber: (phoneNumber: string) => void;
  readonly isError?: boolean;
  readonly onFocus: () => void;
  readonly onBlur: () => void;
}


export function Mobifone({
  phoneNumber,
  onChangePhoneNumber,
  isError = false,
  onFocus,
  onBlur,
}: MobifoneProps) {

    const [countryCode, setCountryCode] = useState<string>('');
    const [countryCodes, setCountryCodes] = useState<string[]>([]);

    useEffect(() => {
        SettingService.GetCountryCodes().then((res) => {
            setCountryCodes(res);
        });
    }, []);

  return (
      <div>
          <select
            className={classNames([styles.phoneCode],{
              error: isError,
            })}
            value={countryCode}
            onChange={(e) => setCountryCode(e.target.value)}
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
          id="phone"
          type="text"
          className={classNames([styles.phoneNumber],{
            error: isError,
          })}
          value={phoneNumber}
          onChange={e => onChangePhoneNumber(e.target.value)}
        />
      </div>
  );
}