import { render } from '@testing-library/react';

import CreateSuperNote from './create-super-note';

describe('CreateSuperNote', () => {
  it('should render successfully', () => {
    const { baseElement } = render(<CreateSuperNote />);
    expect(baseElement).toBeTruthy();
  });
});
