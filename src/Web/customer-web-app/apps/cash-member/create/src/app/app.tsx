// eslint-disable-next-line @typescript-eslint/no-unused-vars
import { i18nInstance } from '@customer-web-app/shared-i18n';
import { CreateCashMemberPageWrapper } from '@customer-web-app/create-cash-member-page';
import { IconError } from '@customer-web-app/shared/base/common-icon';
import * as Sentry from '@sentry/react';
import { useCookieLanguage } from '@customer-web-app/shared/base/components';

export function App() {
  useCookieLanguage(i18nInstance);

  return (
    <Sentry.ErrorBoundary fallback={IconError}>
      <CreateCashMemberPageWrapper />
    </Sentry.ErrorBoundary>
  );
}

export default App;
