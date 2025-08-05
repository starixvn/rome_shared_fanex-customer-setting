import { axiosInstance, getHeadersForPostRequest } from "@customer-web-app/shared-api-helper";
import { CashServiceAds } from "./cash-service-ads";
import { useState } from "react";

declare const homeUrl: any;
declare global {
  interface Window {
    indexManager: any;
  }
}
export function MainSitePopUpAds() {
    const [isOpen, setIsOpen] = useState(true);
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

    return <CashServiceAds
        isOpen={isOpen} 
        onClose={() => {
            setIsOpen(false);
        }}
        onConfirm={onConfirmEnableCashService}
        cashServiceStatus={{ isCashServiceEnabled: false, isCanUpdateCashServiceStatus: true, isOfferedCashService: true, isAllowCloseAds: true }}
        onDontShowAgain={() => { }}
    />;
}

