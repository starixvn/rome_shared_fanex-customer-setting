import { TFunction } from 'i18next';

import { Result, ValidationResult } from '@customer-web-app/share-models';
import { UserLevel } from '@customer-web-app/shared-constants';
import {
    ParlayMaxPayoutGroupItem, ParlayMaxPayoutKey, ParlayMaxPayoutProps
} from '@customer-web-app/shared-group-settings-types';

const ParlayMaxPayoutMissConfigValue = 0.1;

export default class ParlayMaxPayoutValidator {
  protected combo2: number;
  protected combo35: number;
  protected comboOver6: number;
  protected singleMatchParlay: number;

  protected isAllowUnlimitSingleMatchParlay: boolean;
  protected isAllowUnlimitCombo2: boolean;
  protected isAllowUnlimitCombo35: boolean;
  protected isAllowUnlimitComboOver6: boolean;

  protected sportTypeId: number;
  protected betTypeId: number;
  protected upline: ParlayMaxPayoutProps;
  protected original: ParlayMaxPayoutProps;

  protected translate: TFunction;
  public validationState: ValidationResult;
  public uplineRoleId: number | undefined;
  public uplineIsCompany: boolean;

  constructor(props: ParlayMaxPayoutGroupItem, t: TFunction, uplineRoleId: number | undefined) {
    this.validationState = Result.CreateSuccessfulResult() as ValidationResult;
    this.combo2 = props.combo2;
    this.combo35 = props.combo35;
    this.comboOver6 = props.comboOver6;
    this.singleMatchParlay = props.singleMatchParlay;

    this.isAllowUnlimitSingleMatchParlay = props.isAllowUnlimitSingleMatchParlay;
    this.isAllowUnlimitCombo2 = props.isAllowUnlimitCombo2;
    this.isAllowUnlimitCombo35 = props.isAllowUnlimitCombo35;
    this.isAllowUnlimitComboOver6 = props.isAllowUnlimitComboOver6;

    this.translate = t;
    this.sportTypeId = props.sportTypeId;
    this.betTypeId = props.betTypeId;
    this.upline = props.upline;
    this.original = props.original;

    this.uplineRoleId = uplineRoleId;
    this.uplineIsCompany = uplineRoleId === UserLevel.Company;
  }

  public validateAll() {
    return {
      [`singleMatchParlay-${this.sportTypeId}-${this.betTypeId}`]: this.validateSingleMatchParlay(),
      [`combo2-${this.sportTypeId}-${this.betTypeId}`]: this.validateCombo2(),
      [`combo35-${this.sportTypeId}-${this.betTypeId}`]: this.validateCombo35(),
      [`comboOver6-${this.sportTypeId}-${this.betTypeId}`]: this.validateComboOver6(),
    };
  }

  public validateField(fieldName: ParlayMaxPayoutKey) {
    switch (fieldName) {
      case 'singleMatchParlay':
        return this.validateSingleMatchParlay();
      case 'combo2':
        return this.validateCombo2();
      case 'combo35':
        return this.validateCombo35();
      case 'comboOver6':
        return this.validateComboOver6();
    }

    return true;
  }
  public validate(fieldName: ParlayMaxPayoutKey, booleanFieldCheckBoxUnlimited: ParlayMaxPayoutKey) {
    this.resetValidationState();
    const uiFieldLocation = `${fieldName}-${this.sportTypeId}-${this.betTypeId}`;
    // not validate when unlimitedCheckbox checked
    if (this[booleanFieldCheckBoxUnlimited]) {
      return true;
    }

    if (this[fieldName] === undefined) {
      return (this.validationState = new ValidationResult(uiFieldLocation, this.translate('RequiredField')));
    }

    if (this[fieldName] === 0 || this[fieldName] === ParlayMaxPayoutMissConfigValue) {
      return (this.validationState = new ValidationResult(
        uiFieldLocation,
        this.translate('theValueMustBeAPositiveAmount')
      ));
    }

    // Compare to upline value
    // 1- when upline value != 0 (this mean upline not allow unlimited)
    if (this.upline && this.upline[fieldName] !== 0 && (this[fieldName] ?? 0) > this.upline[fieldName]) {
      const message = this.uplineIsCompany
        ? 'theSettingOfNameMustBeLessThanOrEqualToCompany'
        : 'theSettingOfNameMustBeLessThanOrEqualToUpLine';
      return (this.validationState = new ValidationResult(
        uiFieldLocation,
        this.translate(message, { name: this.translate(fieldName) })
      ));
    }

    return true;
  }
  public validateSingleMatchParlay() {
    return this.validate('singleMatchParlay', 'isAllowUnlimitSingleMatchParlay');
  }

  public validateCombo2() {
    return this.validate('combo2', 'isAllowUnlimitCombo2');
  }

  public validateCombo35() {
    return this.validate('combo35', 'isAllowUnlimitCombo35');
  }

  public validateComboOver6() {
    return this.validate('comboOver6', 'isAllowUnlimitComboOver6');
  }

  public resetValidationState(): void {
    this.validationState = Result.CreateSuccessfulResult() as ValidationResult;
  }
}
