import { Title } from "@customer-web-app/shared/base/components";
import { useTranslation } from "react-i18next";

export function EditInfoTitle(){
    const { t } = useTranslation('global');
    let popup = window.parent.document.querySelector('#product-popup');
    if (!popup) {
        popup = window?.top?.document.querySelector('div[id^="page-popup-"]') as HTMLElement;
    }

    const isOpenInPopup = popup !== null;
    if (isOpenInPopup) {
        return null;
    }
    return (
        <Title>
            {t('InfoUsername', { username: updateInfoCustomerPageConfig.username.toLocaleUpperCase() })}
        </Title>
    )
}