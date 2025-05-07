
import { SettingService } from '@customer-web-app/shared-group-settings-services';
import styles from './mobifone.module.scss';
import { useEffect, useState } from 'react';

export interface MobifoneProps {
  readonly phoneNumber: string;
  readonly onChangePhoneNumber: (phoneNumber: string) => void;
}


export function Mobifone({
  phoneNumber,
  onChangePhoneNumber,
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
            className={styles.phoneCode}
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
          type="text"
          className={styles.phoneNumber}
          value={phoneNumber}
          onChange={e => onChangePhoneNumber(e.target.value)}
        />
      </div>
  );
}