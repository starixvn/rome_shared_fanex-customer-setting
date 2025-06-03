// eslint-disable-next-line @typescript-eslint/no-unused-vars
import { i18nInstance } from '@customer-web-app/shared-i18n';
import { CreateCashMemberPageWrapper } from '@customer-web-app/create-cash-member-page';
import { IconError } from '@customer-web-app/shared/base/common-icon';
import * as Sentry from '@sentry/react';
import { useEffect } from 'react';

export function App() {
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

  }, []);

  return (
    <Sentry.ErrorBoundary fallback={IconError}>
      <CreateCashMemberPageWrapper />
    </Sentry.ErrorBoundary>
  );
}

export default App;
