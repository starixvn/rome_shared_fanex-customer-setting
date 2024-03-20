import { EditCustomerPage } from '@customer-web-app/edit-customer-page';
import NiceModal from '@ebay/nice-modal-react';

export function App() {
  return (
    <NiceModal.Provider>
      <EditCustomerPage />
    </NiceModal.Provider>
  );
}

export default App;
