import { useTranslate } from '@tolgee/react';

import { IconInfo } from '@customer-web-app/shared/base/common-icon';

import { CustomerSettingInformationPageUrl } from '../../customer-setting-information-page-url';

export default function SportsBookBetSettingMaxPayoutNote() {
  const { url } = CustomerSettingInformationPageUrl({ tab: 1 });
  const { t } = useTranslate();
  const tooltipContent = t('MaxPayoutInfoTitle');

  return (
    <a
      href={url}
      target="_blank"
      rel="noopener noreferrer"
      data-tooltip-id="global-tooltip"
      data-tooltip-content={tooltipContent}
    >
      {IconInfo}
    </a>
  );
}
