import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { BlockUI } from '@customer-web-app/shared-block-ui';
import { SportTypeIds } from '@customer-web-app/shared-constants';
import { SettingService } from '@customer-web-app/shared-group-settings-services';
import { CurrentWinAmount } from '@customer-web-app/shared-group-settings-types';
import { NumberHelper } from '@customer-web-app/shared-helpers';
import { BlockColumn, BlockRow } from '@customer-web-app/shared/base/block';
import { IconClose } from '@customer-web-app/shared/base/common-icon';
import { useTheme } from '@customer-web-app/shared/base/layout';
import NiceModal, { useModal } from '@ebay/nice-modal-react';

import styles from './win-amount.module.scss';

const WinAmount = NiceModal.create(({ sportType, betType }: { sportType: number; betType: number }) => {
  const { t } = useTranslation();
  const modal = useModal();
  const theme = useTheme();

  const [isLoading, setIsLoading] = useState(true);
  const [winAmount, setWinAmount] = useState<CurrentWinAmount[]>([]);

  useEffect(() => {
    const getCustomerWinAmount = async () => {
      const firstCustId = params.custId ?? Number(params.custIds?.split(',')[0]);
      const currentWinAmount = await SettingService.GetCurrentWinAmount(
        firstCustId,
        sportType,
        betType
      );
      setWinAmount(currentWinAmount);
      setIsLoading(false);
    };

    getCustomerWinAmount();
  }, [betType, sportType]);

  const showWinningPercentColumn = (w: CurrentWinAmount) => {
    const { winPercent, maxWinAmount, currentAmount } = w;
    const maxWinAmountValue = NumberHelper.formatWithCommas(maxWinAmount);
    const winPercentValue = NumberHelper.formatWithCommas(winPercent);
    const currentAmountValue = NumberHelper.formatWithCommas(currentAmount);

    if (winPercent === 0) return <div>{winPercent}%</div>;
    if (winPercent > 0 && winPercent <= 100)
      return (
        <div>
          {winPercentValue}% ({currentAmountValue}/{maxWinAmountValue})
        </div>
      );
    if (winPercent > 100)
      return (
        <div className={styles.textRed}>
          {winPercentValue}% ({currentAmountValue}/{maxWinAmountValue})
        </div>
      );
    if (winPercent === -1) return <div>{t('unlimited')}</div>;
    if (winPercent === -2)
      return (
        <div>
          <span className={styles.textRed}>{t('N/A')}</span>
          <span>
            ({currentAmountValue}/<span className={styles.textRed}>{maxWinAmountValue}</span>)
          </span>
        </div>
      );
    if (winPercent === -3) return <div className={styles.notAvailable}>{t('N/A')}</div>;
  };

  return isLoading ? (
    <BlockUI />
  ) : (
    <div className={styles.overlay} data-theme={theme}>
      <div className={styles.wrapper}>
        <div className={styles.content}>
          <div className={styles.title}>
            {t(`CurrentWinAmount`)}
            <span
              onClick={() => {
                modal.resolve({
                  isCancel: true,
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
                </BlockRow>
              </div>
              {winAmount.length > 0 &&
                winAmount.map((w) => {
                  return (
                    <BlockRow key={w.sportType} label={w.sportTypeName}>
                      <BlockColumn>
                        <div className={styles.textCenter}>{showWinningPercentColumn(w)}</div>
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

export default WinAmount;
