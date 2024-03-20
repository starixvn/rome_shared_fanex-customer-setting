import { useTranslation } from 'react-i18next';
import { UserLevel } from '@customer-web-app/shared-constants';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

export default function PasswordRule({ roleId }: Readonly<{ roleId: UserLevel }>) {
  const { t } = useTranslation();
  const passwordRule = roleId === UserLevel.Member ? t('lblPasswordMemberRequire') : t('lblPasswordRequire');

  return (
    <>
      <span
        className="icon ic-info"
        style={{ margin: '0 0 0 5px', fontSize: '18px' }}
        data-tooltip-id="password-rule"
        data-tooltip-html={passwordRule}
      />
      <WrapperTooltip className="global-tooltip" id="password-rule" />
      </>
  );
}
