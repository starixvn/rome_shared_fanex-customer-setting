import { useTranslation } from 'react-i18next';

import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import { ParlayMaxPayoutGroupItem } from '@customer-web-app/shared-group-settings-types';
import { Block, BlockColumn, BlockRow } from '@customer-web-app/shared/base/block';

import { GroupSettingBlockProps } from '../position-taking/super-position-taking';

const ParlayMaxPayoutSettingView = (props: GroupSettingBlockProps) => {
  const { t } = useTranslation();
  const { productGroups } = useGroupSettingContext();
  const parlayMaxPayouts = getSettingItems<ParlayMaxPayoutGroupItem>(
    productGroups,
    ProductGroupIds.SportsBook,
    GroupTypeIds.ParlayMaxPayout
  );

  return (
    <Block title={props.title}>
      <BlockRow isHeaderRow>
        <BlockColumn>{t('singleMatchParlay')}</BlockColumn>
        <BlockColumn>{t('combo2')}</BlockColumn>
        <BlockColumn>{t('combo35')}</BlockColumn>
        <BlockColumn>{t('comboOver6')}</BlockColumn>
      </BlockRow>
      {parlayMaxPayouts?.map((item) => {
        return (
          <BlockRow key={item.displayName} label={item.displayName}>
            <BlockColumn>
              <div className="show-value">{item.singleMatchParlay}</div>
            </BlockColumn>
            <BlockColumn>
              <div className="show-value">{item.combo2}</div>
            </BlockColumn>
            <BlockColumn>
              <div className="show-value">{item.combo35}</div>
            </BlockColumn>
            <BlockColumn>
              <div className="show-value">{item.comboOver6}</div>
            </BlockColumn>
          </BlockRow>
        );
      })}
    </Block>
  );
};

export default ParlayMaxPayoutSettingView;
