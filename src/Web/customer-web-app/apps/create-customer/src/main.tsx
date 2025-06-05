import 'normalize.css';
import 'react-tooltip/dist/react-tooltip.css';
import '@customer-web-app/shared/base/styles';
import { tolgeeInit } from '@customer-web-app/shared-i18n-tolgee';

import { StrictMode } from 'react';
import * as ReactDOM from 'react-dom/client';
import { TolgeeProvider } from "@tolgee/react";

import App from './app/app';

const root = ReactDOM.createRoot(document.getElementById('root') as HTMLElement);
root.render(
  <StrictMode>
    <TolgeeProvider tolgee={tolgeeInit}>
        <App />
    </TolgeeProvider>
  </StrictMode>
);
