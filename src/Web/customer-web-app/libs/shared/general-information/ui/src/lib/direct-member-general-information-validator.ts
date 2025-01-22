import { ValidationResult } from '@customer-web-app/share-models';
import { UserLevel } from '@customer-web-app/shared-constants';
import { GeneralInfo } from '@customer-web-app/shared-general-information-state';
import { SettingService } from '@customer-web-app/shared-group-settings-services';
import { NumberHelper } from '@customer-web-app/shared-helpers';

import GeneralInformationValidator from './general-information-validator';

export default class DirectMemberGeneralInformationValidator extends GeneralInformationValidator {
  private numberOfAccounts = 1;
  private selectedDirectMember = '';

  constructor(generalInfo: GeneralInfo, numberOfAccounts: number, selectedDirectMember: string) {
    super(generalInfo);

    this.numberOfAccounts = numberOfAccounts;
    this.selectedDirectMember = selectedDirectMember;
  }

  public async validateAll() {
    const results = await super.validateAll();

    return { ...results, [`selectedDirectMember`]: await this.validateSelectedDirectMember() };
  }

  public async validateSelectedDirectMember(): Promise<ValidationResult | boolean> {
    this.resetValidationState();

    if (this.selectedDirectMember) {
      const custId = await SettingService.GetDirectMember(this.selectedDirectMember);
      if (custId > 0) {
        return true;
      } else {
        return (this.validationState = new ValidationResult('selectedDirectMember', 'UserNotExist'));
      }
    }

    return true;
  }

  public validateMaxCredit(isEditMode = false): ValidationResult | boolean {
    this.resetValidationState();
    const { maxCredit, uplineMaxCredit, uplineMemberMaxCredit, roleId, maxCreditMin, maxCreditMax, memberMaxCredit } =
      this.generalInfo;

    if (maxCredit === undefined) {
      return (this.validationState = new ValidationResult('maxCredit', 'RequiredField'));
    }

    if (uplineMaxCredit !== undefined && !isNaN(uplineMaxCredit) && !isEditMode) {
      const max =
        (uplineMemberMaxCredit ? Math.min(uplineMaxCredit, uplineMemberMaxCredit) : uplineMaxCredit) /
        this.numberOfAccounts;
      if (maxCredit > max) {
        return (this.validationState = new ValidationResult(
          `maxCredit`,
          this.invalidateMaxCreditMessages[roleId as UserLevel],
          {
            min: '0',
            max: NumberHelper.roundDownNumber(max),
          }
        ));
      }
    }

    if (isEditMode && roleId < UserLevel.Super && maxCreditMin !== undefined && maxCreditMax !== undefined) {
      const max =
        roleId === UserLevel.Member && memberMaxCredit ? Math.min(memberMaxCredit, maxCreditMax) : maxCreditMax;
      if (maxCredit < maxCreditMin || maxCredit > max) {
        return (this.validationState = new ValidationResult(
          `maxCredit`,
          this.invalidateMaxCreditMessages[roleId as UserLevel],
          {
            min: NumberHelper.roundDownNumber(maxCreditMin),
            max: NumberHelper.roundDownNumber(max),
          }
        ));
      }
    }

    const isEditSuper = isEditMode && roleId === UserLevel.Super && maxCreditMin !== undefined;
    if (isEditSuper && maxCredit < maxCreditMin) {
      return (this.validationState = new ValidationResult(
        `maxCredit`,
        this.invalidateMaxCreditMessages[roleId as UserLevel],
        {
          min: NumberHelper.roundDownNumber(maxCreditMin),
        }
      ));
    }
    return true;
  }
}
