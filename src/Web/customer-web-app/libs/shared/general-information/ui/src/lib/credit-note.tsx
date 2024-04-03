import { useTranslation } from 'react-i18next';

import { NumberHelper } from '@customer-web-app/shared-helpers';
import { IconCredit } from '@customer-web-app/shared/base/common-icon';

type Props = {
  availableCredit: number;
  memberMaxCredit: number;
};

export default function CreditNote({ availableCredit, memberMaxCredit }: Props) {
  const { t } = useTranslation();
  const note = t('creditNote', {
    availableCredit: NumberHelper.roundDownNumber(availableCredit),
    memberMaxCredit: NumberHelper.roundDownNumber(memberMaxCredit),
  });

  return (
    <span data-tooltip-id="global-tooltip" data-tooltip-html={note}>
      <IconCredit />
    </span>
  );
}
