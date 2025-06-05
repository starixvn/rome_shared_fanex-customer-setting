import { currentCdnUri } from '@customer-web-app/shared-api-helper';
import { useMemo, useState } from 'react';

interface ActiveCashContentProps {
    readonly onConfirm: () => void;
    readonly onCancel: () => void;
}

function ActiveCashContent(props: ActiveCashContentProps) {
    const { onConfirm, onCancel } = props;
    return (
        <div style={{
            position: 'absolute',
            top: 0,
            left: 0,
            width: '58%',
            height: '100%',
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            justifyContent: 'center',
            padding: '20px 32px',
            background: 'linear-gradient(90deg,rgba(182, 218, 236, 0.56) 40%, rgba(180, 210, 232, 0) 60%)',
            boxSizing: 'border-box',
        }} className="cash-service-popup-content">
            <div style={{
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
                justifyContent: 'center',
                color: '#0a2c4d',
                padding: '12px 16px',
                background: 'white',
                borderRadius: 16,
                boxShadow: '0 2px 8px rgba(0,0,0,0.08)',
                maxWidth: 480,
                margin: '0 auto',
            }}>
                <div style={{
                    fontWeight: 600,
                    fontSize: 20,
                    color: '#0a2c4d',
                    textAlign: 'center',
                    marginBottom: 14,
                }}>
                    Are you sure you want to activate Cash Service?
                </div>
                <div style={{
                    width: 60,
                    height: 2,
                    background: '#0a2c4d',
                    margin: '0 auto 20px auto',
                    borderRadius: 2,
                }} />
                <div style={{
                    fontSize: 14,
                    color: '#222',
                    textAlign: 'center',
                    marginBottom: 14,
                    fontWeight: 400,
                    maxWidth: 400,
                }}>
                    Start your <span style={{ fontWeight: 600, color: '#03548d' }}>14-days free trial</span> with no transaction fees (excludes payment gateway transactions).
                </div>
                <div style={{
                    fontSize: 14,
                    color: '#222',
                    textAlign: 'center',
                    fontWeight: 400,
                }}>
                    After the trial:
                </div>
                <ul style={{
                    textAlign: 'left',
                    margin: 0,
                    color: '#222',
                    fontSize: 14,
                    maxWidth: 400,
                }}>
                    <li><span style={{ fontWeight: 600, color: '#03548d' }}>THB 200</span> per day</li>
                    <li><span style={{ fontWeight: 600, color: '#03548d' }}>THB 0.15</span> per transaction via your linked bank account and per promotion.</li>
                </ul>
                <div style={{ display: 'flex', gap: 16, justifyContent: 'center', width: '100%', marginTop: 20 }}>
                    <button
                        style={{
                            background: '#0a2c4d',
                            color: 'rgb(255, 255, 255)',
                            border: 'none',
                            borderRadius: 18,
                            padding: '10px 18px',
                            fontWeight: 500,
                            fontSize: 13,
                            cursor: 'pointer',
                            boxShadow: '0 2px 8px rgba(0, 0, 0, 0.2)',
                            transition: 'background 0.2s',
                            letterSpacing: '0.2px',
                        }}
                        onClick={onConfirm}
                    >
                        Confirm
                    </button>
                    <button
                        style={{
                            color: '#ffffff',
                            background: '#286fa1',
                            border: 'none',
                            borderRadius: 18,
                            padding: '10px 18px',
                            fontWeight: 500,
                            fontSize: 13,
                            cursor: 'pointer',
                            transition: 'background 0.2s',
                            letterSpacing: '0.2px',
                        }}
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
        <div style={{
            position: 'absolute',
            top: 0,
            left: 0,
            width: '50%',
            height: '100%',
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            justifyContent: 'center',
            background: 'linear-gradient(90deg,rgba(182, 218, 236, 0.56) 40%, rgba(180, 210, 232, 0) 60%)',
            boxSizing: 'border-box',
        }}>
            <div style={{
                fontWeight: 800,
                fontSize: 38,
                color: '#03548d',
                lineHeight: 1.2,
                textAlign: 'center',
                maxWidth: 350,
                letterSpacing: '0.2px',
                textTransform: 'uppercase',
            }}>
                EXCLUSIVELY FOR YOU
            </div>
            <style>{`
             @keyframes pulse {
    0% { transform: scale(0.95); }
    50% { transform: scale(1.1); }
    100% { transform: scale(0.95); }
  }
  .pulse-animate {
    animation: pulse 1.2s infinite;
    display: inline-block;
  }
            `}</style>
            <div style={{
                fontWeight: 600,
                padding: '0 30px',
                fontSize: 20,
                color: '#03548d',
                lineHeight: 1.2,
                textAlign: 'center',
                margin: 10,
                maxWidth: 250,
                letterSpacing: '0.1px',
                borderRadius: 10,
            }}>
                Enjoy Cash Service <span className="pulse-animate" style={{ color: '#ff9800', fontWeight: 900, animation: 'pulse 1.2s infinite' }}>FREE</span> for 14 days!
            </div>
            <div style={{ display: 'flex', gap: 16, justifyContent: 'center', width: '100%' }}>
                <button
                    style={{
                        background: '#0a2c4d',
                        color: 'rgb(255, 255, 255)',
                        border: 'none',
                        borderRadius: 18,
                        padding: '10px 18px',
                        fontWeight: 500,
                        fontSize: 13,
                        cursor: 'pointer',
                        boxShadow: '0 2px 8px rgba(0, 0, 0, 0.2)',
                        transition: 'background 0.2s',
                        letterSpacing: '0.2px',
                    }}
                    onClick={onClickActivateNow}
                >
                    Activate now
                </button>
                <button
                    style={{
                        color: '#ffffff',
                        background: '#286fa1',
                        border: 'none',
                        borderRadius: 18,
                        padding: '10px 18px',
                        fontWeight: 500,
                        fontSize: 13,
                        cursor: 'pointer',
                        transition: 'background 0.2s',
                        letterSpacing: '0.2px',
                    }}
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
        <div style={{
            position: 'fixed',
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            backgroundColor: 'rgba(0, 0, 0, 0.2)',
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            zIndex: 1000,
            animation: 'fadeInBg 0.3s',
        }}>
            <style>{`
          @keyframes fadeInBg { from { opacity: 0; } to { opacity: 1; } }
          @keyframes fadeInPopup { from { transform: scale(0.95); opacity: 0; } to { transform: scale(1); opacity: 1; } }
          @media (min-width: 768px) {
            .cash-service-popup {
              top: 45px;
            }
          }
        `}</style>
            <div style={{
                position: 'absolute',
                maxWidth: 620,
                height: 'auto',
                margin: '20px',
                borderRadius: 20,
                overflow: 'hidden',
                boxShadow: '0 8px 32px rgba(0,0,0,0.18)',
                animation: 'fadeInPopup 0.35s',
                background: '#b4d2e8',
            }} className="cash-service-popup">
                {onClose && <button
                    onClick={onClose}
                    style={{
                        position: 'absolute',
                        top: 16,
                        right: 16,
                        background: 'rgba(255, 255, 255, 0.2)',
                        border: 'none',
                        borderRadius: '50%',
                        width: 32,
                        height: 32,
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        cursor: 'pointer',
                        zIndex: 1,
                        color: 'white',
                        fontSize: 20,
                        fontWeight: 'bold',
                        transition: 'background 0.2s',
                    }}
                >
                    ×
                </button>}
                <img
                    src={bannerPath}
                    alt="AdvCash"
                    style={{
                        width: '100%',
                        height: 'auto',
                        objectFit: 'cover',
                    }}
                />
                {contentIndex === 0 && <CashServiceAdsContent onClickActivateNow={() => setContentIndex(1)} findOutMore={()=>alert('find out more')} />}
                {contentIndex === 1 && <ActiveCashContent onConfirm={onConfirm} onCancel={() => setContentIndex(0)} />}
                {contentIndex === 2 && <StepSlider steps={[
                      <>
                      <div style={{ fontSize: 20, fontWeight: 400, color: '#03548d', textAlign: 'center' }}>Instant deposits and withdrawals!</div>
                      <img src="http://localhost:4211/public/cash-service-ads/step1.png" style={{ width: '100%', height: 'auto', objectFit: 'cover' }}/>
                      </>,
                      <>
                       <div style={{ fontSize: 20, fontWeight: 400, color: '#03548d', textAlign: 'center' }}>Manage credit and cash member</div>
                       <img src="http://localhost:4211/public/cash-service-ads/step2.png" style={{ width: '100%', height: 'auto', objectFit: 'cover' }}/>
                      </>,
                      <>
                       <div style={{ fontSize: 20, fontWeight: 400, color: '#03548d', textAlign: 'center' }}>Dedicated domain</div>
                       <img src="http://localhost:4211/public/cash-service-ads/step3.gif" style={{ width: '100%', height: 'auto', objectFit: 'cover' }}/>
                      </>,
                      <>
                       <div style={{ fontSize: 20, fontWeight: 400, color: '#03548d', textAlign: 'center' }}>Customise promotions</div>
                       <img src="http://localhost:4211/public/cash-service-ads/step4.png" style={{ width: '100%', height: 'auto', objectFit: 'cover' }}/>
                      </>,
                      <>
                      <div style={{ fontSize: 20, fontWeight: 400, color: '#03548d', textAlign: 'center' }}>Try Cash Service for free!</div>
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
        <div style={{
            position: 'absolute',
            top: 0,
            left: 0,
            width: '100%',
            height: '100%',
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            justifyContent: 'center',
            background: 'white',
            boxSizing: 'border-box',
        }}>
        {onClose && (
          <button
            onClick={onClose}
            style={{
              position: 'absolute', top: 16, right: 16, background: 'none', border: 'none', fontSize: 20, cursor: 'pointer'
            }}
          >×</button>
        )}
  
        <div style={{ minHeight: 220 }}>{steps[current]}</div>
  
       
        <div style={{ display: 'flex', justifyContent: 'center', gap: 12 , marginTop: 20 , marginBottom: 10}}>
          <button
            onClick={() => setCurrent(c => c - 1)}
            disabled={isFirst}
            style={{
              padding: '8px 18px',
              borderRadius: 18,
              border: 'none',
              background: isFirst ? '#eee' : '#286fa1',
              color: '#fff',
              cursor: isFirst ? 'not-allowed' : 'pointer'
            }}
          >Back</button>
          {!isLast ? (
            <button
              onClick={() => setCurrent(c => c + 1)}
              style={{
                padding: '8px 18px',
                borderRadius: 18,
                border: 'none',
                background: '#0a2c4d',
                color: '#fff',
                cursor: 'pointer'
              }}
            >Next</button>
          ) : (
            <button
              onClick={onFinish}
              style={{
                padding: '8px 18px',
                borderRadius: 18,
                border: 'none',
                background: '#0a2c4d',
                color: '#fff',
                cursor: 'pointer'
              }}
            >Finish</button>
          )}
        </div>
        <div style={{ color: '#9a9a9a', fontWeight: 500 }}>
          {current + 1} of {steps.length}
        </div>
      </div>
    );
  };

export default CashServiceAds;