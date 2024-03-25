import { TFunction } from 'i18next';

import { ValidationResult } from '@customer-web-app/share-models';
import { UserLevel } from '@customer-web-app/shared-constants';
import { GeneralInfo } from '@customer-web-app/shared-general-information-state';
import { NumberHelper } from '@customer-web-app/shared-helpers';
import { IconCorrect, IconWrong } from '@customer-web-app/shared/base/common-icon';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import { validateMaxCredit } from './general-information-validator';
import styles from './general-information.module.scss';

export const rangeMinMax = (min: number, max: number, roleId?: UserLevel) => {
  let guides: number[] = [];
  const words: string[] = [
    NumberHelper.formatWithCommas(min, 4),
    '<=',
    'credit',
    NumberHelper.formatWithCommas(max, 4),
    '>=',
  ];
  if (min >= 0 && max === 0) {
    guides = [2, 4, 0];
  }
  if (min >= 0 && max >= 0) {
    guides = [0, 1, 2, 1, 3];
  }
  if (roleId === UserLevel.Super) {
    guides = [0, 1, 2];
  }

  return <div className={styles['max-credit']}>{<b>{guides.map((e) => words[e]).join(' ')}</b>}</div>;
};

export function maxCreditValidateFiled(obj: GeneralInfo): validateMaxCredit {
  const maxCredits: Record<UserLevel, validateMaxCredit> = {
    [UserLevel.Company]: 'companyMaxCredit',
    [UserLevel.Super]: 'superMaxCredit',
    [UserLevel.Master]: 'masterMaxCredit',
    [UserLevel.Agent]: 'agentMaxCredit',
    [UserLevel.Member]: 'memberMaxCredit',
  };
  return maxCredits[(obj.roleId) as UserLevel];
}

export const errorTooltip = (
  locationTooltip: string,
  validationResults: { [key: string]: boolean | ValidationResult },
  t: TFunction
) => {
  if (!locationTooltip) {
    return null;
  }
  const tooltipInfo = validationResults[locationTooltip] as ValidationResult;

  if (!tooltipInfo) {
    return null;
  }

  const { fieldName, isSuccessful, message, msgParams } = tooltipInfo;
  return (
    <WrapperTooltip
      type="error"
      anchorSelect={`#${fieldName}`}
      isOpen={!isSuccessful}
      content={t(message, msgParams)}
    />
  );
};

export const iconTrueFalse = (isValidResult: boolean) => (isValidResult ? <IconWrong /> : <IconCorrect />);
