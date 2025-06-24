import { currentCdnUri } from '@customer-web-app/shared-api-helper';
import { useMemo, useState } from 'react';
import styles from './cash-service-ads.module.scss';
import { CashServiceStatus } from '@customer-web-app/share-models';

interface CashServiceAdsContentProps {
    readonly onClickActivateNow: (() => void) | undefined;
    readonly findOutMore: () => void;
}

function CashServiceAdsContent(props: CashServiceAdsContentProps) {
    const { onClickActivateNow, findOutMore } = props;
    return (
        <div className={styles['ads-content']}>
            <div className={styles['main-title']}>
                EXCLUSIVELY FOR YOU
            </div>
            <div className={styles['sub-title']}>
                Enjoy Cash Service <span className={styles['pulse-animate'] + ' ' + styles['free-highlight']}>FREE</span> for 14 days!
            </div>
            <div className={styles['button-container']}>
                <button
                    className={styles['primary-button']}
                    onClick={onClickActivateNow}
                    disabled={!onClickActivateNow}
                >
                    Activate now
                </button>
                <button
                    className={styles['secondary-button']}
                    onClick={findOutMore}
                >
                    Find out more
                </button>
            </div>
        </div>
    );
}

interface CashServiceAdsProps {
    readonly onConfirm: () => void;
    readonly onClose?: () => void;
    readonly cashServiceStatus: CashServiceStatus;
}

enum ContentFrame {
    Ads,
    StepSlider
}

function CashServiceAds(props: CashServiceAdsProps) {
    const { onClose, onConfirm, cashServiceStatus } = props;
    const [contentFrame, setContentFrame] = useState(ContentFrame.Ads);
    const bannerPath = useMemo(() => currentCdnUri() + '/cash-service-ads/BannerPC.png', []);
    const [currentStep, setCurrentStep] = useState(0);
    const steps = useMemo(() => [
        <>
            <div className={styles['step-title']}>Instant deposits and withdrawals!</div>
            <img key={1} className={styles['step-image']} src={currentCdnUri() + '/cash-service-ads/step1.png'} alt="Instant deposits and withdrawals!" />
            <div className={styles['step-description']}>Easily add a <strong>payment gateway</strong> and <strong>bank accounts</strong> so your members can start playing instantly.</div>
        </>,
        <>
            <div className={styles['step-title']}>Manage credit and cash member</div>
            <img key={2} className={styles['step-image']} src={currentCdnUri() + '/cash-service-ads/step2.jpg'} alt="Manage credit and cash member" />
            <div className={styles['step-description']}>Maximise your earnings by managing both credit and cash in single account.</div>
        </>,
        <>
            <div className={styles['step-title']}>Dedicated domain</div>
            <img key={3} className={styles['step-image']} src={currentCdnUri() + '/cash-service-ads/step3.gif'} alt="Dedicated domain" />
            <div className={styles['step-description']}>Elevate your brand with a unique sign-up linked for member registrations.</div>
        </>,
        <>
            <div className={styles['step-title']}>Customise promotions</div>
            <img key={4} className={styles['step-image']} src={currentCdnUri() + '/cash-service-ads/step4.png'} alt="Customise promotions" />
            <div className={styles['step-description']}>Create and manage promotions featuring popular products <br /> and providers to keep your members entertained.</div>
        </>,
        <>
            <div className={styles['step-title']}>Supercharge Your Cash Flow!</div>
            <div className={styles.divider} />
            <div className={styles['step-highlight-block']}>
                <span className={styles['free-badge']}>14-days <b>FREE</b> trial</span>
                <span className={styles['save-amount']}>Start today and enjoy instant savings of <b>THB 2,800</b></span>
            </div>
            <ul className={styles['step-benefits']}>
                <li><span className={styles['icon-check']} /> <b>No platform fee</b> <span>- save THB 200/day during the trial.</span></li>
                <li><span className={styles['icon-check']} /> <b> Just 0.15 per transaction</b> via your linked bank account.</li>
                <li><span className={styles['icon-check']} /> Get results. Stay if you love it. Cancel anytime.</li>
            </ul>
            <div className={styles['step-note']}>
                <span className={styles['icon-info']}>ℹ️</span> Don't miss out – Activate now and take your cash flow to the next level.
            </div>
        </>
    ], []);

    const onActivateNow = () => {
        setContentFrame(ContentFrame.StepSlider);
        setCurrentStep(4);
    }

    const onFindOutMore = () => {
        setContentFrame(ContentFrame.StepSlider);
        setCurrentStep(0);
    }

    return (
        <div className={styles.overlay}>
            <div className={styles.popup}>
                {onClose && <button
                    onClick={onClose}
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
                        findOutMore={onFindOutMore} />}
                {contentFrame === ContentFrame.StepSlider
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
                >Back</button>
                {!isLast ? (
                    <button
                        onClick={() => setCurrent(c => c + 1)}
                        className={`${styles['step-button']} ${styles['next-button']}`}
                    >Next</button>
                ) : (
                    <button
                        onClick={onFinish}
                        disabled={!onFinish}
                        className={`${styles['step-button']} ${styles['next-button']}`}
                    >Confirm</button>
                )}
            </div>
            <div className={styles['step-counter']}>
                {current + 1} of {steps.length}
            </div>
        </div>
    );
};

export default CashServiceAds;