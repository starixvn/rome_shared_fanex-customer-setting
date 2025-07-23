import { useEffect, useState } from 'react';
import { CopyToClipboard } from 'react-copy-to-clipboard';
import { useTranslation } from 'react-i18next';

import { CreateCustomerResult } from '@customer-web-app/share-models';
import { IconThumbsUp } from '@customer-web-app/shared/base/common-icon';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import styles from './success-popup.module.scss';
import { SettingService } from '@customer-web-app/shared-group-settings-services';

declare global {
  interface Window {
    indexManager: any;
    handleReferChecklist: () => void;
  }
}

type Props = {
  credit: number;
  numberOfAccounts: number;
  handleOk: () => void;
} & CreateCustomerResult;



export default function CreateCashMemberSuccessPopup(props: Props) {
  const { t } = useTranslation();
  const [copied, setCopied] = useState("");
  const [isCompleteSetup, setIsCompleteSetup] = useState(false);
  const [isCanEditCashService, setIsCanEditCashService] = useState(false);
  const [compactText, setCompactText] = useState('');
  const [individualText, setIndividualText] = useState('');

  const { newUserNames, handleOk, numberOfAccounts, message, subMessages } = props;
  
  const domains = props.availableAlphaDomain?.slice(0, 3).join(', ');

  const noOfFailedAccounts = numberOfAccounts - (newUserNames?.length ?? 0);

  const formatUserInfo = (usernameText: string, domainText: string, isCompleteSetup: boolean=false) => {
    return isCompleteSetup 
      ? `${t('username')}: ${usernameText}\n${t('domain')}: ${domainText}`
      : `${t('username')}: ${usernameText}`;
  };

  const numberOfNewUserNames = newUserNames?.length ?? 0;

  const handleCopy = (btnId: string) => {
    setCopied(btnId);
    setTimeout(() => {
      setCopied('');
    }, 1500);
  };

  const getSetupChecklist = async () => {
    const data = await SettingService.GetSetupChecklist();

    const isCanEditCashService = data.isCanEditCashService;
    setIsCanEditCashService(isCanEditCashService);

    const isCompleteAll = isCanEditCashService && data.isCompleteAll;
    setIsCompleteSetup(isCompleteAll);

    const ct = formatUserInfo(newUserNames?.join(', ') ?? '', domains ?? '', isCompleteAll) ;
    const it = newUserNames?.map((username) => formatUserInfo(username, domains ?? '', isCompleteAll)).join('\n\n') ?? '';
    setCompactText(ct);
    setIndividualText(it);
  };

  useEffect(() => {
    getSetupChecklist();
  }, []);

  const handleReferChecklist = () => {
    window.top && window.top.indexManager.showHideCashServiceChecklist(true);
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
              <CopyToClipboard text={compactText} options={{ format: 'text/plain' }} onCopy={() => handleCopy('compact-copy-btn')}>
                <button
                  type="button"
                  className={styles['copy-btn']}
                  data-tooltip-id="compact-copy-btn"
                >
                  {t('Copy')}
                </button>
              </CopyToClipboard>
              {numberOfNewUserNames > 1 && (
                <CopyToClipboard text={individualText} options={{ format: 'text/plain' }} onCopy={() => handleCopy('individual-copy-btn')}>
                  <button
                    type="button"
                    className={styles['copy-btn']}
                    data-tooltip-id="individual-copy-btn"
                  >
                    {t('IndividualCopy')}
                  </button>
                </CopyToClipboard>
              )}
            </div>
            <div className={styles['info-copy']}>
              <div className={styles['label']}>{t('username')}</div>
              <div className={styles['row-value-username']}>
                <div>{newUserNames?.join(', ')}</div>
              </div>
            </div>

            {isCompleteSetup && (
              <div className={styles['info-copy']}>
                <div className={styles['label']}>{t('Domain')}</div>
                <div className={styles['row-value']}>{domains}</div>
              </div>)}

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

            {isCanEditCashService && !isCompleteSetup && (            
              <div className={styles['refer-checklist']}>
                <span className="icon ic-info"></span>
                <span dangerouslySetInnerHTML={{ __html: t('ReferChecklist') }} />
                <button type="button" onClick={handleReferChecklist} className={styles['checklist-link']}>{t('checklist')}</button>
              </div>
            )}
            
            <button className={styles.buttonSuccess} type="button" onClick={handleOk}>
              {t('okay')}
            </button>
          </div>

        </div>
        <WrapperTooltip content={t('copied') ?? ''} id={copied} type="default" isOpen={copied !== ''} />
      </div>
    </div>
  );
}
