import { MouseEvent } from 'react';
import { useTranslation } from 'react-i18next';

import { IconInfo } from '@customer-web-app/shared/base/common-icon';

import { CustomerSettingInformationPageUrl, TabNames } from '../customer-setting-information-page-url';

export default function ParlayMaxPayoutNote() {
  const { t } = useTranslation();
  const { url } = CustomerSettingInformationPageUrl({ tabName: TabNames.MaxPayoutSettingTab });
  const handleClick = (e: MouseEvent<HTMLDivElement>) => {
    e.preventDefault();
    window.open(url, '_blank');
    e.stopPropagation();
  };

  const tooltipContent = t('MaxPayoutInfoTitle');
  return (
    <span onClick={handleClick} aria-hidden data-tooltip-id="global-tooltip" data-tooltip-content={tooltipContent}>
      {IconInfo}
    </span>
  );
}
