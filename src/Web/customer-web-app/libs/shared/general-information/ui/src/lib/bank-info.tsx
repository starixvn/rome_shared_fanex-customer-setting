import { SettingService } from '@customer-web-app/shared-group-settings-services';
import { useEffect, useMemo, useRef, useState } from 'react';
import styles from './general-information.module.scss';
import stylesDropdown from './bank-info.module.scss';
import { BlockItem } from '@customer-web-app/shared/base/block';
import { useTranslation } from 'react-i18next';
import classNames from 'classnames';
import { inputNumberOnly } from '@customer-web-app/shared-helpers';

interface Bank {
  id: number;
  name: string;
  code: string;
}

interface BankSelectProps {
  readonly bankId?: number;
  readonly bankAccountNumber?: string;
  readonly onChangeBankId: (bankId: number) => void;
  readonly onChangeBankAccountNumber: (bankAccountNumber: string) => void;
  readonly isBankAccountNumberError: boolean;
  readonly onFocusBankAccountNumber: () => void;
  readonly onBlurBankAccountNumber: () => void;
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

const BankDropdown = ({ banks, onChangeBankId }: { banks: Bank[], onChangeBankId: (bankId: number) => void }) => {
  const { t } = useTranslation();
  const [selectedBank, setSelectedBank] = useState<Bank | null>(null);
  const [isOpen, setIsOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const emptyBank: Bank = { id: 0, name: t('SelectBankAccount'), code: '' };
  const wrapperRef = useRef<HTMLDivElement>(null);
  const bankImageUrls = useMemo(() =>
    banks
      .filter(bank => bank.code)
      .map(bank => `${cdnUri}/bank/${bank.code?.toLocaleUpperCase()}.png`),
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

  const filteredBanks = useMemo(() => {
    if (!searchTerm) return [emptyBank, ...banks];
    const searchLower = searchTerm.toLowerCase();
    return [emptyBank, ...banks].filter(bank =>
      bank.name.toLowerCase().includes(searchLower)
    );
  }, [banks, searchTerm]);

  return (
    <div className={stylesDropdown['bank-dropdown']} ref={wrapperRef}>
      <div className={stylesDropdown['bank-dropdown__selected']} onClick={() => setIsOpen(!isOpen)}>
        {selectedBank?.code &&
          <img
            alt={selectedBank.name}
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
      {isOpen && (
        <div className={stylesDropdown['bank-dropdown__menu']}>
          <div className={stylesDropdown['bank-dropdown__search']}>
            <input
              type="text"
              placeholder={t('SearchBank')}
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              onClick={(e) => e.stopPropagation()}
              onKeyDown={(e) => {
                if (e.key === 'Tab' && filteredBanks.length > 0) {
                  e.preventDefault();
                  const firstBank = filteredBanks[0];
                  setSelectedBank(firstBank);
                  setIsOpen(false);
                  onChangeBankId(firstBank.id);
                  setSearchTerm('');
                }
              }}
            />
          </div>
          <div className={stylesDropdown['bank-dropdown__list']}>
          {filteredBanks.map((bank, index) => (
            <div key={bank.id} className={classNames(stylesDropdown['bank-dropdown__item'], { [stylesDropdown['bank-dropdown__item--selected']]: selectedBank?.id === bank.id })} onClick={() => {
              setSelectedBank(bank);
              setIsOpen(false);
              onChangeBankId(bank.id);
              setSearchTerm('');
            }}>
              {bank.code && <img alt={bank.name} src={`${(window.top as any).alpha.urls.cashServiceRootPath}assets/bank/${bank.code}.png`} style={{ width: '20px', height: '20px', marginRight: '8px' }} />}
              {bank.id === 0 ? <span style={{ padding: '0 0 0 28px' }}>{bank.name}</span> : bank.name}
            </div>
          ))}
          </div>
        </div>
      )}
    </div>
  );
}

export function BankInfomation(props: BankSelectProps) {
  const { t } = useTranslation();
  const [banks, setBanks] = useState<Bank[]>([]);
  const {
    bankAccountNumber,
    onChangeBankId,
    onChangeBankAccountNumber,
    isBankAccountNumberError,
    onFocusBankAccountNumber,
    onBlurBankAccountNumber
  } = props;
  
  useEffect(() => {
    async function fetchBanks() {
      const result = await SettingService.GetBanks();
      result.map((bank: Bank) => {
        bank.name = t(bank.code);
      });
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
          <BankDropdown banks={banks} onChangeBankId={onChangeBankId} />
        </BlockItem>
        <BlockItem label={t('AccountNumber')}>
          <input
            id="bankAccountNumber"
            type="text"
            value={bankAccountNumber}
            className={classNames({ error: isBankAccountNumberError })}
            onChange={e => {
              const value = inputNumberOnly(e.target.value);
              onChangeBankAccountNumber(value);
            }}
            onFocus={onFocusBankAccountNumber}
            onBlur={onBlurBankAccountNumber} />
        </BlockItem>
      </div >
    </div >)
}


