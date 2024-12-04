import { useState } from 'react';
import { CopyToClipboard } from 'react-copy-to-clipboard';
import { useTranslation } from 'react-i18next';

import { IconThumbsUp } from '@customer-web-app/shared/base/common-icon';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import styles from './success-popup.module.scss';

type Props = {
  usernames: string[];
  domains: string[];
  credit: number;
  handleOk: () => void;
};

export default function SuccessPopup({ credit, domains, usernames, handleOk }: Props) {
  const { t } = useTranslation();
  const [copied, setCopied] = useState(false);
  const accountInfo = `${t('username')}: ${usernames?.join(', ')}\n${t('credit')}: ${credit}\n${t('domain')}: ${domains
    ?.slice(0, 3)
    .join(', ')}`;
  const handleCopyAccountInfo = () => {
    setCopied(true);
    setTimeout(() => {
      setCopied(false);
    }, 1500);
  };

  return (
    <div className={styles['popup-overlay']}>
      <div className={styles['popup-box']}>
        <div>
          <div className={styles.IconThumbsUp}>{IconThumbsUp}</div>
          <div className={styles.success}>{t('success')}</div>

          <div className={styles['account-info']}>
            <div>
              <span style={{ color: 'red' }}>* </span>
              <span style={{ fontStyle: 'italic', opacity: 0.8 }}>{t('introCopyNote')}</span>
            </div>
            <div>
              <CopyToClipboard text={accountInfo} options={{ format: 'text/plain' }}>
                <button
                  type="button"
                  className={styles['copy-btn']}
                  data-tooltip-id="copy-username-id"
                  onClick={handleCopyAccountInfo}
                >
                  {t('copy')}
                </button>
              </CopyToClipboard>
            </div>
            <div className={styles['info-copy']}>
              <div className={styles['label']}>{t('username')}</div>
              <div className={styles['row-value']}>{usernames?.join(', ')}</div>
            </div>
            <div className={styles['info-copy']}>
              <div className={styles['label']}>{t('credit')}</div>
              <div className={styles['row-value']}>
                {credit.toLocaleString('en-US', {
                  style: 'decimal',
                  maximumFractionDigits: 2,
                }) || '0'}
              </div>
            </div>
            <div className={styles['info-copy']}>
              <div className={styles['label']}>{t('domain')}</div>
              <div className={styles['row-value']}>{domains?.slice(0, 3).join(', ')}</div>
            </div>
            <button className={styles.buttonSuccess} type="button" onClick={handleOk}>
              {t('okay')}
            </button>
          </div>
        </div>
        <WrapperTooltip content={t('copied') ?? ''} id="copy-username-id" type="default" isOpen={copied} />
      </div>
    </div>
  );
}
