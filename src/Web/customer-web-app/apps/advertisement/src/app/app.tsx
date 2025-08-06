// eslint-disable-next-line @typescript-eslint/no-unused-vars
import { i18nInstance } from '@customer-web-app/shared-i18n';
import { IconError } from '@customer-web-app/shared/base/common-icon';
import * as Sentry from '@sentry/react';
import { useCookieLanguage } from '@customer-web-app/shared/base/components';
import { MainSitePopUpAds } from '@customer-web-app/advertisement-pages';

declare const appConfig: any;

function Advertisement() {
  if (appConfig.advName === 'CashServiceAds') {
    return <MainSitePopUpAds />;
  }
  return null;
}

export function App() {
  useCookieLanguage(i18nInstance);


  return (
    <Sentry.ErrorBoundary fallback={IconError}>
      <Advertisement />
    </Sentry.ErrorBoundary>
  );
}

export default App;
