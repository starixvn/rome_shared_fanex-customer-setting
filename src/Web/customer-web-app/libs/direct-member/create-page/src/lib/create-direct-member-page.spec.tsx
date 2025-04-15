import { render } from '@testing-library/react';

import CreateDirectMemberPage from './create-direct-member-page';

describe('CreateDirectMemberPage', () => {
  it('should render successfully', () => {
    const { baseElement } = render(<CreateDirectMemberPage />);
    expect(baseElement).toBeTruthy();
  });
});
