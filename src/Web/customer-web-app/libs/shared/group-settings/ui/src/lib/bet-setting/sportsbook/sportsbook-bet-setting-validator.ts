import { SportTypeIds } from '@customer-web-app/shared-constants';
import { GroupSettingItem, SportsBookBetSettingProps } from '@customer-web-app/shared-group-settings-types';
import { NumberHelper } from '@customer-web-app/shared-helpers';

import BetSettingBaseValidator, { ValidateBetSetting } from '../bet-setting-base-validator';

export default class SportsBookBetSettingValidator extends BetSettingBaseValidator<SportsBookBetSettingProps> {
  private maxPayoutPerMatch?: number;

  constructor(
    betSettingItem: SportsBookBetSettingProps & GroupSettingItem<SportsBookBetSettingProps>,
    isSpecialSite: boolean
  ) {
    super(betSettingItem, isSpecialSite);

    this.maxPayoutPerMatch = betSettingItem.maxPayoutPerMatch;
    this.$minConstraintBySystem = this.isSpecialBetTypeOfSpecialSite() ? 1 : 0;
  }

  public isESportsOrMuayThai() {
    return [SportTypeIds.ESports, SportTypeIds.MuayThai].includes(this.sportTypeId);
  }

  public isSpecialBetTypeOfSpecialSite() {
    return this.isSpecialSite && this.isESportsOrMuayThai();
  }

  private isUnlimitedOrPreventBetValue() {
    if (this.maxPayoutPerMatch === null) {
      const betSettings = JSON.stringify([this.minBet, this.maxBet, this.maxPerMatch]);
      return (
        betSettings === JSON.stringify([0, 0, 0]) ||
        betSettings === JSON.stringify([0, 0.1, 0.1]) ||
        (this.minBet === 0 &&
          this.maxBet <= this.upline.maxBet &&
          this.maxPerMatch <= this.upline.maxPerMatch &&
          this.maxBet <= this.maxPerMatch) ||
        (this.minBet === 0 && this.maxBet === 0 && this.maxPerMatch <= this.upline.maxPerMatch)
      );
    }

    const betSettings = JSON.stringify([this.minBet, this.maxBet, this.maxPerMatch, this.maxPayoutPerMatch]);
    return (
      betSettings === JSON.stringify([0, 0, 0, 0]) ||
      betSettings === JSON.stringify([0, 0.1, 0.1, 0.1]) ||
      (this.minBet === 0 &&
        this.maxPayoutPerMatch !== undefined &&
        this.upline.maxPayoutPerMatch !== undefined &&
        this.maxBet <= this.upline.maxBet &&
        this.maxPerMatch <= this.upline.maxPerMatch &&
        this.maxPayoutPerMatch <= this.upline.maxPayoutPerMatch &&
        this.maxBet <= this.maxPerMatch &&
        this.maxPerMatch <= this.maxPayoutPerMatch) ||
      (this.minBet === 0 &&
        this.maxBet === 0 &&
        this.maxPayoutPerMatch !== undefined &&
        this.upline.maxPayoutPerMatch !== undefined &&
        this.maxPerMatch <= this.upline.maxPerMatch &&
        this.maxPayoutPerMatch <= this.upline.maxPayoutPerMatch &&
        this.maxPerMatch <= this.maxPayoutPerMatch) ||
      (this.minBet === 0 &&
        this.maxBet === 0 &&
        this.maxPerMatch === 0 &&
        this.maxPayoutPerMatch !== undefined &&
        this.upline.maxPayoutPerMatch !== undefined &&
        this.maxPayoutPerMatch <= this.upline.maxPayoutPerMatch)
    );
  }

  public validateBetSettingItem(): { [key: string]: ValidateBetSetting } {
    const id = `${this.sportTypeId}-${this.betTypeId}`;
    const isValidBetSettingItem = !this.isSpecialBetTypeOfSpecialSite() ? this.isUnlimitedOrPreventBetValue() : false;

    return {
      [`minBet-${id}`]: isValidBetSettingItem ? new ValidateBetSetting(true) : this.validateMinBet(),
      [`maxBet-${id}`]: isValidBetSettingItem ? new ValidateBetSetting(true) : this.validateMaxBet(),
      [`maxPerMatch-${id}`]: isValidBetSettingItem ? new ValidateBetSetting(true) : this.validateMaxPerMatch(),
      [`maxPayoutPerMatch-${id}`]: isValidBetSettingItem
        ? new ValidateBetSetting(true)
        : this.validateMaxPayoutPerMatch(),
    };
  }

  public validateMaxPerMatch(): ValidateBetSetting {
    const fieldName = `maxPerMatch-${this.sportTypeId}-${this.betTypeId}`;
    if (this.maxPerMatch === undefined) return new ValidateBetSetting(false, fieldName, 'RequiredField');

    if (this.upline.maxPerMatch !== 0 && this.maxPerMatch > this.upline.maxPerMatch)
      return new ValidateBetSetting(false, fieldName, 'maxPerMatchMustBeLessThanOrEqualTo', {
        value: NumberHelper.formatWithCommas(this.upline.maxPerMatch),
      });

    if (
      this.maxPayoutPerMatch !== undefined &&
      this.maxPayoutPerMatch !== null &&
      this.maxPerMatch > this.maxPayoutPerMatch
    )
      return new ValidateBetSetting(false, fieldName, 'maxPerMatchMustBeLessThanOrEqualToMaxWinPerMatch');

    if (this.maxBet !== undefined && this.maxBet > this.maxPerMatch)
      return new ValidateBetSetting(false, fieldName, 'maxPerMatchMustBeGreaterThanOrEqualToMaxBet');

    return new ValidateBetSetting(true);
  }

  public validateMaxPayoutPerMatch(): ValidateBetSetting {
    const fieldName = `maxPayoutPerMatch-${this.sportTypeId}-${this.betTypeId}`;
    if (this.maxPayoutPerMatch === undefined) return new ValidateBetSetting(false, fieldName, 'RequiredField');

    if (
      this.upline.maxPayoutPerMatch &&
      this.upline.maxPayoutPerMatch !== 0 &&
      this.maxPayoutPerMatch > this.upline.maxPayoutPerMatch
    )
      return new ValidateBetSetting(false, fieldName, 'maxWinPerMatchMustBeLessThanOrEqualTo', {
        value: NumberHelper.formatWithCommas(this.upline.maxPayoutPerMatch),
      });

    if (this.maxPerMatch !== undefined && this.maxPayoutPerMatch < this.maxPerMatch && this.maxPayoutPerMatch !== null)
      return new ValidateBetSetting(false, fieldName, 'maxWinPerMatchMustBeGreaterThanOrEqualToMaxPerMatch');

    return new ValidateBetSetting(true);
  }
}
