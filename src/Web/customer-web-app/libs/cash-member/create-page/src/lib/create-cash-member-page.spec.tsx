import { render } from '@testing-library/react';

import CreateCashMemberPage from './create-cash-member-page';

describe('CreateCashMemberPage', () => {
  it('should render successfully', () => {
    const { baseElement } = render(<CreateCashMemberPage />);
    expect(baseElement).toBeTruthy();
  });
});
