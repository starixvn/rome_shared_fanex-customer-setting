// eslint-disable-next-line @typescript-eslint/no-unused-vars
import { i18nInstance } from '@customer-web-app/shared-i18n';
import { IconError } from '@customer-web-app/shared/base/common-icon';
import * as Sentry from '@sentry/react';
import { OverlaySkeleton, useCookieLanguage } from '@customer-web-app/shared/base/components';
import { CashServiceAds } from '@customer-web-app/advertisement-pages';

declare const appConfig: any;

function CashServicePage() {
  return <CashServiceAds
    onClose={() => { }}
    onConfirm={() => { }}
    cashServiceStatus={{ isCashServiceEnabled: false, isCanUpdateCashServiceStatus: false, isOfferedCashService: true }}
    onDontShowAgain={() => { }}
  />;
}

function Advertisement() {
  if (appConfig.advName === 'CashServiceAds') {
    return <CashServicePage />;
  }
  return (
    <div>Advertisement</div>
  );
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
