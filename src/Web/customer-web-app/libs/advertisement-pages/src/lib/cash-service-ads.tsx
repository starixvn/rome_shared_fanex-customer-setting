import { currentCdnUri } from '@customer-web-app/shared-api-helper';
import { useMemo, useState } from 'react';
import styles from './cash-service-ads.module.scss';
import { CashServiceStatus } from '@customer-web-app/share-models';
import { useTranslation, Trans } from 'react-i18next';

interface CashServiceAdsContentProps {
    readonly onClickActivateNow: (() => void) | undefined;
    readonly findOutMore: () => void;
    readonly onDontShowAgain?: () => void;
}

function CashServiceAdsContent(props: CashServiceAdsContentProps) {
    const { onClickActivateNow, findOutMore, onDontShowAgain } = props;
    const { t } = useTranslation();
    return (
        <div className={styles['ads-content']}>
            <div className={styles['main-title']}>
                {t('ExclusivelyForYou')}
            </div>
            <div className={styles['sub-title']}>
                <Trans i18nKey="EnjoyCashServiceFreeFor14Days"
                    components={{
                        free: <span className={styles['pulse-animate'] + ' ' + styles['free-highlight']}></span>
                    }} />
            </div>
            <div className={styles['button-container']}>
                <button
                    className={styles['primary-button']}
                    onClick={onClickActivateNow}
                    disabled={!onClickActivateNow}
                >
                    {t('ActivateNow')}
                </button>
                <button
                    className={styles['secondary-button']}
                    onClick={findOutMore}
                >
                    {t('FindOutMore')}
                </button>
            </div>
            {onDontShowAgain &&
                    <div className={styles['dont-show-again']} onClick={onDontShowAgain}>
                        <input type="checkbox" id="dont-show-again" />
                        <label htmlFor="dont-show-again">{t('DontShowAgain')}</label>
                    </div>}
        </div>
    );
}

interface CashServiceAdsProps {
    readonly onDontShowAgain?: () => void;
    readonly onConfirm?: () => void;
    readonly onClose?: () => void;
    readonly cashServiceStatus: CashServiceStatus;
}

enum ContentFrame {
    Ads,
    TourGuide
}

