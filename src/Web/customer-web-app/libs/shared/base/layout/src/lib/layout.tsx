import { useEffect, useState } from 'react';

import styles from './layout.module.scss';

/* eslint-disable-next-line */
export interface LayoutProps {
  children: React.ReactNode;
  theme: string;
}

export function Layout(props: LayoutProps) {
  return (
    <div className={styles['container']} data-theme={props.theme}>
      {props.children}
    </div>
  );
}

export const useTheme = (init = 'light') => {
  const [theme, setTheme] = useState<string>(init);

  useEffect(() => {
    const searchParams = new URLSearchParams(document.location.search);

    if (searchParams.has('theme')) {
      setTheme(searchParams.get('theme') ?? 'light');
    }

    if (searchParams.has('isDarkMode')) {
      const isDarkMode = searchParams.get('isDarkMode') === 'true';
      setTheme(isDarkMode ? 'dark' : 'light');
    }
  }, []);

  return theme;
};

export default Layout;
