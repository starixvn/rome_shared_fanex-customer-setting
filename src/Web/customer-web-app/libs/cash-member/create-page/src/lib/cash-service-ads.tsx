import { currentCdnUri } from '@customer-web-app/shared-api-helper';
import { useMemo, useState } from 'react';
import styles from './cash-service-ads.module.scss';

interface ActiveCashContentProps {
    readonly onConfirm: () => void;
    readonly onCancel: () => void;
}

function ActiveCashContent(props: ActiveCashContentProps) {
    const { onConfirm, onCancel } = props;
    return (
        <div className={styles.activeCashContent}>
            <div className={styles.contentBox}>
                <div className={styles.title}>
                    Ready to unlock faster cash flow?
                </div>
                <div className={styles.divider} />
                <div className={styles.description}>
                    <span className={styles.highlight}>14-days free trial</span> – save THB 2,800.
                </div>
                <div className={styles.description}>
                    No platform fee (THB 200/day).
                    <br />
                    Only THB 0.15 per transaction and promotion.
                    <br />
                    Don’t miss this chance to elevate your cash flow. 
                    <br />
                    The sooner you start, the sooner you profit.
                </div>
                <div className={styles.buttonContainer}>
                    <button
                        className={styles.primaryButton}
                        onClick={onConfirm}
                    >
                        Confirm
                    </button>
                    <button
                        className={styles.secondaryButton}
                        onClick={onCancel}
                    >
                        Cancel
                    </button>
                </div>
            </div>
        </div>
    );
}

interface CashServiceAdsContentProps {
    readonly onClickActivateNow: () => void;
    readonly findOutMore: () => void;
}

function CashServiceAdsContent(props: CashServiceAdsContentProps) {
    const { onClickActivateNow, findOutMore } = props;
    return (
        <div className={styles.adsContent}>
            <div className={styles.mainTitle}>
                EXCLUSIVELY FOR YOU
            </div>
            <div className={styles.subTitle}>
                Enjoy Cash Service <span className={styles.pulseAnimate + ' ' + styles.freeHighlight}>FREE</span> for 14 days!
            </div>
            <div className={styles.buttonContainer}>
                <button
                    className={styles.primaryButton}
                    onClick={onClickActivateNow}
                >
                    Activate now
                </button>
                <button
                    className={styles.secondaryButton}
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
}

function CashServiceAds(props: CashServiceAdsProps) {
    const { onClose, onConfirm } = props;
    const [contentIndex, setContentIndex] = useState(0);
    const bannerPath = useMemo(() => currentCdnUri() + '/banner/BannerPC.png', []);

    return (
        <div className={styles.overlay}>
            <div className={styles.popup}>
                {onClose && <button
                    onClick={onClose}
                    className={styles.closeButton}
                >
                    ×
                </button>}
                <img
                    src={bannerPath}
                    alt="AdvCash"
                    className={styles.bannerImage}
                />
                {contentIndex === 0 && <CashServiceAdsContent onClickActivateNow={() => setContentIndex(1)} findOutMore={() => alert('find out more')} />}
                {contentIndex === 1 && <ActiveCashContent onConfirm={onConfirm} onCancel={() => setContentIndex(0)} />}
                {contentIndex === 2 && <StepSlider steps={[
                    <>
                        <div className={styles.description}>Instant deposits and withdrawals!</div>
                        <img src="http://localhost:4211/public/cash-service-ads/step1.png" className={styles.bannerImage} />
                    </>,
                    <>
                        <div className={styles.description}>Manage credit and cash member</div>
                        <img src="http://localhost:4211/public/cash-service-ads/step2.png" className={styles.bannerImage} />
                    </>,
                    <>
                        <div className={styles.description}>Dedicated domain</div>
                        <img src="http://localhost:4211/public/cash-service-ads/step3.gif" className={styles.bannerImage} />
                    </>,
                    <>
                        <div className={styles.description}>Customise promotions</div>
                        <img src="http://localhost:4211/public/cash-service-ads/step4.png" className={styles.bannerImage} />
                    </>,
                    <>
                        <div className={styles.description}>Try Cash Service for free!</div>
                    </>,
                ]} onClose={() => setContentIndex(0)} onFinish={onConfirm} />}
            </div>
        </div>
    );
}

interface StepSliderProps {
    steps: React.ReactNode[];
    onClose?: () => void;
    onFinish?: () => void;
}

const StepSlider: React.FC<StepSliderProps> = ({ steps, onClose, onFinish }) => {
    const [current, setCurrent] = useState(0);

    const isLast = current === steps.length - 1;
    const isFirst = current === 0;

    return (
        <div className={styles.stepSlider}>
            {onClose && (
                <button
                    onClick={onClose}
                    className={styles.closeButton}
                >×</button>
            )}

            <div className={styles.stepContent}>{steps[current]}</div>

            <div className={styles.stepNavigation}>
                <button
                    onClick={() => setCurrent(c => c - 1)}
                    disabled={isFirst}
                    className={`${styles.stepButton} ${styles.backButton}`}
                >Back</button>
                {!isLast ? (
                    <button
                        onClick={() => setCurrent(c => c + 1)}
                        className={`${styles.stepButton} ${styles.nextButton}`}
                    >Next</button>
                ) : (
                    <button
                        onClick={onFinish}
                        className={`${styles.stepButton} ${styles.nextButton}`}
                    >Finish</button>
                )}
            </div>
            <div className={styles.stepCounter}>
                {current + 1} of {steps.length}
            </div>
        </div>
    );
};

export default CashServiceAds;