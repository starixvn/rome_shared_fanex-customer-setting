import { GeneralInfo } from '@customer-web-app/shared-general-information-state';

import GeneralInformationValidator from './general-information-validator';

export default class EditCashMemberGeneralInformationValidator extends GeneralInformationValidator {

  constructor(generalInfo: GeneralInfo) {
    super(generalInfo); 
  }

  public async validateAllRules() {
    const results = {
      [`firstName`]: await this.validateRequired('firstName'),
      [`lastName`]: await this.validateRequired('lastName'),
      [`mobilePhone`]: await this.validateMobilePhone(),
    };
    return results;
  }
}
