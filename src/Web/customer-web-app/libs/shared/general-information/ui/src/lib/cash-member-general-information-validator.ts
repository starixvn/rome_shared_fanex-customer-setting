import { ValidationResult } from '@customer-web-app/share-models';
import { GeneralInfo } from '@customer-web-app/shared-general-information-state';
import { SettingService } from '@customer-web-app/shared-group-settings-services';

import GeneralInformationValidator from './general-information-validator';

export default class CashMemberGeneralInformationValidator extends GeneralInformationValidator {
  private readonly selectedCashMember: string;

  constructor(generalInfo: GeneralInfo, selectedCashMember: string) {
    super(generalInfo); 

    this.selectedCashMember = selectedCashMember;
  }

  public async validateAllRules() {
    const results = {
      [`userName`]: await super.validateUsername(),
      [`password`]: super.validatePassword(),
      [`selectedCashMember`]: await this.validateSelectedCashMember(),
      [`bankAccountNumber`]: await this.validateBankAccountNumber(),
      [`bankId`]: await this.validateBankId(),
      [`firstName`]: await this.validateRequired('firstName'),
      [`lastName`]: await this.validateRequired('lastName'),
      [`mobilePhone`]: await this.validateMobilePhone(),
    };
    return results;
  }

  public async validateSelectedCashMember(): Promise<ValidationResult | boolean> {
    this.resetValidationState();

    if (this.selectedCashMember) {
      const custId = await SettingService.GetCashMember(this.selectedCashMember);
      if (custId > 0) {
        return true;
      } else {
        return (this.validationState = new ValidationResult('selectedCashMember', 'UserNotExist'));
      }
    }

    return true;
  }

}
