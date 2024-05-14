import { TFunction } from 'i18next';

import { ValidationResult } from '@customer-web-app/share-models';
import { UserLevel } from '@customer-web-app/shared-constants';
import { GeneralInfo } from '@customer-web-app/shared-general-information-state';
import { IconCorrect, IconWrong } from '@customer-web-app/shared/base/common-icon';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import { validateMaxCredit } from './general-information-validator';

export function maxCreditValidateFiled(obj: GeneralInfo): validateMaxCredit {
  const maxCredits: Record<UserLevel, validateMaxCredit> = {
    [UserLevel.Company]: 'companyMaxCredit',
    [UserLevel.Super]: 'superMaxCredit',
    [UserLevel.Master]: 'masterMaxCredit',
    [UserLevel.Agent]: 'agentMaxCredit',
    [UserLevel.Member]: 'memberMaxCredit',
  };
  return maxCredits[obj.roleId as UserLevel];
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
