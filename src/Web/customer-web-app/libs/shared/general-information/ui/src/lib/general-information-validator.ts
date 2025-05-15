import { ValidationResult } from '@customer-web-app/share-models';
import { SystemIds, UserLevel } from '@customer-web-app/shared-constants';
import { GeneralInfo } from '@customer-web-app/shared-general-information-state';
import { SettingService } from '@customer-web-app/shared-group-settings-services';
import { NumberHelper } from '@customer-web-app/shared-helpers';

export type validateMaxCredit =
  | 'companyMaxCredit'
  | 'superMaxCredit'
  | 'masterMaxCredit'
  | 'agentMaxCredit'
  | 'memberMaxCredit';

export type validatePassword = 'password';
export type validateUsername = 'userName' | 'newUserName';
export type validateUserInfo = 'firstName' | 'lastName' | 'mobilePhone' | 'fax';

export type generalInformationValidatorFields = validateMaxCredit | validatePassword | validateUsername | validateUserInfo;

export default class GeneralInformationValidator {
  protected readonly generalInfo: GeneralInfo;
  public validationState: ValidationResult;

  protected invalidateMaxCreditMessages: Record<UserLevel, string> = {
    [UserLevel.Company]: '',
    [UserLevel.Super]: 'superMaxCreditInvalid',
    [UserLevel.Master]: 'masterMaxCreditInvalid',
    [UserLevel.Agent]: 'agentMaxCreditInvalid',
    [UserLevel.Member]: 'givenCreditInvalid',
  };

  constructor(generalInfo: GeneralInfo) {
    this.generalInfo = generalInfo;
    this.validationState = ValidationResult.CreateSuccessfulResult() as ValidationResult;
  }

  public resetValidationState(): void {
    this.validationState = ValidationResult.CreateSuccessfulResult() as ValidationResult;
  }

  public async validateAll() {
    const results = {
      [`userName`]: await this.validateUsername(),
      [`password`]: this.validatePassword(),
      [`superMaxCredit`]: this.generalInfo.roleId === UserLevel.Super ? this.validateMaxCredit() : true,
      [`masterMaxCredit`]: this.generalInfo.roleId === UserLevel.Master ? this.validateMaxCredit() : true,
      [`agentMaxCredit`]: this.generalInfo.roleId === UserLevel.Agent ? this.validateMaxCredit() : true,
      [`memberMaxCredit`]: (() => {
        switch (this.generalInfo.roleId) {
          case UserLevel.Agent:
            return this.validateMemberMaxCreditWhenCreateAgent();
          case UserLevel.Member:
            return this.validateMaxCredit();
          default:
            return true;
        }
      })(),
    };
    return results;
  }

  public async validateAllForEdit() {
    const results = {
      [`superMaxCredit`]: this.generalInfo.roleId === UserLevel.Super ? this.validateMaxCredit(true) : true,
      [`masterMaxCredit`]: this.generalInfo.roleId === UserLevel.Master ? this.validateMaxCredit(true) : true,
      [`agentMaxCredit`]: this.generalInfo.roleId === UserLevel.Agent ? this.validateMaxCredit(true) : true,
      [`memberMaxCredit`]: (() => {
        switch (this.generalInfo.roleId) {
          case UserLevel.Agent:
            return this.validateMemberMaxCreditWhenCreateAgent(true);
          case UserLevel.Member:
            return this.validateMaxCredit(true);
          default:
            return true;
        }
      })(),
    };
    return results;
  }

  public async validateField(fieldName: generalInformationValidatorFields, isEditMode = false) {
    console.log('validateField', fieldName, isEditMode);
    switch (fieldName) {
      case 'userName':
      case 'newUserName':
        return await this.validateUsername();
      case 'password':
        return this.validatePassword();
      case 'superMaxCredit':
      case 'masterMaxCredit':
      case 'agentMaxCredit':
        return this.validateMaxCredit(isEditMode);
      case 'memberMaxCredit':
        return this.generalInfo.roleId === UserLevel.Agent
          ? this.validateMemberMaxCreditWhenCreateAgent(isEditMode)
          : this.validateMaxCredit(isEditMode);
      case 'firstName':
        return this.validateRequired(fieldName);
      case 'lastName':
        return this.validateRequired(fieldName);
      case 'mobilePhone':
        return this.validateMobilePhone();
      default:
        return true;
    }
  }

