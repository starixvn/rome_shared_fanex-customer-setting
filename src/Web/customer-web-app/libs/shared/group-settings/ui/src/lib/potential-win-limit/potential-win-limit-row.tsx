import { useContext, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { ValidationResult } from '@customer-web-app/share-models';
import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { ColumnsPotentialWinLimit, PotentialWinLimitGroupItem, PotentialWinLimitKey } from '@customer-web-app/shared-group-settings-types';
import { BlockColumn, BlockRow } from '@customer-web-app/shared/base/block';

import { NumberHelper } from '@customer-web-app/shared-helpers';
import { ValidationContext } from '../bet-setting/validation-context';
import { PotentialWinLimitCell } from './potential-win-limit-cell';
import PotentialWinLimitNote from './potential-win-limit-note';
import PotentialWinLimitValidator from './potential-win-limit-validator';

interface PotentialWinLimitRowProps {
  potentialWinLimitItem: PotentialWinLimitGroupItem;
  uplineRoleId: number | undefined;
  setErrorTooltip: (validationState: ValidationResult | undefined) => void;
  isReadOnly: boolean;
}

type CachePotentialWinLimitItem = {
  sportTypeId: number;
  betTypeId: number;
  value: Partial<PotentialWinLimitGroupItem>;
};

const PotentialWinLimitRow = (props: PotentialWinLimitRowProps) => {
  const { t } = useTranslation();
  const { potentialWinLimitItem, uplineRoleId, setErrorTooltip } = props;
  const { sportTypeId, betTypeId, productName, upline, isMultipleModeChecked } = potentialWinLimitItem;
  const { handleSettingItemChange, isMultipleMode, roleId, productGroupId } = useGroupSettingContext();
  const groupId = Number(productGroupId);
  const { setValidationState } = useContext(ValidationContext);
  const [itemValidateResult, setItemValidateResult] = useState<{ [key: string]: ValidationResult | boolean }>({});
  const [cacheValues, setCacheValues] = useState<PotentialWinLimitGroupItem[]>([potentialWinLimitItem]);
  useEffect(() => {
    const validator = new PotentialWinLimitValidator({ ...potentialWinLimitItem }, t, uplineRoleId);
    setItemValidateResult(validator.validateAll());
  }, [potentialWinLimitItem, t, uplineRoleId]);
  const { isReadOnly } = props;

  const onPotentialWinLimitChange = (
    sportTypeId: number,
    betTypeId: number,
    data: Partial<PotentialWinLimitGroupItem>,
    validateFieldName: PotentialWinLimitKey, 
    cachedObject?: CachePotentialWinLimitItem
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
    handleSettingItemChange?.(ProductGroupIds.SportsBook, GroupTypeIds.PotentialWinLimit, sportTypeId, betTypeId, data);

    const validator = new PotentialWinLimitValidator({ ...potentialWinLimitItem, ...data }, t, uplineRoleId);
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

  const handlePotentialWinLimitChange = (
    winLossLimit: PotentialWinLimitGroupItem,
    newWinLossLimit: Partial<PotentialWinLimitGroupItem>
  ) => {
    handleSettingItemChange?.(
      groupId,
      GroupTypeIds.PotentialWinLimit,
      winLossLimit.sportTypeId,
      winLossLimit.betTypeId,
      newWinLossLimit
    );
  };


  const potentialWinLimitPercent = () => {
    if (potentialWinLimitItem.accumAmount <= 0) return NumberHelper.formatWithCommas(0);
    
    return NumberHelper.formatWithCommas((potentialWinLimitItem.accumAmount * 100) / (potentialWinLimitItem.winLimit||1));
  }

  return (
    <BlockRow
      label={
        isMultipleMode && !isReadOnly
          ? renderBlockRowTitle(isMultipleModeChecked, productName, (isChecked) =>
            handleSettingItemChange?.(
              ProductGroupIds.SportsBook,
              GroupTypeIds.PotentialWinLimit,
              sportTypeId,
              betTypeId,
              { isMultipleModeChecked: isChecked }
            )
          )
          : (
            <>
              {productName}
              <PotentialWinLimitNote />
            </>
          )
      }
    >
      {ColumnsPotentialWinLimit.map((column) => {
        const { inputFieldName, checkBoxFieldName } = column;
        const id = `${inputFieldName}-${sportTypeId}-${betTypeId}`;
        const isErr = itemValidateResult[id] && itemValidateResult[id] !== true;
        return (
          <>
            <BlockColumn key={id}>
              {potentialWinLimitItem[inputFieldName] !== null &&
                (isReadOnly ? (
                  <div className="show-value">{potentialWinLimitItem[inputFieldName]}</div>
                ) : (
                  <PotentialWinLimitCell
                    key={id}
                    isDisable={isReadOnly}
                    // maxValue of downline is flow upline
                    maxValue={upline ? upline[inputFieldName] : 0}
                    currentValue={potentialWinLimitItem[inputFieldName]}
                    currentValueUnlimited={potentialWinLimitItem[checkBoxFieldName]}
                    originalIsUplineUnlimited={potentialWinLimitItem.original ? potentialWinLimitItem.original[checkBoxFieldName] : false}
                    onChange={(value) => {
                      onPotentialWinLimitChange(sportTypeId, betTypeId, { [inputFieldName]: value }, inputFieldName);
                    }}
                    onChangeUnlimited={(valueCheckBox, value) => {
                      if (valueCheckBox) {
                        onPotentialWinLimitChange(
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
                              [inputFieldName]: potentialWinLimitItem[inputFieldName],
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
                        onPotentialWinLimitChange(
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

            <BlockColumn>
              <div>
                <div className="blockLeft" style={{ display: 'flex', flexDirection: 'column' }}>
                  <div style={{ display: 'flex', alignItems: 'center' }}>
                    <input
                      type="checkbox"
                      checked={potentialWinLimitItem.isAutoReset}
                      disabled={isReadOnly || potentialWinLimitItem.isAllowUnlimitWinLimit}
                      onChange={(e) => handlePotentialWinLimitChange(potentialWinLimitItem, { isAutoReset: e.target.checked })}
                    />
                    <div>{t('DailyAutoResetLimitAt')} </div>
                  </div>

                  <div>
                    <div className='accum-info'
                      hidden={potentialWinLimitItem.isAllowUnlimitWinLimit}>
                      <div>
                        {t('AccumulateAmount', {
                          accumulateAmount: NumberHelper.formatWithCommas(potentialWinLimitItem.accumAmount),
                          accumulatePercent: potentialWinLimitPercent(),
                          maxPotentialWinLimit: NumberHelper.formatWithCommas(potentialWinLimitItem.winLimit || 0)
                        })}
                      </div>
                      <div className="accum-to-win" style={{ float: 'left' }}>
                        {t('ToWinAmount', {
                          toWinAmount: NumberHelper.formatWithCommas(potentialWinLimitItem.accumToWin)
                        })}
                      </div>
                      <div className="accum-winloss" style={{ float: 'right' }}>
                        {t('WinlossAmount', {
                          winlossAmount: NumberHelper.formatWithCommas(potentialWinLimitItem.accumWinLoss),
                        })}
                      </div>
                    </div>

                  </div>
                </div>
              </div>
            </BlockColumn>
          </>
        );
      })}
    </BlockRow>
  );
};

export default PotentialWinLimitRow;
