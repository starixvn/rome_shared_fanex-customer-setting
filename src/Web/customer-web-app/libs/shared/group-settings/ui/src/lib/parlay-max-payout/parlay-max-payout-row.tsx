import { useContext, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { ValidationResult } from '@customer-web-app/share-models';
import { GroupTypeIds, ProductGroupIds, UserLevel } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import {
    ColumnsParlayMaxPayout, ParlayMaxPayoutGroupItem, ParlayMaxPayoutKey
} from '@customer-web-app/shared-group-settings-types';
import { BlockColumn, BlockRow } from '@customer-web-app/shared/base/block';

import { ValidationContext } from '../bet-setting/validation-context';
import { ParlayMaxPayoutCell } from './parlay-max-payout-cell';
import ParlayMaxPayoutValidator from './parlay-max-payout-validator';

interface ParlayMaxPayoutRowProps {
  parlayMaxPayoutItem: ParlayMaxPayoutGroupItem;
  uplineRoleId: number | undefined;
  setErrorTooltip: (validationState: ValidationResult | undefined) => void;
  isReadOnly: boolean;
}

type CacheParlayMaxPayoutItem = {
  sportTypeId: number;
  betTypeId: number;
  value: Partial<ParlayMaxPayoutGroupItem>;
};

const ParlayMaxPayoutRow = (props: ParlayMaxPayoutRowProps) => {
  const { t } = useTranslation();
  const { parlayMaxPayoutItem, uplineRoleId, setErrorTooltip } = props;
  const { sportTypeId, betTypeId, displayName, upline, isMultipleModeChecked } = parlayMaxPayoutItem;
  const { handleSettingItemChange, isMultipleMode, roleId } = useGroupSettingContext();
  const { setValidationState } = useContext(ValidationContext);
  const [itemValidateResult, setItemValidateResult] = useState<{ [key: string]: ValidationResult | boolean }>({});
  const [cacheValues, setCacheValues] = useState<ParlayMaxPayoutGroupItem[]>([parlayMaxPayoutItem]);
  useEffect(() => {
    const validator = new ParlayMaxPayoutValidator({ ...parlayMaxPayoutItem }, t, uplineRoleId);
    setItemValidateResult(validator.validateAll());
  }, [parlayMaxPayoutItem, t, uplineRoleId]);
  const { isReadOnly } = props;

  const onParlayMaxPayoutChange = (
    sportTypeId: number,
    betTypeId: number,
    data: Partial<ParlayMaxPayoutGroupItem>,
    validateFieldName: ParlayMaxPayoutKey,
    cachedObject?: CacheParlayMaxPayoutItem
  ) => {
    if (cachedObject) {
      const newCacheValues = cacheValues?.map((e) => {
        if (e.sportTypeId === sportTypeId && e.betTypeId === betTypeId) {
          return { ...e, ...cachedObject.value };
        }
        return e;
      });
      setCacheValues(newCacheValues);
    }
    handleSettingItemChange?.(ProductGroupIds.SportsBook, GroupTypeIds.ParlayMaxPayout, sportTypeId, betTypeId, data);

    const validator = new ParlayMaxPayoutValidator({ ...parlayMaxPayoutItem, ...data }, t, uplineRoleId);
    const result = validator.validateField(validateFieldName);
    setValidationState?.(validator.validationState);
  };

  const renderBlockRowTitle = (
    isChecked: boolean,
    blockRowTitle: string | undefined,
    onChange: (isChecked: boolean) => void
  ) => {
    return (
      <div>
        <input type="checkbox" onChange={(e) => onChange(e.target.checked)} checked={isChecked} />
        <span>{blockRowTitle}</span>
      </div>
    );
  };
  return (
    <BlockRow
      label={
        isMultipleMode && !isReadOnly
          ? renderBlockRowTitle(isMultipleModeChecked, displayName, (isChecked) =>
              handleSettingItemChange?.(
                ProductGroupIds.SportsBook,
                GroupTypeIds.ParlayMaxPayout,
                sportTypeId,
                betTypeId,
                { isMultipleModeChecked: isChecked }
              )
            )
          : displayName
      }
    >
      {ColumnsParlayMaxPayout.map((column) => {
        const { inputFieldName, checkBoxFieldName } = column;
        const id = `${inputFieldName}-${sportTypeId}-${betTypeId}`;
        const isErr = itemValidateResult[id] && itemValidateResult[id] !== true;
        return (
          <BlockColumn key={id}>
            {parlayMaxPayoutItem[inputFieldName] !== null &&
              (isReadOnly ? (
                <div className="show-value">{parlayMaxPayoutItem[inputFieldName]}</div>
              ) : (
                <ParlayMaxPayoutCell
                  key={id}
                  isDisable={
                    !parlayMaxPayoutItem.isMultipleModeChecked
                    || (roleId===UserLevel.Super && !(parlayMaxPayoutItem as any)[inputFieldName + 'MultipleMode'])
                  }
                  // maxValue of downline is flow upline
                  maxValue={upline ? upline[inputFieldName] : 0}
                  currentValue={parlayMaxPayoutItem[inputFieldName]}
                  currentValueUnlimited={parlayMaxPayoutItem[checkBoxFieldName]}
                  originalIsUplineUnlimited={parlayMaxPayoutItem.original[checkBoxFieldName]}
                  onChange={(value) => {
                    onParlayMaxPayoutChange(sportTypeId, betTypeId, { [inputFieldName]: value }, inputFieldName);
                  }}
                  onChangeUnlimited={(valueCheckBox, value) => {
                    if (valueCheckBox) {
                      onParlayMaxPayoutChange(
                        sportTypeId,
                        betTypeId,
                        {
                          [checkBoxFieldName]: valueCheckBox,
                          [inputFieldName]: value,
                        },
                        inputFieldName,
                        {
                          sportTypeId,
                          betTypeId,
                          value: {
                            [inputFieldName]: parlayMaxPayoutItem[inputFieldName],
                          },
                        }
                      );
                    } else {
                      const cachedValue = cacheValues?.find(
                        (e) => e.sportTypeId === sportTypeId && e.betTypeId === betTypeId
                      );
                      if (!cachedValue) {
                        return;
                      }
                      onParlayMaxPayoutChange(
                        sportTypeId,
                        betTypeId,
                        {
                          [checkBoxFieldName]: valueCheckBox,
                          [inputFieldName]: cachedValue[inputFieldName],
                        },
                        inputFieldName
                      );
                    }
                  }}
                  componentId={id}
                  isError={isErr}
                  onFocus={() => {
                    const result = isErr ? (itemValidateResult[id] as ValidationResult) : undefined;
                    setErrorTooltip(result);
                  }}
                  onBlur={() => setErrorTooltip(undefined)}
                />
              ))}
          </BlockColumn>
        );
      })}
    </BlockRow>
  );
};

export default ParlayMaxPayoutRow;
