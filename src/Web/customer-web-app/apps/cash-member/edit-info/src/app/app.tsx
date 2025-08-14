// eslint-disable-next-line @typescript-eslint/no-unused-vars
import { i18nInstance } from '@customer-web-app/shared-i18n';
import { IconError } from '@customer-web-app/shared/base/common-icon';
import * as Sentry from '@sentry/react';
import { EditInfoCashMemberPage, ViewInfoCashMemberForm ,EditInfoNormalMemberAlpha} from '@customer-web-app/edit-info-cash-member-page';
import { useCookieLanguage, useSystems } from '@customer-web-app/shared/base/components';

function EditInfoCustomerPageWrapper() {
  const { isAlphaSystem, isExternalNapSystem } = useSystems(updateInfoCustomerPageConfig.systemId);

  if(isAlphaSystem && updateInfoCustomerPageConfig.isCashMember){
    return <EditInfoCashMemberPage />
  }

  if(isAlphaSystem && !updateInfoCustomerPageConfig.isCashMember){
    return <EditInfoNormalMemberAlpha />
  }

  if(isExternalNapSystem && updateInfoCustomerPageConfig.isCashMember){
    return <ViewInfoCashMemberForm />
  }
  return null;
}

export function App() {
  useCookieLanguage(i18nInstance);


  return (
    <Sentry.ErrorBoundary fallback={IconError}>
      <EditInfoCustomerPageWrapper />
    </Sentry.ErrorBoundary>
  );
}

export default App;
