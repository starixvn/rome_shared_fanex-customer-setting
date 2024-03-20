import { render } from '@testing-library/react';

import SharedBaseCharactersSelector from './shared-base-characters-selector';

describe('SharedBaseCharactersSelector', () => {
  it('should render successfully', () => {
    const { baseElement } = render(<SharedBaseCharactersSelector />);
    expect(baseElement).toBeTruthy();
  });
});
