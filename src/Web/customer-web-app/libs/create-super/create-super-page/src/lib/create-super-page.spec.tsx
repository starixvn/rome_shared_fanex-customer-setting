import { render } from '@testing-library/react';

import CreateSuperPage from './create-super-page';

describe('CreateSuperPage', () => {
  it('should render successfully', () => {
    const { baseElement } = render(<CreateSuperPage />);
    expect(baseElement).toBeTruthy();
  });
});