export function CashServiceAds(props: CashServiceAdsProps) {
    const { t } = useTranslation();
    const { onClose, onConfirm, cashServiceStatus, onDontShowAgain } = props;
    const [isOpen, setIsOpen] = useState(true);
    const [contentFrame, setContentFrame] = useState(ContentFrame.Ads);
    const imagePath = `${currentCdnUri()}advertisement/cash-service-ads/`;
    const bannerPath = useMemo(() => `${imagePath}BannerPC.jpg`, []);
    const [currentStep, setCurrentStep] = useState(0);
    const steps = useMemo(() => [
        <>
            <div className={styles['step-title']}>{t('InstantDepositsAndWithdrawals')}</div>
            <img key={1} className={styles['step-image']} src={`${imagePath}step1.jpg`} alt={t('InstantDepositsAndWithdrawals')} />
            <div className={styles['step-description']}>{t('EasilyAddAPaymentGatewayAndBankAccountsSoYourMembersCanStartPlayingInstantly')}</div>
        </>,
        <>
            <div className={styles['step-title']}>{t('ManageCreditAndCashMember')}</div>
            <img key={2} className={styles['step-image']} src={`${imagePath}step2.jpg`} alt={t('ManageCreditAndCashMember')} />
            <div className={styles['step-description']}>{t('MaximiseYourEarningsByManagingBothCreditAndCashInSingleAccount')}</div>
        </>,
        <>
            <div className={styles['step-title']}>{t('DedicatedDomain')}</div>
            <img key={3} className={styles['step-image']} src={`${imagePath}step3.gif`} alt={t('DedicatedDomain')} />
            <div className={styles['step-description']}>{t('ElevateYourBrandWithASignUpLinkedForMemberRegistrations')}</div>
        </>,
        <>
            <div className={styles['step-title']}>{t('CustomisePromotions')}</div>
            <img key={4} className={styles['step-image']} src={`${imagePath}step4.jpg`} alt={t('CustomisePromotions')} />
            <div className={styles['step-description']}>{t('CreateAndManagePromotionsFeaturingPopularProductsAndProvidersToKeepYourMembersEntertained')}</div>
        </>,
        <>
            <div className={styles['step-title']}>{t('SuperchargeYourCashFlow')}</div>
            <div className={styles.divider} />
            <div className={styles['step-highlight-block']}>
                <span className={styles['free-badge']}>{
                    <Trans i18nKey="14-daysFreeTrial" components={{ b: <b></b> }} />
                }</span>
                <span className={styles['save-amount']}>{
                    <Trans i18nKey="StartTodayAndEnjoyInstantSavingsOf" components={{ b: <b></b> }} />
                }</span>
            </div>
            <ul className={styles['step-benefits']}>
                <li><span className={styles['icon-check']} /> <b>{t('NoPlatformFee')}</b> <span>- {t('SaveTHB200DayDuringTheTrial')}</span></li>
                <li><span className={styles['icon-check']} /> <b> {t('Just015PerTransactionViaYourLinkedBankAccount')}</b></li>
                <li><span className={styles['icon-check']} /> {t('GetResultsStayIfYouLoveItCancelAnytime')}</li>
            </ul>
            <div className={styles['step-note']}>
                <span className={styles['icon-info']}>ℹ️</span> {t('DontMissOutActivateNowAndTakeYourCashFlowToTheNextLevel')}
            </div>
        </>
    ], [t]);

    const onActivateNow = () => {
        setContentFrame(ContentFrame.TourGuide);
        setCurrentStep(4);
    }

    const onFindOutMore = () => {
        setContentFrame(ContentFrame.TourGuide);
        setCurrentStep(0);
    }

    if (!isOpen) return null;

    return (
        <div className={styles.overlay}>
            <div className={styles.popup}>
                {onClose && <button
                    onClick={() => {
                        setIsOpen(false);
                        onClose();
                    }}
                    className={styles['close-button']}
                >
                    ×
                </button>}
                <img
                    src={bannerPath}
                    alt="AdvCash"
                    className={styles['banner-image']}
                />
                {contentFrame === ContentFrame.Ads &&
                    <CashServiceAdsContent
                        onClickActivateNow={cashServiceStatus.isCanUpdateCashServiceStatus ? onActivateNow : undefined}
                        findOutMore={onFindOutMore}
                        onDontShowAgain={onDontShowAgain}
                    />}
                {contentFrame === ContentFrame.TourGuide
                    && <StepSlider
                        steps={steps}
                        currentStep={currentStep}
                        onClose={() => setContentFrame(ContentFrame.Ads)}
                        onFinish={cashServiceStatus.isCanUpdateCashServiceStatus ? onConfirm : undefined} />}

                
            </div>
        </div>
    );
}

interface StepSliderProps {
    steps: React.ReactNode[];
    currentStep: number;
    onClose?: () => void;
    onFinish?: () => void;
}

const StepSlider: React.FC<StepSliderProps> = (props) => {
    const { steps, currentStep, onClose, onFinish } = props;
    const [current, setCurrent] = useState(currentStep || 0);
    const { t } = useTranslation();

    const isLast = current === steps.length - 1;
    const isFirst = current === 0;

    return (
        <div className={styles['step-slider']}>
            {onClose && (
                <button
                    onClick={onClose}
                    className={styles['close-button']}
                >×</button>
            )}

            <div className={styles['step-content']}>{steps[current]}</div>

            <div className={styles['step-navigation']}>
                <button
                    onClick={() => setCurrent(c => c - 1)}
                    disabled={isFirst}
                    className={`${styles['step-button']} ${styles['back-button']}`}
                >{t('Back')}</button>
                {!isLast ? (
                    <button
                        onClick={() => setCurrent(c => c + 1)}
                        className={`${styles['step-button']} ${styles['next-button']}`}
                    >{t('Next')}</button>
                ) : (
                    <button
                        onClick={onFinish}
                        disabled={!onFinish}
                        className={`${styles['step-button']} ${styles['next-button']}`}
                    >{t('Confirm')}</button>
                )}
            </div>
            <div className={styles['step-counter']}>
                {current + 1} of {steps.length}
            </div>
        </div>
    );
};
