import { useTranslation } from 'react-i18next';

export default function CopySettingFromNote() {
  const { t } = useTranslation();
  return (
    <span
      className="icon ic-info"
      style={{ margin: '0 0 0 5px', fontSize: '18px' }}
      data-tooltip-id="global-tooltip"
      data-tooltip-content={t('CopySettingFromNote')}
    />
  );
}
