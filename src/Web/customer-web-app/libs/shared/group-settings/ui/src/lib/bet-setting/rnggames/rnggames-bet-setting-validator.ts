import {
  GroupSettingItem,
  RNGGamesBetSettingGroupItem,
  RNGGamesBetSettingProps,
} from '@customer-web-app/shared-group-settings-types';
import { NumberHelper } from '@customer-web-app/shared-helpers';

import BetSettingBaseValidator, { ValidateBetSetting } from '../bet-setting-base-validator';

export default class RNGGamesBetSettingValidator extends BetSettingBaseValidator<RNGGamesBetSettingProps> {
  private maxPerBall?: number;

  constructor(
    betSettingItem: RNGGamesBetSettingGroupItem & GroupSettingItem<RNGGamesBetSettingProps>,
    isSpecialSite: boolean
  ) {
    super(betSettingItem, isSpecialSite);
    this.maxPerBall = betSettingItem.maxPerBall;
  }

  public validateBetSettingItem(): { [key: string]: ValidateBetSetting } {
    const id = `${this.sportTypeId}-${this.betTypeId}`;
    const isValidBetSettingItem = this.isSpecialSite ? false : this.isUnlimitedOrPreventBetValue();

    return {
      [`minBet-${id}`]: isValidBetSettingItem ? new ValidateBetSetting(true) : this.validateMinBet(),
      [`maxBet-${id}`]: isValidBetSettingItem ? new ValidateBetSetting(true) : this.validateMaxBet(),
      [`maxPerMatch-${id}`]: isValidBetSettingItem ? new ValidateBetSetting(true) : this.validateMaxPerMatch(),
      [`maxPerBall-${id}`]: isValidBetSettingItem ? new ValidateBetSetting(true) : this.validateMaxPerBall(),
    };
  }

  private isUnlimitedOrPreventBetValue() {
    if (this.maxPerBall === null) {
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

    const betSettings = JSON.stringify([this.minBet, this.maxBet, this.maxPerMatch, this.maxPerBall]);
    return (
      betSettings === JSON.stringify([0, 0, 0, 0]) ||
      betSettings === JSON.stringify([0, 0.1, 0.1, 0.1]) ||
      (this.minBet === 0 &&
        this.maxPerBall !== undefined &&
        this.upline.maxPerBall !== undefined &&
        this.maxBet <= this.upline.maxBet &&
        this.maxPerMatch <= this.upline.maxPerMatch &&
        this.maxPerBall <= this.upline.maxPerBall &&
        this.maxBet <= this.maxPerMatch &&
        this.maxPerBall >= this.maxBet &&
        this.maxPerBall <= this.maxPerMatch) ||
      (this.minBet === 0 &&
        this.maxBet === 0 &&
        this.maxPerBall !== undefined &&
        this.upline.maxPerBall !== undefined &&
        this.maxPerMatch <= this.upline.maxPerMatch &&
        this.maxPerBall <= this.upline.maxPerBall &&
        this.maxPerBall <= this.maxPerMatch) ||
      (this.minBet === 0 &&
        this.maxBet === 0 &&
        this.maxPerBall === 0 &&
        this.maxPerMatch !== undefined &&
        this.upline.maxPerMatch !== undefined &&
        this.maxPerMatch <= this.upline.maxPerMatch)
    );
  }

  public validateMaxPerMatch(): ValidateBetSetting {
    const fieldName = `maxPerMatch-${this.sportTypeId}-${this.betTypeId}`;
    if (this.maxPerMatch === undefined) return new ValidateBetSetting(false, fieldName, 'RequiredField');

    if (this.upline.maxPerMatch !== 0 && this.maxPerMatch > this.upline.maxPerMatch)
      return new ValidateBetSetting(false, fieldName, 'maxPerMatchMustBeLessThanOrEqualTo', {
        value: NumberHelper.formatWithCommas(this.upline.maxPerMatch),
      });

    if (this.maxPerBall !== undefined && this.maxPerBall !== null && this.maxPerMatch < this.maxPerBall)
      return new ValidateBetSetting(false, fieldName, 'maxPerMatchMustBeGreaterThanOrEqualToMaxPerBall');

    if (this.maxBet !== undefined && this.maxBet > this.maxPerMatch)
      return new ValidateBetSetting(false, fieldName, 'maxPerMatchMustBeGreaterThanOrEqualToMaxBet');

    return new ValidateBetSetting(true);
  }

  public validateMaxPerBall(): ValidateBetSetting {
    const fieldName = `maxPerBall-${this.sportTypeId}-${this.betTypeId}`;
    if (this.maxPerBall === undefined) return new ValidateBetSetting(false, fieldName, 'RequiredField');

    if (this.upline.maxPerBall && this.upline.maxPerBall !== 0 && this.maxPerBall > this.upline.maxPerBall)
      return new ValidateBetSetting(false, fieldName, 'maxPerBallMustBeLessThanOrEqualTo', {
        value: NumberHelper.formatWithCommas(this.upline.maxPerBall),
      });

    if (this.maxBet !== undefined && this.maxPerBall !== null && this.maxBet > this.maxPerBall)
      return new ValidateBetSetting(false, fieldName, 'maxPerBallMustBeGreaterThanOrEqualToMaxBet');

    if (this.maxPerMatch !== undefined && this.maxPerBall > this.maxPerMatch && this.maxPerBall !== null)
      return new ValidateBetSetting(false, fieldName, 'maxPerBallMustBeLessThanOrEqualToMaxPerMatch');

    return new ValidateBetSetting(true);
  }
}
