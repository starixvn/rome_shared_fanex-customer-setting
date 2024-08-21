import { CopySuperBetSettingsPage, EditCustomerPage } from '@customer-web-app/edit-customer-page';
import NiceModal from '@ebay/nice-modal-react';

export function App() {
  return (
    <NiceModal.Provider>
      {params.productGroupId !== 0 ? <EditCustomerPage /> : <CopySuperBetSettingsPage />}
    </NiceModal.Provider>
  );
}

export default App;
