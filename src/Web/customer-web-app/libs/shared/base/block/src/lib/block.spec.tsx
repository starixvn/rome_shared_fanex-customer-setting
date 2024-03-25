import { render } from '@testing-library/react';

import Block from './block';

describe('Block', () => {
  it('should render successfully', () => {
    const { baseElement } = render(<Block title="text" />);
    expect(baseElement).toBeTruthy();
  });
});
