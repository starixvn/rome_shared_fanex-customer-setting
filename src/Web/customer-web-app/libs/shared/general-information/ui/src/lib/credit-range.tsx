import { useTranslate } from '@tolgee/react';

import { NumberHelper } from '@customer-web-app/shared-helpers';

type Props = {
  min: number;
  max?: number;
};

export default function CreditRange({ min, max }: Props) {
  const { t } = useTranslate();

  return (
    <>
      <span>
        {`${t('Min')}: `}
        <strong>{NumberHelper.roundDownNumber(min)}</strong>
      </span>
      {max !== undefined && (
        <span>
          {` - ${t('Max')}: `}
          <strong>{NumberHelper.roundDownNumber(max)}</strong>
        </span>
      )}
    </>
  );
}
