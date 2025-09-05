import { useTranslation } from 'react-i18next';

import { IconInfo } from '@customer-web-app/shared/base/common-icon';

import { CustomerSettingInformationPageUrl, TabNames } from '../../customer-setting-information-page-url';

export default function SportsBookBetSettingMaxPayoutNote() {
  const { url } = CustomerSettingInformationPageUrl({ tabName: TabNames.MaxPayoutSettingTab });
  const { t } = useTranslation();
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
