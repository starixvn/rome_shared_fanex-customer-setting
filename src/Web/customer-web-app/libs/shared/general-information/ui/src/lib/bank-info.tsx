import { SettingService } from '@customer-web-app/shared-group-settings-services';
import { useEffect, useState } from 'react';
import styles from './general-information.module.scss';
import stylesDropdown from './bank-info.module.scss';
import { BlockItem } from '@customer-web-app/shared/base/block';
import { useTranslation } from 'react-i18next';
import classNames from 'classnames';

interface Bank {
  id: string;
  name: string;
  code?: string;
}

interface BankSelectProps {
  bankId?: string;
  bankAccountNumber?: string;
  onChangeBankId: (bankId: string) => void;
  onChangeBankAccountNumber: (bankAccountNumber: string) => void;
}

const BankDropdown = ({ banks }: { banks: Bank[] }) => {
  const [selectedBank, setSelectedBank] = useState<Bank | null>(null);
  const [isOpen, setIsOpen] = useState(false);
  return (
    <div className={stylesDropdown['bank-dropdown']}>
      <div className={stylesDropdown['bank-dropdown-selected']} onClick={() => setIsOpen(!isOpen)}>
        {selectedBank?.code &&
          <img
            src={`${(window.top as any).alpha.urls.cashServiceRootPath}assets/bank/${selectedBank.code}.png`}
            style={{ width: '18px', height: '18px', marginRight: '8px' }}
          />}
        {selectedBank?.name || 'Select Bank Account'}
        <span style={{ marginLeft: 'auto' }}>▼</span>
      </div>
      {isOpen &&
        <div className={stylesDropdown['bank-options']}>
          {banks.map(bank => (
            <div key={bank.id} onClick={() => {
              setSelectedBank(bank);
              setIsOpen(false);
            }}>
              {bank.code && <img src={`${(window.top as any).alpha.urls.cashServiceRootPath}assets/bank/${bank.code}.png`} style={{ width: '20px', height: '20px', marginRight: '8px' }} />}
              {bank.name}
            </div>
          ))}
        </div>
      }
    </div>
  );
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
          <BankDropdown banks={banks} />
        </BlockItem>
        <BlockItem label={t('AccountNumber')}>
          <input type="text" value={bankAccountNumber} onChange={e => onChangeBankAccountNumber(e.target.value)} />
        </BlockItem>
      </div >
    </div >)
}


