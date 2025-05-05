import { SettingService } from '@customer-web-app/shared-group-settings-services';
import { useEffect, useState } from 'react';
import styles from './general-information.module.scss';
import { BlockItem } from '@customer-web-app/shared/base/block';
import { useTranslation } from 'react-i18next';
import classNames from 'classnames';

interface Bank {
  id: string;
  name: string;
}

interface BankSelectProps {
  bankId?: string;
  bankAccountNumber?: string;
  onChangeBankId: (bankId: string) => void;
  onChangeBankAccountNumber: (bankAccountNumber: string) => void;
}


export function BankInfomation({ bankId, bankAccountNumber, onChangeBankId, onChangeBankAccountNumber }: BankSelectProps) {
  const { t } = useTranslation();
  const [banks, setBanks] = useState<Bank[]>([]);

  useEffect(() => {
    async function fetchBanks() {
      const result = await SettingService.GetBanks();
      setBanks(result || []);
    }
    fetchBanks();
  }, []);
  return (
    <div className={styles['user-info']}>
      <div className={styles['subheader']}>
        {t('BankInfo')}
      </div>
      <div className={classNames(styles['row'])}>
        <BlockItem label={t('Bank')}>
          <select value={bankId} onChange={e => onChangeBankId(e.target.value)}>
            <option value="">Select Bank Account</option>
            {banks.map(bank => (
              <option key={bank.id} value={bank.id}>
                {bank.name}
              </option>
            ))}
          </select>
        </BlockItem>
        <BlockItem label={t('AccountNumber')}>
          <input type="text" value={bankAccountNumber} onChange={e => onChangeBankAccountNumber(e.target.value)} />
        </BlockItem>
      </div >
    </div >)
}


