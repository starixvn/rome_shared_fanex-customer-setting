import { TFunction } from 'i18next';

import { Result, ValidationResult } from '@customer-web-app/share-models';
import {
  PotentialWinLimitGroupItem,
  PotentialWinLimitKey,
  PotentialWinLimitProps
} from '@customer-web-app/shared-group-settings-types';

export default class PotentialWinLimitValidator {
  protected winLimit: number;

  protected isAllowUnlimitWinLimit: boolean;

  protected sportTypeId: number;
  protected betTypeId: number;
  protected upline: PotentialWinLimitProps;
  protected original: PotentialWinLimitProps;

  protected translate: TFunction;
  public validationState: ValidationResult;

  constructor(props: PotentialWinLimitGroupItem, t: TFunction, uplineRoleId: number | undefined) {
    this.validationState = Result.CreateSuccessfulResult() as ValidationResult;
    this.winLimit = props.winLimit;

    this.isAllowUnlimitWinLimit = props.isAllowUnlimitWinLimit;    

    this.translate = t;
    this.sportTypeId = props.sportTypeId;
    this.betTypeId = props.betTypeId;
    this.upline = props.upline;
    this.original = props.original;
  }

  public validateAll() {
    return {
      [`winLimit-${this.sportTypeId}-${this.betTypeId}`]: this.validate()      
    };
  }

  public validateField(fieldName: PotentialWinLimitKey) {
    switch (fieldName) {
      case 'winLimit':
        return this.validate();      
    }

    return true;
  }
  
  public validate() {
    this.resetValidationState();
    const uiFieldLocation = `winLimit-${this.sportTypeId}-${this.betTypeId}`;
    // not validate when unlimitedCheckbox checked
    if (this.isAllowUnlimitWinLimit) {
      return true;
    }

    if (this.winLimit === undefined) {
      return (this.validationState = new ValidationResult(uiFieldLocation, this.translate('RequiredField')));
    }

    if (this.winLimit === 0) {
      return (this.validationState = new ValidationResult(
        uiFieldLocation,
        this.translate('theValueMustBeAPositiveAmount')
      ));
    }

    return true;
  }
  
  public resetValidationState(): void {
    this.validationState = Result.CreateSuccessfulResult() as ValidationResult;
  }
}
