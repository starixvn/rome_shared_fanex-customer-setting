import { render } from '@testing-library/react';

import EditMultipleCustomerPage from './edit-multiple-customer-page';

describe('EditMultipleCustomerPage', () => {
  it('should render successfully', () => {
    const { baseElement } = render(<EditMultipleCustomerPage />);
    expect(baseElement).toBeTruthy();
  });
});
