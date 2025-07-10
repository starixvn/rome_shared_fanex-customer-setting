import { useEffect } from 'react';

export function useCookieLanguage(i18nInstance: any, cookieName = 'LANGUAGE') {
    function getCookie(name: string) {
        const value = `; ${document.cookie}`;
        const parts = value.split(`; ${name}=`);
        if (parts.length === 2) return parts.pop()!.split(';').shift();
        return undefined;
    }
    useEffect(() => {
        const cookieLang = getCookie(cookieName);
        if (cookieLang) {
            i18nInstance.changeLanguage(cookieLang);
        }
    }, [cookieName]);
}