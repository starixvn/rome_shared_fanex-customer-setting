import 'normalize.css';
import 'react-tooltip/dist/react-tooltip.css';
import '@customer-web-app/shared/base/styles';

import { StrictMode } from 'react';
import * as ReactDOM from 'react-dom/client';

import App from './app/app';

const root = ReactDOM.createRoot(document.getElementById('adv-popup-root') as HTMLElement);
root.render(
  <StrictMode>
    <App />
  </StrictMode>
);
