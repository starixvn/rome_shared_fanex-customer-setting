// import { IBetSettingValidator, Operator, RNGGamesValue } from './type';
// import { getRNGGamesValue, validateBetSettingValue } from './utils';
import { RNGGamesBetSettingGroupItem, RNGGamesBetSettingProps } from 'libs/shared/group-settings/types/src/lib/group-item';

export default class RNGGamesValidator implements IBetSettingValidator<RNGGamesBetSettingProps> {
  private is789y: boolean;
  private betSettingConfigs: RNGGamesBetSettingGroupItem;
  private betSettings: RNGGamesValue;
  private error: string = '';

  constructor(is789y: boolean, configs: RNGGamesBetSettingGroupItem, settings: RNGGamesBetSettingGroupItem) {
    this.is789y = is789y;
    this.betSettingConfigs = configs;
    this.betSettings = getRNGGamesValue(settings);
  }

  private setError(error: string) {
    if (error !== '' && this.error === '') {
      this.error = error;
    }

    return this;
  }

  private getError() {
    return this.error;
  }

  private isUnlimitedOrPreventBetValue() {
    const { minBet, maxBet, maxPerBall, maxPerMatch } = this.betSettings;
    if (maxPerBall === null) {
      const betSettings = JSON.stringify([minBet, maxBet, maxPerMatch]);
      return betSettings === JSON.stringify([0, 0, 0]) || betSettings === JSON.stringify([0, 0.1, 0.1]);
    }

    const betSettings = JSON.stringify([minBet, maxBet, maxPerMatch, maxPerBall]);
    return betSettings === JSON.stringify([0, 0, 0, 0]) || betSettings === JSON.stringify([0, 0.1, 0.1, 0.1]);
  }

  private validateMinBet(prop?: keyof RNGGamesBetSettingGroupItem) {
    if (this.error || (prop && prop !== 'minBet')) return this;
    const { minBet, maxBet } = this.betSettings;
    const error = validateBetSettingValue(this.is789y, {
      current: { name: 'minBet', value: minBet },
      config: { operator: Operator.LessEqual, value: this.betSettingConfigs.minBet },
      max: { name: 'maxBet', value: maxBet },
    });
    if (error) return this.setError(error);
    return this;
  }

  private validateMaxBet(prop?: keyof RNGGamesBetSettingGroupItem) {
    if (this.error || (prop && prop !== 'maxBet')) return this;
    const { minBet, maxBet, maxPerMatch, maxPerBall } = this.betSettings;
    const error = validateBetSettingValue(this.is789y, {
      current: { name: 'maxBet', value: maxBet },
      config: { operator: Operator.GreaterEqual, value: this.betSettingConfigs.maxBet },
      max:
        maxPerBall !== null ? { name: 'maxPerBall', value: maxPerBall } : { name: 'maxPerMatch', value: maxPerMatch },
      min: { name: 'minBet', value: minBet },
    });
    if (error) return this.setError(error);

    return this;
  }

  private validateMaxPerBall(prop?: keyof RNGGamesBetSettingGroupItem) {
    if (this.error || (prop && prop !== 'maxPerBall')) return this;
    const { maxPerBall, maxBet, maxPerMatch } = this.betSettings;
    if (maxPerBall === null) return this;
    const error = validateBetSettingValue(this.is789y, {
      current: { name: 'maxPerBall', value: maxPerBall },
      config: { operator: Operator.GreaterEqual, value: this.betSettingConfigs.maxPerBall },
      max: maxPerMatch !== null ? { name: 'maxPerMatch', value: maxPerMatch } : undefined,
      min: { name: 'maxBet', value: maxBet },
    });
    if (error) return this.setError(error);
    return this;
  }

  private validateMaxPerMatch(prop?: keyof RNGGamesBetSettingGroupItem) {
    if (this.error || (prop && prop !== 'maxPerMatch')) return this;
    const { maxPerMatch, maxPerBall, maxBet } = this.betSettings;
    const error = validateBetSettingValue(this.is789y, {
      current: { name: 'maxPerMatch', value: maxPerMatch },
      config: { operator: Operator.GreaterEqual, value: this.betSettingConfigs.maxPerMatch },
      min: maxPerBall !== null ? { name: 'maxPerBall', value: maxPerBall } : { name: 'maxBet', value: maxBet },
    });
    if (error) return this.setError(error);
    return this;
  }

  validate(prop?: keyof RNGGamesBetSettingGroupItem): string {
    this.error = '';
    if (!this.is789y && this.isUnlimitedOrPreventBetValue()) return this.error;

    return this.validateMinBet(prop).validateMaxBet(prop).validateMaxPerMatch(prop).validateMaxPerBall(prop).getError();
  }
}
