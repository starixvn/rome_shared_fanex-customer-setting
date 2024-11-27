import { ChangeEvent, useEffect, useState } from 'react';

import { GroupTypeIds, MaxNumberInputLength, ProductGroupIds, UserLevel } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { RNGGamesBetSettingGroupItem, RNGGamesBetSettingProps } from '@customer-web-app/shared-group-settings-types';
import { NumberHelper } from '@customer-web-app/shared-helpers';
import { BlockColumn, BlockRow } from '@customer-web-app/shared/base/block';

import { Operator } from '../../operator';
import { ValidateBetSetting } from '../bet-setting-base-validator';
import BetSettingCell from '../bet-setting-cell';
import RNGGamesBetSettingValidator from './rnggames-bet-setting-validator';

const fieldNames: ReadonlyArray<keyof RNGGamesBetSettingProps> = ['minBet', 'maxBet', 'maxPerMatch', 'maxPerBall'];

interface BetSettingRowProps {
  label: string | React.ReactNode;
  settingItem: RNGGamesBetSettingGroupItem;
  isCopied?: boolean;
  isReadOnly?: boolean;
  validationState?: ValidateBetSetting;
  setErrorTooltip: (validationState: ValidateBetSetting) => void;
}

export default function RNGGamesBetSettingRow({
  label,
  settingItem,
  isCopied = false,
  isReadOnly = false,
  validationState,
  setErrorTooltip,
}: BetSettingRowProps) {
  const { handleSettingItemChange, generalInfo, roleId } = useGroupSettingContext();
  const [itemValidateResult, setItemValidateResult] = useState<{ [key: string]: ValidateBetSetting }>({});
  const isSpecialSite = generalInfo?.isSpecialSite ?? false;
  const { sportTypeId, betTypeId, upline } = settingItem;

  useEffect(() => {
    const validator = new RNGGamesBetSettingValidator({ ...settingItem }, isSpecialSite);
    const validateResult = validator.validateBetSettingItem();
    setItemValidateResult(validateResult);
    if (
      validationState &&
      validateResult[validationState.fieldName] &&
      validateResult[validationState.fieldName].isValid
    ) {
      setErrorTooltip(new ValidateBetSetting(true));
    }
  }, [isSpecialSite, setErrorTooltip, settingItem, validationState]);

  const handleBetSettingChange = (fieldName: string, id: string, event: ChangeEvent<HTMLInputElement>) => {
    const data = { [fieldName]: NumberHelper.parseNumber(event.target.value) };
    handleSettingItemChange?.(ProductGroupIds.RNGGames, GroupTypeIds.RNGGamesBetSetting, sportTypeId, betTypeId, data);
    const validator = new RNGGamesBetSettingValidator({ ...settingItem, ...data }, isSpecialSite);
    const validateResult = validator.validateBetSettingItem();
    setItemValidateResult(validateResult);
    const errorTooltip = validateResult && !validateResult[id].isValid ? validateResult[id] : undefined;
    if (errorTooltip) {
      setErrorTooltip(errorTooltip);
    } else {
      setErrorTooltip(new ValidateBetSetting(true));
    }
  };

  const renderBetSettingItem = (
    betSettingItem: RNGGamesBetSettingGroupItem,
    fieldName: keyof RNGGamesBetSettingProps
  ) => {
    if ((betSettingItem.original || betSettingItem)[fieldName] == null) return null;

    const id = `${fieldName}-${sportTypeId}-${betTypeId}`;
    return isReadOnly ? (
      <div className="show-value">{settingItem[fieldName]}</div>
    ) : (
      <BetSettingCell
        key={id}
        id={id}
        value={settingItem[fieldName]}
        isDisable={
          !settingItem.isMultipleModeChecked
          || (roleId===UserLevel.Super && !(settingItem as any)[fieldName + 'MultipleMode'])
        }
        operator={fieldName === 'minBet' ? Operator['>='] : Operator['<=']}
        limit={upline[fieldName]}
        isError={itemValidateResult[id] && !itemValidateResult[id].isValid}
        onChange={(e) => handleBetSettingChange(fieldName, id, e)}
        isCopied={isCopied}
        onFocus={() => {
          const result = itemValidateResult && !itemValidateResult[id].isValid ? itemValidateResult[id] : undefined;
          if (result) {
            setErrorTooltip(result);
          }
        }}
        onBlur={() => setErrorTooltip(new ValidateBetSetting(true))}
        maxLength={MaxNumberInputLength}
      />
    );
  };

  return (
    <BlockRow label={label}>
      {fieldNames.map((fieldName) => {
        const id = `${fieldName}-${sportTypeId}-${betTypeId}`;
        return <BlockColumn key={id}>{renderBetSettingItem(settingItem, fieldName)}</BlockColumn>;
      })}
    </BlockRow>
  );
}
