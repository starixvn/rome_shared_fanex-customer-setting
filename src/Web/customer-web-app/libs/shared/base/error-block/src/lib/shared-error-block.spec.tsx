import { render } from '@testing-library/react';

import SharedErrorBlock from './shared-error-block';

describe('SharedErrorBlock', () => {
  it('should render successfully', () => {
    const { baseElement } = render(<SharedErrorBlock />);
    expect(baseElement).toBeTruthy();
  });
});
