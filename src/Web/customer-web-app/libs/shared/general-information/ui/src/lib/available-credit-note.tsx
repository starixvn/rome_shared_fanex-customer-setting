import { useTranslation } from 'react-i18next';

import { NumberHelper } from '@customer-web-app/shared-helpers';
import { IconCredit } from '@customer-web-app/shared/base/common-icon';

type Props = {
  availableCredit: number;
};

export default function AvailableCreditNote({ availableCredit }: Props) {
  const { t } = useTranslation();
  const note = t('AvailableCredit', {
    availableCredit: NumberHelper.roundDownNumber(availableCredit || 0),
  });

  return (
    <span data-tooltip-id="global-tooltip" data-tooltip-content={note}>
      <IconCredit />
    </span>
  );
}
