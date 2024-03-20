import { render } from '@testing-library/react';

import WrapperTooltip from './wrapper-tooltip';

describe('WrapperTooltip', () => {
  it('should render successfully', () => {
    const { baseElement } = render(<WrapperTooltip />);
    expect(baseElement).toBeTruthy();
  });
});
