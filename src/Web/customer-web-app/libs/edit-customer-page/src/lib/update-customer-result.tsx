import { TFunction } from 'i18next';

import { UpdateCustomerResult } from '@customer-web-app/share-models';
import { ErrorBlock } from '@customer-web-app/shared-error-block';

import styles from './update-customer-result.module.scss';

export const renderUpdateCustomerResult = (result: UpdateCustomerResult, t: TFunction) => {
  if (result.subResults) {
    return result.subResults.map((groupType) => {
      return (
        <div key={groupType.groupTypeId} className={styles['block-alert']}>
          {groupType.message && groupType.isSuccessful && (
            <div className={styles['alert-success']}>{groupType.message}</div>
          )}
          {groupType.message && !groupType.isSuccessful && (
            <div className={styles['alert-error']}>{groupType.message}</div>
          )}
          <div className={!groupType.message ? `${styles['alert-error']} ` : ``}>
            {groupType.subMessages &&
              groupType.subMessages.length > 0 &&
              groupType.subMessages.map((error, index) => (
                <div className={styles['note-error']} key={`${error.message}${index}`}>
                  {error.isLive ? (
                    <>
                      <span className="live-badge">{t('Live')}</span>{' '}
                    </>
                  ) : (
                    ''
                  )}
                  {error.message}
                </div>
              ))}
          </div>
        </div>
      );
    });
  }

  if (!result.isSuccessful) {
    return <ErrorBlock error={result} />;
  }

  return null;
};
