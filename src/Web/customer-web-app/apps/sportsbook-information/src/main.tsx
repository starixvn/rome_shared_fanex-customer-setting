import 'normalize.css';
import '@customer-web-app/shared/base/styles';
import '@customer-web-app/shared-i18n';

import App from './app/app';
import { StrictMode } from 'react';
import * as ReactDOM from 'react-dom/client';

const root = ReactDOM.createRoot(document.getElementById('root') as HTMLElement);
root.render(
  <StrictMode>
    <App />
  </StrictMode>
);
