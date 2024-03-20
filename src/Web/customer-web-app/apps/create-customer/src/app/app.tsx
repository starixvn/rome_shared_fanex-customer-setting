import { CreateCustomerPage } from '@customer-web-app/create-customer-page';
import { IconError } from '@customer-web-app/shared/base/common-icon';
import * as Sentry from '@sentry/react';

// Sentry.init({
//   dsn: 'https://8911356ce96e46d293bd145997b43fdd@sentry.nexdev.net/175',
//   integrations: [new Sentry.BrowserTracing({ tracingOrigins: ['*'] })],
//   // Set tracesSampleRate to 1.0 to capture 100%
//   // of transactions for performance monitoring.
//   tracesSampleRate: 1.0,

//   // Set `tracePropagationTargets` to control for which URLs distributed tracing should be enabled
//   // tracePropagationTargets: ['localhost', /^https:\/\/yourserver\.io\/api/],

//   // Capture Replay for 10% of all sessions,
//   // plus for 100% of sessions with an error
//   replaysSessionSampleRate: 0.1,
//   replaysOnErrorSampleRate: 1.0,
// });

export function App() {
  return (
    <Sentry.ErrorBoundary fallback={IconError}>
      <CreateCustomerPage />
    </Sentry.ErrorBoundary>
  );
}

export default App;
