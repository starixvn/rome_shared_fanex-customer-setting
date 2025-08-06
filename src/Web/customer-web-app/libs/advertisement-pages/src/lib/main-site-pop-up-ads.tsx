import { axiosInstance, getHeadersForPostRequest } from "@customer-web-app/shared-api-helper";
import { CashServiceAds } from "./cash-service-ads";
import { useState, useEffect } from "react";

declare const appConfig: {
    isCanUpdateCashServiceStatus: boolean;
    isCanEditCashService: boolean;
    isOfferedCashService: boolean;
    isCashServiceEnabled: boolean;
    nickName: string;
};

declare const homeUrl: string;
declare global {
    interface Window {
        indexManager: any;
    }
}
export function MainSitePopUpAds() {
    const [isOpen, setIsOpen] = useState(true);
    const [shouldShowPopup, setShouldShowPopup] = useState(false);

    useEffect(() => {
        const hideAds = localStorage.getItem(`${appConfig.nickName}-hideCashServiceAds`);
        if (hideAds !== 'true') {
            setShouldShowPopup(true);
        }
    }, []);

    const onConfirmEnableCashService = async () => {
        const response = await axiosInstance.post(
            `/main/Dashboard/UpdateCashServiceStatus?IsEnableCash=true`, {}, {
            headers: getHeadersForPostRequest(),
        }
        );
        if (response.data.isSuccessful) {
            window.top && window.top.indexManager.showHideCashServiceChecklist(true);
            const mainFrame = document.getElementById('main') as HTMLIFrameElement;
            mainFrame.src = homeUrl;
            setIsOpen(false);
        }
        else {
            alert(response.data.message);
        }
    }

    const closePopup = () => {
        setIsOpen(false);
    }

    const checkDontShowAgain = () => {
        if (appConfig.nickName) {
            localStorage.setItem(`${appConfig.nickName}-hideCashServiceAds`, 'true');
        }
        closePopup();
    }

    if (!shouldShowPopup) {
        return null;
    }

    return <CashServiceAds
        isOpen={isOpen}
        onClose={() => {
            setIsOpen(false);
        }}
        onConfirm={onConfirmEnableCashService}
        cashServiceStatus={{
            isCashServiceEnabled: appConfig.isCashServiceEnabled,
            isCanUpdateCashServiceStatus: appConfig.isCanUpdateCashServiceStatus,
            isOfferedCashService: appConfig.isOfferedCashService,
            isAllowCloseAds: true
        }}
        onDontShowAgain={checkDontShowAgain}
    />;
}

