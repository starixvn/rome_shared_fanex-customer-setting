// eslint-disable-next-line @typescript-eslint/no-unused-vars
import { i18nInstance } from '@customer-web-app/shared-i18n';
import { CreateCashMemberPage } from '@customer-web-app/create-cash-member-page';
import { IconError } from '@customer-web-app/shared/base/common-icon';
import * as Sentry from '@sentry/react';
import { useEffect, useState } from 'react';
import Banner from './BannerOption-1';
import { CashMemberSettingService } from '@customer-web-app/shared-group-settings-services';
export function App() {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [error, setError] = useState(false);
  function getCookie(name: string) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop()!.split(';').shift();
    return undefined;
  }

  useEffect(() => {
    const cookieLang = getCookie('LANGUAGE');
    if (cookieLang) {
      i18nInstance.changeLanguage(cookieLang);
    }

    const checkCashServiceStatus = async () => {
      try {
        const isCashServiceActive = await CashMemberSettingService.GetCashServiceStatus();
        setIsModalOpen(!isCashServiceActive);
      } catch (error) {
        console.error('Error checking cash service status:', error);
        setError(true);
        Sentry.captureException(error);
      }
    };

    checkCashServiceStatus();
  }, []);

  return (
    <Sentry.ErrorBoundary fallback={IconError}>
      {error && <CreateCashMemberPage />}
      {isModalOpen && <Banner />}
    </Sentry.ErrorBoundary>
  );
}

export default App;
