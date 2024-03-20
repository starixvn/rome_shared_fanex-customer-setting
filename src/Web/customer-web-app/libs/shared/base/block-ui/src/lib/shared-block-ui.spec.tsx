import { render } from '@testing-library/react';

import SharedBlockUi from './shared-block-ui';

describe('SharedBlockUi', () => {
  it('should render successfully', () => {
    const { baseElement } = render(<SharedBlockUi />);
    expect(baseElement).toBeTruthy();
  });
});
