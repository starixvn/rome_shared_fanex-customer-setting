// eslint-disable-next-line @typescript-eslint/no-unused-vars
import { EditMultipleCustomerPage } from '@customer-web-app/edit-multiple-customer-page';
import NiceModal from '@ebay/nice-modal-react';

export function App() {
  return (
    <NiceModal.Provider>
      <EditMultipleCustomerPage />
    </NiceModal.Provider>
  );
}

export default App;
