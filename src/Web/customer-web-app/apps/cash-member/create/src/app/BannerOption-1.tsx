import { useState } from "react";

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
            width: '50%',
            height: '100%',
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            justifyContent: 'center',
            padding: '48px 32px',
            background: 'linear-gradient(90deg,rgba(182, 218, 236, 0.56) 40%, rgba(180, 210, 232, 0) 60%)',
            boxSizing: 'border-box',
        }}>

            <div style={{
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
                justifyContent: 'center',
                color: '#0a2c4d',
                padding: '32px 16px',
                background: 'white',
                borderRadius: 16,
                boxShadow: '0 2px 8px rgba(0,0,0,0.08)',
                maxWidth: 480,
                margin: '0 auto',
            }}>
                <div style={{
                    fontWeight: 700,
                    fontSize: 24,
                    color: '#0a2c4d',
                    textAlign: 'center',
                    marginBottom: 16,
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
                    fontSize: 16,
                    color: '#222',
                    textAlign: 'center',
                    marginBottom: 16,
                    fontWeight: 400,
                    maxWidth: 400,
                }}>
                    Start your <span style={{ fontWeight: 700, color: '#03548d' }}>14-days free trial</span> with no transaction fees (excludes payment gateway transactions).
                </div>
                <div style={{
                    fontSize: 16,
                    color: '#222',
                    textAlign: 'center',
                    fontWeight: 400,
                }}>
                    After the trial:
                </div>
                <ul style={{
                    textAlign: 'left',
                    margin: 0,
                    padding: '0 0 0 24px',
                    color: '#222',
                    fontSize: 16,
                    maxWidth: 400,
                }}>
                    <li><span style={{ fontWeight: 700, color: '#03548d' }}>THB 200</span> per day</li>
                    <li><span style={{ fontWeight: 700, color: '#03548d' }}>THB 0.15</span> per transaction via your linked bank account and per promotion.</li>
                </ul>
                <div style={{ display: 'flex', gap: 16, justifyContent: 'center', width: '100%' , marginTop: 20}}>
                    <button
                        style={{
                            background: '#0a2c4d',
                            color: 'rgb(255, 255, 255)',
                            border: 'none',
                            borderRadius: 18,
                            padding: '16px 32px',
                            fontWeight: 600,
                            fontSize: 15,
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
                            padding: '16px 32px',
                            fontWeight: 600,
                            fontSize: 15,
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

interface BannerContentProps {
    readonly onClickActivate: () => void;
}

function BannerContent(props: BannerContentProps) {
    const { onClickActivate } = props;
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
                fontWeight: 900,
                fontSize: 52,
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
                fontWeight: 700,
                padding: 10,
                fontSize: 24,
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
                        padding: '16px 32px',
                        fontWeight: 600,
                        fontSize: 15,
                        cursor: 'pointer',
                        boxShadow: '0 2px 8px rgba(0, 0, 0, 0.2)',
                        transition: 'background 0.2s',
                        letterSpacing: '0.2px',
                    }}
                    onClick={onClickActivate}
                >
                    Activate now
                </button>
                <button
                    style={{
                        color: '#ffffff',
                        background: '#286fa1',
                        border: 'none',
                        borderRadius: 18,
                        padding: '16px 32px',
                        fontWeight: 600,
                        fontSize: 15,
                        cursor: 'pointer',
                        transition: 'background 0.2s',
                        letterSpacing: '0.2px',
                    }}
                    onClick={() => alert('Find out more clicked!')}
                >
                    Find out more
                </button>
            </div>
        </div>
    );
}

interface BannerProps {
    readonly onClose?: () => void;
}

function Banner(props: BannerProps) {
    const { onClose } = props;
    const [contentIndex, setContentIndex] = useState(0);

    return (
        <div style={{
            position: 'fixed',
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            backgroundColor: 'rgba(0, 0, 0, 0.7)',
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            zIndex: 1000,
            animation: 'fadeInBg 0.3s',
        }}>
            <style>{`
          @keyframes fadeInBg { from { opacity: 0; } to { opacity: 1; } }
          @keyframes fadeInPopup { from { transform: scale(0.95); opacity: 0; } to { transform: scale(1); opacity: 1; } }
        `}</style>
            <div style={{
                position: 'relative',
                maxWidth: 820,
                height: 550,
                margin: '20px',
                borderRadius: 20,
                overflow: 'hidden',
                boxShadow: '0 8px 32px rgba(0,0,0,0.18)',
                animation: 'fadeInPopup 0.35s',
                background: '#b4d2e8',
            }}>
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
                    src={'http://localhost:4211/public/BannerPC.png'}
                    alt="AdvCash"
                    style={{
                        width: '100%',
                        height: 'auto',
                        objectFit: 'cover',
                    }}
                />
                {contentIndex === 0 && <BannerContent onClickActivate={() => setContentIndex(1)} />}
                {contentIndex === 1 && <ActiveCashContent onConfirm={() => setContentIndex(2)} onCancel={() => setContentIndex(0)} />}
            </div>
        </div>
    );
}

export default Banner;