  public async validateRequired(fieldName: string) {
    this.resetValidationState();
    const value = this.generalInfo[fieldName as keyof GeneralInfo];
    console.log('validateRequired 1', fieldName, value);
    if (!value) {
      console.log('validateRequired 1', fieldName, value);
      return (this.validationState = new ValidationResult(fieldName, 'RequiredField'));
    }

    return true;
  }

  public async validateMobilePhone() {
    this.resetValidationState();
    const value = this.generalInfo.mobilePhone;
    if (!value?.split('-')[1]) {
      return (this.validationState = new ValidationResult('mobilePhone', 'RequiredField'));
    }

    return true;
  }


  public async validateUsername() {
    this.resetValidationState();
    const { userName } = this.generalInfo;

    if (!userName) {
      return (this.validationState = new ValidationResult('userName', 'RequiredField'));
    }

    const isExisted = await SettingService.CheckExistedCustomer(userName);
    if (isExisted) {
      return (this.validationState = new ValidationResult('userName', 'usernameIsInvalid'));
    }

    return true;
  }

  public validatePassword() {
    this.resetValidationState();
    const { password } = this.generalInfo;

    if (!password) {
      return (this.validationState = new ValidationResult('password', 'RequiredField'));
    }

    if (appConfig.systemId !== SystemIds.NAP && !this.isValidPassword(password)) {
      return (this.validationState = new ValidationResult('password', 'PasswordInCorrectFormat'));
    }

    return true;
  }

  public validateMemberMaxCreditWhenCreateAgent(isEditMode = false): ValidationResult | boolean {
    this.resetValidationState();
    const { maxCredit, memberMaxCredit, memberMaxCreditMin } = this.generalInfo;

    if (memberMaxCredit === undefined) {
      return (this.validationState = new ValidationResult('memberMaxCredit', 'RequiredField'));
    }

    if (memberMaxCredit === 0) {
      return true;
    }

    const idEditAgent = isEditMode && memberMaxCreditMin !== undefined;
    if (idEditAgent && (memberMaxCredit > maxCredit || memberMaxCredit < memberMaxCreditMin)) {
      return (this.validationState = new ValidationResult('memberMaxCredit', 'memberMaxCreditInvalid', {
        min: NumberHelper.roundDownNumber(memberMaxCreditMin),
        max: NumberHelper.roundDownNumber(maxCredit),
      }));
    }

    if (!isEditMode && memberMaxCredit > maxCredit) {
      return (this.validationState = new ValidationResult('memberMaxCredit', 'memberMaxCreditInvalid', {
        min: '0',
        max: NumberHelper.roundDownNumber(maxCredit),
      }));
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
      const max = uplineMemberMaxCredit ? Math.min(uplineMaxCredit, uplineMemberMaxCredit) : uplineMaxCredit;
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

  private isValidPassword(password: string) {
    if (this.generalInfo.roleId === UserLevel.Member) {
      const regex = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,10}$/;
      const whiteSpace = /^\S*$/;

      const case5 = password.search(whiteSpace) !== -1;

      return password.search(regex) !== -1 && case5;
    }

    const whiteSpace = /^\S*$/;
    const whiteSpaceCase = password.search(whiteSpace) !== -1;

    const regex1 = /^(?=.{8,100}$).*\d/i;
    const regex2 = /^(?=.{8,100}$).*[a-z]/;
    const regex3 = /^(?=.{8,100}$).*[A-Z]/;
    // eslint-disable-next-line no-useless-escape
    const regex4 = /^(?=.{8,100}$).*[\[\]\^\$\.\|\?\*\+\(\)\\~`\!@#%&\-_+={}'""<>:;,]/i;

    const case1 = password.search(regex1) !== -1 ? 1 : 0;
    const case2 = password.search(regex2) !== -1 ? 1 : 0;
    const case3 = password.search(regex3) !== -1 ? 1 : 0;
    const case4 = password.search(regex4) !== -1 ? 1 : 0;

    return case1 + case2 + case3 + case4 >= 3 && whiteSpaceCase;
  }
}
