// eslint-disable-next-line @typescript-eslint/no-unused-vars
import { CreateDirectMemberPage } from '@customer-web-app/create-direct-member-page';
import { IconError } from '@customer-web-app/shared/base/common-icon';
import * as Sentry from '@sentry/react';

export function App() {
  return (
    <Sentry.ErrorBoundary fallback={IconError}>
      <CreateDirectMemberPage />
    </Sentry.ErrorBoundary>
  );
}

export default App;
