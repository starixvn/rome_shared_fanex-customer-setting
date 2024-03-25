import i18next, { changeLanguage } from 'i18next';
import { ReactNode, useEffect } from 'react';

import { useTheme } from '@customer-web-app/shared/base/layout';

interface IProps {
  children: ReactNode;
}

export const Layout = ({ children }: IProps) => {
  const theme = useTheme();

  useEffect(() => {
    const searchParams = new URLSearchParams(document.location.search);
    const lng = searchParams.get('lng') ?? 'en-US';
    const { languages } = i18next;
    if (!languages.includes(lng)) {
      changeLanguage('en-US');
    }
  }, []);

  return (
    <div data-theme={theme} style={{ height: '100%' }}>
      {children}
    </div>
  );
};
