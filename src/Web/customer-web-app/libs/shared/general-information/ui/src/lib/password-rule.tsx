import { useTranslate } from '@tolgee/react';

import { UserLevel } from '@customer-web-app/shared-constants';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

export default function PasswordRule({ roleId }: Readonly<{ roleId: UserLevel }>) {
  const { t } = useTranslate();
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
