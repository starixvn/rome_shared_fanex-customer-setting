import { useTranslation } from 'react-i18next';

import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

export default function CopySettingFromNote() {
  const { t } = useTranslation();
  return (
    <>
      <span
        className="icon ic-info"
        style={{ margin: '0 0 0 5px', fontSize: '18px' }}
        data-tooltip-id="copy-note"
        data-tooltip-content={t('CopySettingFromNote')}
      />
      <WrapperTooltip className="global-tooltip" id="copy-note" />
    </>
  );
}
