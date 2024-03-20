import { t } from 'i18next';

import { GeneralInfo } from '@customer-web-app/shared-general-information-state';
import { IconInfo } from '@customer-web-app/shared/base/common-icon';

import { CustomerSettingInformationPageUrl } from '../../customer-setting-information-page-url';

export default function SportsBookBetSettingMaxPayoutNote({
  generalInfo,
}: Readonly<{ generalInfo: GeneralInfo | undefined }>) {
  const { url } = CustomerSettingInformationPageUrl({ tab: 1, generalInfo });
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
