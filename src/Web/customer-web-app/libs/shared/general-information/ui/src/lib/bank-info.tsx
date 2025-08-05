import { SettingService } from '@customer-web-app/shared-group-settings-services';
import { useEffect, useMemo, useRef, useState, useCallback } from 'react';
import styles from './general-information.module.scss';
import stylesDropdown from './bank-info.module.scss';
import { BlockItem } from '@customer-web-app/shared/base/block';
import { useTranslation } from 'react-i18next';
import classNames from 'classnames';
import { inputNumberOnly } from '@customer-web-app/shared-helpers';
import { currentCdnUri } from '@customer-web-app/shared-api-helper';
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
  readonly isBankIdError: boolean;
  readonly onFocusBankAccountNumber: () => void;
  readonly onBlurBankAccountNumber: () => void;
}

const BankDropdown = ({ banks, onChangeBankId, isBankIdError }: { banks: Bank[], onChangeBankId: (bankId: number) => void, isBankIdError: boolean }) => {
  const { t } = useTranslation();
  const [selectedBank, setSelectedBank] = useState<Bank | null>(null);
  const [isOpen, setIsOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const emptyBank: Bank = { id: 0, name: t('SelectBankAccount'), code: '' };
  const wrapperRef = useRef<HTMLDivElement>(null);
  
  const baseUrl = useMemo(() => 
    currentCdnUri(),
    []
  );

  const getBankImageUrl = useCallback((code: string) => 
    `${baseUrl}cash-member/create/bank/${code?.toLocaleUpperCase()}.png`,
    [baseUrl]
  );

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
    <div className={stylesDropdown['bank-dropdown']} ref={wrapperRef} id="bankId">
      <div className={classNames(stylesDropdown['bank-dropdown__selected'], { [stylesDropdown['bank-dropdown__selected--error']]: isBankIdError })} onClick={() => setIsOpen(!isOpen)}>
        {selectedBank?.code &&
          <img
            alt={selectedBank.name}
            src={getBankImageUrl(selectedBank.code)}
            style={{ width: '16px', height: '16px', marginRight: '8px', top: '-1px', position: 'relative' }}
          />}
        <span className={stylesDropdown['bank-name']}>
          {selectedBank?.name || emptyBank.name}
        </span>
        <span className={stylesDropdown['bank-dropdown-arrow']}>
          <svg viewBox="0 0 32 32" xmlns="http://www.w3.org/2000/svg"><path d="M15.8909 23.6457C15.3613 23.6435 14.8543 23.4313 14.4809 23.0557L4.48091 13.0557C4.15326 12.6731 3.98205 12.1809 4.00149 11.6776C4.02093 11.1742 4.2296 10.6967 4.58579 10.3406C4.94198 9.98437 5.88727 9.16523 6.39062 9.14579C6.39062 9.14579 7.00802 8.81802 7.39062 9.14567L15.8906 17.6456L24.3906 9.14559C24.7732 8.81794 25.8906 9.14569 25.8906 9.14569C26.394 9.16513 26.8399 9.98437 27.196 10.3406C27.5522 10.6967 27.7609 11.1742 27.7803 11.6776C27.7998 12.1809 27.6286 12.6731 27.3009 13.0557L17.3009 23.0557C16.9276 23.4313 16.4205 23.6435 15.8909 23.6457Z"></path></svg>
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
          {filteredBanks.map((bank) => (
            <div key={bank.id} className={classNames(stylesDropdown['bank-dropdown__item'], { [stylesDropdown['bank-dropdown__item--selected']]: selectedBank?.id === bank.id })} onClick={() => {
              setSelectedBank(bank);
              setIsOpen(false);
              onChangeBankId(bank.id);
              setSearchTerm('');
            }}>
              {bank.code && <img alt={bank.name} src={getBankImageUrl(bank.code)} style={{ width: '20px', height: '20px', marginRight: '8px' }} />}
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
    isBankIdError,
    onFocusBankAccountNumber,
    onBlurBankAccountNumber
  } = props;
  
  useEffect(() => {
    async function fetchBanks() {
      const result = await SettingService.GetBanks<{bankId: number, bankName: string, bankCode: string}[]>();
      const banks = result.map((bank: {bankId: number, bankName: string, bankCode: string}) => {
        return {
          id: bank.bankId,
          name: t(bank.bankCode),
          code: bank.bankCode
        }
      });
      setBanks(banks || []);
    }
    fetchBanks();
  }, []);

  return (
    <div className={styles['user-info']}>
      <div className={styles['subheader']}>
        {t('BankInformation')}
      </div>
      <div className={classNames(styles['row'])}>
        <BlockItem label={t('Bank')}>
          <BankDropdown banks={banks} onChangeBankId={onChangeBankId} isBankIdError={isBankIdError} />
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
            onBlur={onBlurBankAccountNumber} 
            maxLength={50}/>
        </BlockItem>
      </div >
    </div >)
}


