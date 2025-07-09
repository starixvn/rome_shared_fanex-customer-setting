export const splitMobile = (mobile: string) => {
    const fm = mobile.includes('-') ? mobile.split('-')[0] : '';
    const m = mobile.includes('-') ? mobile.split('-')[1] : mobile;
    return { countryCode: fm, mobile: m };
  };