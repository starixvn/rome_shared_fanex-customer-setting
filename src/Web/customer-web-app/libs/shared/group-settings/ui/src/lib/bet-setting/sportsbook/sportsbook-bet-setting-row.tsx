import React, { ChangeEvent, useEffect, useState } from 'react';

import { GroupTypeIds, MaxNumberInputLength, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import {
  SportsBookBetSettingGroupItem,
  SportsBookBetSettingProps,
} from '@customer-web-app/shared-group-settings-types';
import { NumberHelper } from '@customer-web-app/shared-helpers';
import { BlockColumn, BlockRow } from '@customer-web-app/shared/base/block';

import { Operator } from '../../operator';
import { ValidateBetSetting } from '../bet-setting-base-validator';
import BetSettingCell from '../bet-setting-cell';
import SportsBookBetSettingValidator from './sportsbook-bet-setting-validator';

const fieldNames: ReadonlyArray<keyof SportsBookBetSettingProps> = [
  'minBet',
  'maxBet',
  'maxPerMatch',
  'maxPayoutPerMatch',
];

interface SportsBookBetSettingRowProps {
  label: string | React.ReactNode;
  settingItem: SportsBookBetSettingGroupItem;
  isCopied?: boolean;
  isReadOnly?: boolean;
  validationState?: ValidateBetSetting;
  setErrorTooltip: (validationState: ValidateBetSetting) => void;
}

export function SportsBookBetSettingRow({
  label,
  settingItem,
  isCopied = false,
  isReadOnly = false,
  validationState,
  setErrorTooltip,
}: Readonly<SportsBookBetSettingRowProps>) {
  const { handleSettingItemChange, generalInfo } = useGroupSettingContext();
  const [itemValidateResult, setItemValidateResult] = useState<{ [key: string]: ValidateBetSetting }>({});
  const isSpecialSite = generalInfo?.isSpecialSite ?? false;
  const { sportTypeId, betTypeId, upline } = settingItem;

  useEffect(() => {
    const validator = new SportsBookBetSettingValidator({ ...settingItem }, isSpecialSite);
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
    handleSettingItemChange?.(
      ProductGroupIds.SportsBook,
      GroupTypeIds.SportsBookBetSetting,
      sportTypeId,
      betTypeId,
      data
    );
    const validator = new SportsBookBetSettingValidator({ ...settingItem, ...data }, isSpecialSite);
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
    betSettingItem: SportsBookBetSettingGroupItem,
    fieldName: keyof SportsBookBetSettingProps
  ) => {
    if (betSettingItem[fieldName] === null) return null;

    const id = `${fieldName}-${sportTypeId}-${betTypeId}`;
    return isReadOnly ? (
      <div className="show-value">{settingItem[fieldName]}</div>
    ) : (
      <BetSettingCell
        key={id}
        id={id}
        value={settingItem[fieldName]}
        isDisable={!settingItem.isMultipleModeChecked}
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
