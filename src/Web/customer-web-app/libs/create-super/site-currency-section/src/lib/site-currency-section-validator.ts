import { Result, ValidationResult } from '@customer-web-app/share-models';
import { TFunction } from 'i18next';

export interface SiteCurrencySectionValidatorProps {
  site?: string;
  currency?: string;
}

export default class SiteCurrencySectionValidator {
  protected translate: TFunction;
  public validationState: ValidationResult;
  protected site?: string;
  protected currency?: string;

  constructor(props: SiteCurrencySectionValidatorProps, t: TFunction) {
    this.validationState = Result.CreateSuccessfulResult() as ValidationResult;
    this.site = props.site;
    this.currency = props.currency;
    this.translate = t;
  }

  public validate() {
    return {
      [`site`]: this.validateSite(),
      [`currency`]: this.validateCurrency(),
    };
  }

  public validateField(fieldName: string) {
    switch (fieldName) {
      case 'site':
        return this.validateSite();
      case 'currency':
        return this.validateCurrency();
    }

    return true;
  }

  public validateSite() {
    this.resetValidationState();
    if (this.site === undefined || this.site === '') {
      this.validationState = new ValidationResult('site', this.translate('alertSite'));
      return this.validationState;
    }

    return true;
  }

  public validateCurrency() {
    this.resetValidationState();

    if (this.currency === undefined || this.currency === '') {
      this.validationState = new ValidationResult('currency', this.translate('alertCurrency'));
      return this.validationState;
    }
    return true;
  }

  public resetValidationState(): void {
    this.validationState = Result.CreateSuccessfulResult() as ValidationResult;
  }
}
