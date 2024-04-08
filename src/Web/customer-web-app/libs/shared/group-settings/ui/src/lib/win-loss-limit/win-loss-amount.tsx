import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { BlockUI } from '@customer-web-app/shared-block-ui';
import { SportTypeIds } from '@customer-web-app/shared-constants';
import { SettingService } from '@customer-web-app/shared-group-settings-services';
import { CurrentWinLossAmount } from '@customer-web-app/shared-group-settings-types';
import { BlockColumn, BlockRow } from '@customer-web-app/shared/base/block';
import { IconClose } from '@customer-web-app/shared/base/common-icon';
import { useTheme } from '@customer-web-app/shared/base/layout';
import NiceModal, { useModal } from '@ebay/nice-modal-react';

import styles from './win-loss-amount.module.scss';

const WinLossAmount = NiceModal.create(({ sportType, betType }: { sportType: number; betType: number }) => {
  const { t } = useTranslation();
  const modal = useModal();
  const theme = useTheme();

  const [isLoading, setIsLoading] = useState(true);
  const [winLossAmount, setWinLossAmount] = useState<CurrentWinLossAmount[]>([]);

  useEffect(() => {
    const getCustomerWinLossAmount = async () => {
      const firstCustId = params.custId ?? Number(params.custIds?.split(',')[0]);
      const currentWinLossAmount = await SettingService.GetCustomerCurrentWinLossAmount(
        firstCustId,
        sportType,
        betType
      );
      setWinLossAmount(currentWinLossAmount);
      setIsLoading(false);
    };

    getCustomerWinLossAmount();
  }, [betType, sportType]);

  const showWinningPercentColumn = (wl: CurrentWinLossAmount) => {
    const { winPercent, currentAmount, maxWinAmount } = wl;
    if (winPercent === 0) return <div>{winPercent}%</div>;
    if (winPercent > 0 && winPercent <= 100)
      return (
        <div>
          {winPercent}% ({currentAmount}/{maxWinAmount})
        </div>
      );
    if (winPercent > 100)
      return (
        <div className={styles.textRed}>
          {winPercent}% ({currentAmount}/{maxWinAmount})
        </div>
      );
    if (winPercent === -1) return <div>{t('unlimited')}</div>;
    if (winPercent === -2)
      return (
        <div>
          <span className={styles.textRed}>{t('N/A')}</span>
          <span>
            ({currentAmount}/<span className={styles.textRed}>{maxWinAmount}</span>)
          </span>
        </div>
      );
    if (winPercent === -3) return <div className={styles.notAvailable}>{t('N/A')}</div>;
  };

  const showLosingPercentColumn = (wl: CurrentWinLossAmount) => {
    const { lossPercent, currentAmount, maxLossAmount } = wl;
    if (lossPercent === 0) return <div>{lossPercent}%</div>;
    if (lossPercent === -1) return <div>{t('unlimited')}</div>;
    if (lossPercent > 0 && lossPercent <= 100)
      return (
        <div>
          {lossPercent}% ({-currentAmount}/{maxLossAmount})
        </div>
      );
    if (lossPercent > 100)
      return (
        <div className={styles.textRed}>
          {lossPercent}% ({-currentAmount}/{maxLossAmount})
        </div>
      );
  };

  return isLoading ? (
    <BlockUI />
  ) : (
    <div className={styles.overlay} data-theme={theme}>
      <div className={styles.wrapper}>
        <div className={styles.content}>
          <div className={styles.title}>
            {t(`CurrentWinLossAmount`)}
            <span
              onClick={() => {
                modal.resolve({
                  isCancel: true,
                  groupItems: [],
                });
                modal.remove();
              }}
              className={styles.close}
              aria-hidden
            >
              {IconClose}
            </span>
          </div>
          <div className={styles.body}>
            <div className={styles.contentBody}>
              <div className={styles.contentHead}>
                <BlockRow label={t('Product')}>
                  <BlockColumn>{t('Winning')}%</BlockColumn>
                  <BlockColumn>{t('Losing')}%</BlockColumn>
                </BlockRow>
              </div>
              {winLossAmount.length > 0 &&
                winLossAmount.map((wl) => {
                  return (
                    <BlockRow key={wl.sportType} label={wl.sportTypeName}>
                      <BlockColumn>
                        <div className={styles.textCenter}>{showWinningPercentColumn(wl)}</div>
                      </BlockColumn>
                      <BlockColumn>
                        <div className={styles.textCenter}>{showLosingPercentColumn(wl)}</div>
                      </BlockColumn>
                    </BlockRow>
                  );
                })}
            </div>
          </div>
          {sportType === SportTypeIds.LiveCasino && <div className={styles.footer}>{t('BBinSettingNote')}</div>}
        </div>
      </div>
    </div>
  );
});

export default WinLossAmount;
