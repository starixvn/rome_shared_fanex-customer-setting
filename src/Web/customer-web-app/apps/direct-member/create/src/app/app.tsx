// eslint-disable-next-line @typescript-eslint/no-unused-vars
import { CreateCashMemberPage } from '@customer-web-app/create-cash-member-page';
import { IconError } from '@customer-web-app/shared/base/common-icon';
import * as Sentry from '@sentry/react';

export function App() {
  return (
    <Sentry.ErrorBoundary fallback={IconError}>
      <CreateCashMemberPage />
    </Sentry.ErrorBoundary>
  );
}

export default App;
