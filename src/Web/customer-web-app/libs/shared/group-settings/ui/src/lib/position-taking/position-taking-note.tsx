import { t } from 'i18next';
import { MouseEvent } from 'react';

import { IconInfo } from '@customer-web-app/shared/base/common-icon';

import { CustomerSettingInformationPageUrl } from '../customer-setting-information-page-url';

export default function PositionTakingNote() {
  const { url } = CustomerSettingInformationPageUrl({ tab: 2 });

  const handleClick = (e: MouseEvent<HTMLDivElement>) => {
    e.preventDefault();
    window.open(url, '_blank');
    e.stopPropagation();
  };

  const tooltipContent = t('PTInfoTitle');
  return (
    <span onClick={handleClick} aria-hidden data-tooltip-id="global-tooltip" data-tooltip-content={tooltipContent}>
      {IconInfo}
    </span>
  );
}
