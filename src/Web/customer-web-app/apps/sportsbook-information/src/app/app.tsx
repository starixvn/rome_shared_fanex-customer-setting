import classNames from 'classnames';
import { useEffect, useState } from 'react';

import { TabOptions } from '@customer-web-app/share-models';

import styles from './app.module.scss';
import { Layout } from './layout';
import { BetSettingTab } from './tabs/bet-setting-tab';
import { CommissionHintTab } from './tabs/commission-hint-tab';
import { MaxPayoutSettingTab } from './tabs/max-payout-setting-tab';
import { ProductCategorySettingTab } from './tabs/product-category-setting-tab';
import { PtSettingTab } from './tabs/pt-setting-tab';
import { useTranslate } from '@tolgee/react';
import { WinLossLimitTab } from './tabs/win-loss-limit-tab';

export function App() {
  const { t } = useTranslate();
  const [tabs, setTabs] = useState<TabOptions[]>([]);
  const [selectedTab, setSelectedTab] = useState<TabOptions>({} as TabOptions);

  useEffect(() => {
    const initTabs = () => {
      const tabs = informationPageParams.tabs.map((e) => {
        return e;
      });

      const activeTab = tabs.find((tab) => tab.isActive);
      setTabs(tabs);
      activeTab && setSelectedTab(activeTab);
    };
    if (informationPageParams) {
      initTabs();
    }
  }, []);

  const handleTabClick = (tab: TabOptions) => {
    setSelectedTab(tab);
  };

  return (
    <Layout>
      <div className={styles.sportbookinformation}>
        <div className={styles.title}>
          <h1>{t('CustomerSettingInformation')}  </h1>
        </div>
        <div className={styles.content}>
          <ul className={styles.tab} role="tablist">
            {tabs.map((tab, index) => (
              <li key={tab.name + index.toString()} className={classNames({ [styles.active]: tab === selectedTab })}>
                <a onClick={() => handleTabClick(tab)} aria-controls="home" role="tab" data-toggle="tab">
                  {t(tab.name)}
                </a>
              </li>
            ))}
          </ul>
          <RenderTab tab={selectedTab} />
        </div>
      </div>
    </Layout>
  );
}

const RenderTab = ({ tab }: { tab: TabOptions }) => {
  switch (tab.id) {
    case 'PTSettingTab':
      return <PtSettingTab url={tab.url} />;
    case 'BetSettingTab':
      return <BetSettingTab />;
    case 'ProductCategorySettingTab':
      return <ProductCategorySettingTab url={tab.url} />;
    case 'CommissionHintTab':
      return <CommissionHintTab url={tab.url} />;
    case 'MaxPayoutSettingTab':
      return <MaxPayoutSettingTab url={tab.url} />;
    case 'WinLossLimitTab':
      return <WinLossLimitTab url={tab.url} />;
    default:
      return <div />;
  }
};

export default App;
