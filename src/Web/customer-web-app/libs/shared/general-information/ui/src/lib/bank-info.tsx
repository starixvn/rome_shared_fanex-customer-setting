import { SettingService } from '@customer-web-app/shared-group-settings-services';
import { useEffect, useMemo, useRef, useState } from 'react';
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

const usePreloadImages = (urls: string[]) => {
  useEffect(() => {
    const preloadImages = urls.map(url => {
      return new Promise((resolve, reject) => {
        const img = new Image();
        img.src = url;
        img.onload = resolve;
        img.onerror = reject;
      });
    });

    Promise.all(preloadImages).catch(console.error);
  }, [urls]);
};

const BankDropdown = ({ banks }: { banks: Bank[] }) => {
  const [selectedBank, setSelectedBank] = useState<Bank | null>(null);
  const [isOpen, setIsOpen] = useState(false);
  const emptyBank: Bank = { id: '', name: 'Select Bank Account' };
  const wrapperRef = useRef<HTMLDivElement>(null);
  const bankImageUrls = useMemo(() =>
    banks
      .filter(bank => bank.code)
      .map(bank => `${(window.top as any).alpha.urls.cashServiceRootPath}assets/bank/${bank.code}.png`),
    [banks]
  );
  usePreloadImages(bankImageUrls);
  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (!isOpen) return;
      if (wrapperRef.current && !wrapperRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    }
    if (isOpen) {
      document.addEventListener('mousedown', handleClickOutside);
    }

    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [isOpen]);

  return (
    <div className={stylesDropdown['bank-dropdown']} ref={wrapperRef}>
      <div className={stylesDropdown['bank-dropdown-selected']} onClick={() => setIsOpen(!isOpen)}>
        {selectedBank?.code &&
          <img
            src={`${(window.top as any).alpha.urls.cashServiceRootPath}assets/bank/${selectedBank.code}.png`}
            style={{ width: '18px', height: '18px', marginRight: '8px' }}
          />}
        <span className={stylesDropdown['bank-name']}>
          {selectedBank?.name || emptyBank.name}
        </span>
        <span className={stylesDropdown['bank-dropdown-arrow']}>
          <svg width="10" height="12" viewBox="0 0 12 12" fill="none">
            <path d="M2 4L6 8L10 4" stroke="currentColor" strokeWidth="1.6" strokeLinecap="round" strokeLinejoin="round" />
          </svg>
        </span>
      </div>
      {isOpen &&
        <div className={stylesDropdown['bank-options']}>
          {[emptyBank, ...banks].map(bank => (
            <div key={bank.id} className={classNames({ [stylesDropdown['bank-option-selected']]: selectedBank?.id === bank.id })} onClick={() => {
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


