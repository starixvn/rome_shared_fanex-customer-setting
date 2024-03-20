import { render } from '@testing-library/react';

import EditCustomerPage from './edit-customer-page';

describe('EditCustomerPage', () => {
  it('should render successfully', () => {
    const { baseElement } = render(<EditCustomerPage />);
    expect(baseElement).toBeTruthy();
  });
});
