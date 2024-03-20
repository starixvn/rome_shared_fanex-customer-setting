import { useTranslation } from 'react-i18next';

import { Result, SubMessagesResult } from '@customer-web-app/share-models';

import styles from './shared-error-block.module.scss';

export interface SharedErrorBlockProps {
  error: SubMessagesResult;
}

export function ErrorBlock({ error }: SharedErrorBlockProps) {
  const { t } = useTranslation();

  return (
    <div className={styles.container}>
      <div className={styles['block-alert']}>
        <div className={styles['alert-error']}>
          {error.code === Result.UnAuthorizedResult.code ? t('YouAreNotAuthorized') : error.message}
        </div>
        {error.subMessages &&
          error.subMessages.length > 0 &&
          error.subMessages.map((subMsg, index) => (
            <div className={styles['note-error']} key={`${subMsg.message}${index}`}>
              {subMsg.isLive && (
                <>
                  <span className="live-badge">{t('Live')}</span>{' '}
                </>
              )}
              {subMsg.message}
            </div>
          ))}
      </div>
    </div>
  );
}

export default ErrorBlock;
