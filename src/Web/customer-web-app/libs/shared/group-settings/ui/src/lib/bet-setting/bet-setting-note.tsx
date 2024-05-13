import { MouseEvent } from 'react';
import { useTranslation } from 'react-i18next';

import { IconInfo } from '@customer-web-app/shared/base/common-icon';

import { CustomerSettingInformationPageUrl } from '../customer-setting-information-page-url';

export default function BetSettingNote() {
  const { t } = useTranslation();
  const { url } = CustomerSettingInformationPageUrl({ tab: 0 });
  const handleClick = (e: MouseEvent<HTMLDivElement>) => {
    e.preventDefault();
    window.open(url, '_blank');
    e.stopPropagation();
  };

  const tooltipContent = t('BetSettingInfoTitle');
  return (
    <span onClick={handleClick} aria-hidden data-tooltip-id="global-tooltip" data-tooltip-content={tooltipContent}>
      {IconInfo}
    </span>
  );
}
