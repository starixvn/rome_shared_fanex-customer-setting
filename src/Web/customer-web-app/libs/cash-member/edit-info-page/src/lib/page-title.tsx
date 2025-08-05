import { Title } from "@customer-web-app/shared/base/components";
import { useTranslation } from "react-i18next";

export function PageTitle(){
    const { t } = useTranslation();
    let popup = window.parent.document.querySelector('#product-popup');
    if (!popup) {
        popup = window?.top?.document.querySelector('div[id^="page-popup-"]') as HTMLElement;
    }
    if(!popup){
        popup = window.parent.document.querySelector('.ui-dialog-title') as HTMLElement;
    }

    const isOpenInPopup = popup !== null;
    if (isOpenInPopup) {
        return null;
    }
    return (
        <Title>
            {t('Info')}
            <span style={{ marginLeft: '5px' }}>{updateInfoCustomerPageConfig.username}</span>
        </Title>
    )
}