import { BetSettingsProps, GroupSettingItem } from '@customer-web-app/shared-group-settings-types';
import { NumberHelper } from '@customer-web-app/shared-helpers';

export class ValidateBetSetting {
  public isValid: boolean;
  public fieldName: string;
  public message?: string;
  public msgParams?: { [key: string]: string };

  constructor(isValid: boolean, fieldName = '', message?: string, msgParams?: { [key: string]: string }) {
    this.isValid = isValid;
    this.fieldName = fieldName;
    this.message = message;
    this.msgParams = msgParams;
  }
}

export default class BetSettingBaseValidator<T extends BetSettingsProps> {
  protected sportTypeId: number;
  protected betTypeId: number;
  protected minBet: number;
  protected maxBet: number;
  protected maxPerMatch: number;
  protected upline: T;
  protected $minConstraintBySystem: number;
  protected isSpecialSite: boolean;

  constructor(betSettingItem: T & GroupSettingItem<T>, isSpecialSite: boolean) {
    this.sportTypeId = betSettingItem.sportTypeId;
    this.betTypeId = betSettingItem.betTypeId;
    this.minBet = betSettingItem.minBet;
    this.maxBet = betSettingItem.maxBet;
    this.maxPerMatch = betSettingItem.maxPerMatch;
    this.upline = betSettingItem.upline;
    this.isSpecialSite = isSpecialSite;

    this.$minConstraintBySystem = this.isSpecialSite ? 1 : 0;
  }

  get $minConstraint() {
    return Math.max(this.$minConstraintBySystem, this.upline?.minBet ?? 0);
  }

  public validateMinBet(): ValidateBetSetting {
    console.log(this.$minConstraint);
    const fieldName = `minBet-${this.sportTypeId}-${this.betTypeId}`;
    if (this.minBet === undefined) return new ValidateBetSetting(false, fieldName, 'RequiredField');

    if (this.minBet < this.$minConstraint)
      return new ValidateBetSetting(false, fieldName, 'minBetMustBeGreaterThanOrEqualTo', {
        value: NumberHelper.formatWithCommas(this.$minConstraint),
      });

    if (this.maxBet !== undefined && this.minBet > this.maxBet)
      return new ValidateBetSetting(false, fieldName, 'minBetMustBeLessThanOrEqualToMaxBet');

    return new ValidateBetSetting(true);
  }

  public validateMaxBet(): ValidateBetSetting {
    const fieldName = `maxBet-${this.sportTypeId}-${this.betTypeId}`;
    if (this.maxBet === undefined) return new ValidateBetSetting(false, fieldName, 'RequiredField');

    if (this.upline.maxBet !== 0 && this.maxBet > this.upline.maxBet)
      return new ValidateBetSetting(false, fieldName, 'maxBetMustBeLessThanOrEqualTo', {
        value: NumberHelper.formatWithCommas(this.upline.maxBet),
      });

    if (this.maxPerMatch !== undefined && this.maxBet > this.maxPerMatch)
      return new ValidateBetSetting(false, fieldName, 'maxBetMustBeLessThanOrEqualToMaxPerMatch');

    if (this.minBet !== undefined && this.maxBet < this.minBet)
      return new ValidateBetSetting(false, fieldName, 'maxBetMustBeGreaterThanOrEqualToMinBet');

    return new ValidateBetSetting(true);
  }

  public validateMaxPerMatch(): ValidateBetSetting {
    const fieldName = `maxPerMatch-${this.sportTypeId}-${this.betTypeId}`;
    if (this.maxPerMatch === undefined) return new ValidateBetSetting(false, fieldName, 'RequiredField');

    if (this.upline.maxPerMatch !== 0 && this.maxPerMatch > this.upline.maxPerMatch)
      return new ValidateBetSetting(false, fieldName, 'maxPerMatchMustBeLessThanOrEqualTo', {
        value: NumberHelper.formatWithCommas(this.upline.maxPerMatch),
      });

    if (this.maxBet !== undefined && this.maxPerMatch < this.maxBet)
      return new ValidateBetSetting(false, fieldName, 'maxPerMatchMustBeGreaterThanOrEqualToMaxBet');

    return new ValidateBetSetting(true);
  }
}
