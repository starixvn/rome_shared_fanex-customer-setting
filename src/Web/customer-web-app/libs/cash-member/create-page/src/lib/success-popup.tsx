import { useState } from 'react';
import { CopyToClipboard } from 'react-copy-to-clipboard';
import { useTranslation } from 'react-i18next';

import { CreateCustomerResult } from '@customer-web-app/share-models';
import { IconThumbsUp } from '@customer-web-app/shared/base/common-icon';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import styles from './success-popup.module.scss';

type Props = {
  credit: number;
  numberOfAccounts: number;
  handleOk: () => void;
} & CreateCustomerResult;

export default function CreateCashMemberSuccessPopup(props: Props) {
  const { t } = useTranslation();
  const [copied, setCopied] = useState(false);
  const { newUserNames, handleOk, numberOfAccounts, message, subMessages } = props;
  const credit = props.credit.toLocaleString('en-US', { style: 'decimal', maximumFractionDigits: 2 });
  const domains = props.availableAlphaDomain?.slice(0, 3).join(', ');

  const noOfFailedAccounts = numberOfAccounts - (newUserNames?.length ?? 0);

  const formatUserInfo = (usernameText: string, creditText: string, domainText: string) => {
    return `${t('username')}: ${usernameText}\n${t('credit')}: ${creditText}\n${t('domain')}: ${domainText}`;
  };

  const compactText = formatUserInfo(newUserNames?.join(', ') ?? '', credit, domains ?? '');

  const individualText =
    newUserNames?.map((username) => formatUserInfo(username, credit, domains ?? '')).join('\n\n') ?? '';

  const handleCopy = () => {
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
            <div className={styles['buttons']}>
              <CopyToClipboard text={compactText} options={{ format: 'text/plain' }}>
                <button
                  type="button"
                  className={styles['copy-btn']}
                  data-tooltip-id="copy-username-id"
                  onClick={handleCopy}
                >
                  {t('CompactCopy')}
                </button>
              </CopyToClipboard>
              <CopyToClipboard text={individualText} options={{ format: 'text/plain' }}>
                <button
                  type="button"
                  className={styles['copy-btn']}
                  data-tooltip-id="copy-username-id"
                  onClick={handleCopy}
                >
                  {t('IndividualCopy')}
                </button>
              </CopyToClipboard>
            </div>
            <div className={styles['info-copy']}>
              <div className={styles['label']}>{t('username')}</div>
              <div className={styles['row-value-username']}>
                <div>{newUserNames?.join(', ')}</div>
              </div>
            </div>
            <div className={styles['info-copy']}>
              <div className={styles['label']}>{t('Credit')}</div>
              <div className={styles['row-value']}>{credit}</div>
            </div>
            <div className={styles['info-copy']}>
              <div className={styles['label']}>{t('Domain')}</div>
              <div className={styles['row-value']}>{domains}</div>
            </div>
            {noOfFailedAccounts > 0 && (
              <div className={styles['failed-account-created']}>
                <p className={`msg msg-error ${styles['result-text']}`}>
                  {t('NoOfAccountCreatedFailed', { noOfFailedAccounts })}
                </p>

                <p className="result-text">
                  <span>{t('FailureReason')}: </span>
                  <div className="msg msg-error">
                    <div className="main-message">{message}</div>
                    {subMessages?.map((subMessage) => (
                      <div className={styles['sub-message']}>
                        {subMessage.isLive && <span className={styles['live-badge']}>{t('Live')}</span>}
                        {subMessage.message}
                      </div>
                    ))}
                  </div>
                </p>
              </div>
            )}

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
