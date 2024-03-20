import { render } from '@testing-library/react';

import SiteCurrencySection from './site-currency-section';

describe('SiteCurrencySection', () => {
  it('should render successfully', () => {
    const { baseElement } = render(
      <SiteCurrencySection
        site=""
        sites={[]}
        currency=""
        currencies={[]}
        handleChangeCurrency={() => {
          console.log("handleChangeCurrency");
        }}
        handleChangeSite={() => {
          console.log("handleChangeSite");
        }}
        handleChangeInternalUser={() => {
          console.log("handleChangeInternalUser");
        }}
        isInternalUser
      />
    );
    expect(baseElement).toBeTruthy();
  });
});
