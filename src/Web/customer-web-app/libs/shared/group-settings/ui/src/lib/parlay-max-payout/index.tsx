import { forwardRef, useImperativeHandle, useRef, useState } from 'react';
import { useTranslate } from '@tolgee/react';

import { Result, ValidationRefType, ValidationResult } from '@customer-web-app/share-models';
import { GroupTypeIds, ProductGroupIds, UserLevel } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import { ParlayMaxPayoutGroupItem } from '@customer-web-app/shared-group-settings-types';
import { scrollToBLock } from '@customer-web-app/shared-group-settings-ui';
import { Block, BlockColumn, BlockRefType, BlockRow } from '@customer-web-app/shared/base/block';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import { ValidationContext } from '../bet-setting/validation-context';
import { GroupSettingBlockProps } from '../types';
import ParlayMaxPayoutRow from './parlay-max-payout-row';
import ParlayMaxPayoutValidator from './parlay-max-payout-validator';
import { AppConfigHelper } from '@customer-web-app/shared-helpers';

const ParlayMaxPayoutBlockId = 'parlay-max-payout';

const ParlayMaxPayout = forwardRef<ValidationRefType, GroupSettingBlockProps>(function ParlayMaxPayout(props, ref) {
  const { t } = useTranslate();
  const blockRef = useRef<BlockRefType>(null);
  const { productGroups, generalInfo, 
    isMultipleMode, handleUpdateSettingItems, 
    isEditMode, updateGroupSetting, roleId } =
    useGroupSettingContext();
  const parlayMaxPayouts = getSettingItems<ParlayMaxPayoutGroupItem>(
    productGroups,
    ProductGroupIds.SportsBook,
    GroupTypeIds.ParlayMaxPayout
  );
  const [validationState, setValidationState] = useState<ValidationResult | undefined>(
    Result.CreateSuccessfulResult() as ValidationResult
  );
  const { fieldName, isSuccessful } = validationState ?? {};

  useImperativeHandle(ref, () => {
    const validate = () => {
      let validateResult = true;

      if (props.isReadOnly) return validateResult;

      for (const element of parlayMaxPayouts) {
        if (element.isMultipleModeChecked) {
          const validator = new ParlayMaxPayoutValidator(element, t, generalInfo?.uplineRoleId);
          const validateItemResult = validator.validateAll();
          const isErrorField = Object.values(validateItemResult).find((item) => item !== true) as ValidationResult;
          if (isErrorField) {
            setValidationState(isErrorField);
            validateResult = false;
            break;
          }
        }
      }
      if (validateResult === false) {
        blockRef.current?.expand();
      }

      if (validateResult) {
        setValidationState(undefined);
      }

      return validateResult;
    };

    return {
      validate,
      scrollToBlock: () => scrollToBLock(ParlayMaxPayoutBlockId),
    };
  });

  const handleSelectAll = (isChecked: boolean) => {
    const newParlayMaxPayouts = parlayMaxPayouts.map((item) => ({ ...item, isMultipleModeChecked: isChecked }));
    handleUpdateSettingItems?.(ProductGroupIds.SportsBook, GroupTypeIds.ParlayMaxPayout, newParlayMaxPayouts);
  };

  const handleSelectSingleMatchParlay = (isChecked: boolean) => {
    const newParlayMaxPayouts = parlayMaxPayouts.map((item) => ({ ...item, singleMatchParlayMultipleMode: isChecked }));
    handleUpdateSettingItems?.(ProductGroupIds.SportsBook, GroupTypeIds.ParlayMaxPayout, newParlayMaxPayouts);
  };

  const handleSelectCombo2 = (isChecked: boolean) => {
    const newParlayMaxPayouts = parlayMaxPayouts.map((item) => ({ ...item, combo2MultipleMode: isChecked }));
    handleUpdateSettingItems?.(ProductGroupIds.SportsBook, GroupTypeIds.ParlayMaxPayout, newParlayMaxPayouts);
  };

  const handleSelectCombo35 = (isChecked: boolean) => {
    const newParlayMaxPayouts = parlayMaxPayouts.map((item) => ({ ...item, combo35MultipleMode: isChecked }));
    handleUpdateSettingItems?.(ProductGroupIds.SportsBook, GroupTypeIds.ParlayMaxPayout, newParlayMaxPayouts);
  };

  const handleSelectComboOver6 = (isChecked: boolean) => {
    const newParlayMaxPayouts = parlayMaxPayouts.map((item) => ({ ...item, comboOver6MultipleMode: isChecked }));
    handleUpdateSettingItems?.(ProductGroupIds.SportsBook, GroupTypeIds.ParlayMaxPayout, newParlayMaxPayouts);
  };

  if (parlayMaxPayouts.length === 0) {
    return null;
  }

  return (
    <Block
      id={ParlayMaxPayoutBlockId}
      title={t('ParlayMaxPayout')}
      ref={blockRef}
      isPartialUpdate={isEditMode === true && !props.isReadOnly}
      onSaveClick={() => updateGroupSetting?.(GroupTypeIds.ParlayMaxPayout)}
    >
      <ValidationContext.Provider value={{ validationState, setValidationState }}>
        <BlockRow
          isHeaderRow
          label={
            isMultipleMode &&
            !props.isReadOnly && (
              <div style={{ display: 'flex', marginLeft: '2px' }}>
                <input
                  type="checkbox"
                  style={{ top: '0' }}
                  onChange={(e) => handleSelectAll(e.target.checked)}
                  checked={parlayMaxPayouts.every((item) => item.isMultipleModeChecked === true)}
                />
                <div>{t('SelectAllSports')}</div>
              </div>
            )
          }
        >
          <BlockColumn>
            {
              <>
                {!props.isReadOnly && roleId === UserLevel.Super && !AppConfigHelper.isEditBetSettingOnly() && (
                  <input
                      type="checkbox"
                      defaultChecked={true}
                      style={{ top: '0', margin: '0 4px' }}
                      onChange={(e) => handleSelectSingleMatchParlay(e.target.checked)}
                      checked={parlayMaxPayouts.every((item) => item.singleMatchParlayMultipleMode === true)}
                    />
                )}
                {t('singleMatchParlay')}
              </>
            }
          </BlockColumn>
          <BlockColumn>
            {
              <>
                {!props.isReadOnly && roleId === UserLevel.Super && !AppConfigHelper.isEditBetSettingOnly() && (
                  <input
                      type="checkbox"
                      defaultChecked={true}
                      style={{ top: '0', margin: '0 4px' }}
                      onChange={(e) => handleSelectCombo2(e.target.checked)}
                      checked={parlayMaxPayouts.every((item) => item.combo2MultipleMode === true)}
                    />
                )}
                {t('combo2')}
              </>
            }
          </BlockColumn>
          <BlockColumn>
            {
              <>
                {!props.isReadOnly && roleId === UserLevel.Super && !AppConfigHelper.isEditBetSettingOnly() && (
                  <input
                      type="checkbox"
                      defaultChecked={true}
                      style={{ top: '0', margin: '0 4px' }}
                      onChange={(e) => handleSelectCombo35(e.target.checked)}
                      checked={parlayMaxPayouts.every((item) => item.combo35MultipleMode === true)}
                    />
                )}
                {t('combo35')}
              </>
            }
          </BlockColumn>
          <BlockColumn>
            {
              <>
                {!props.isReadOnly && roleId === UserLevel.Super && !AppConfigHelper.isEditBetSettingOnly() && (
                  <input
                      type="checkbox"
                      defaultChecked={true}
                      style={{ top: '0', margin: '0 4px' }}
                      onChange={(e) => handleSelectComboOver6(e.target.checked)}
                      checked={parlayMaxPayouts.every((item) => item.comboOver6MultipleMode === true)}
                    />
                )}
                {t('comboOver6')}
              </>
            }
          </BlockColumn>
        </BlockRow>
        {parlayMaxPayouts?.map((item) => {
          return (
            <ParlayMaxPayoutRow
              isReadOnly={props.isReadOnly}
              key={`${item.sportTypeId}-${item.betTypeId}`}
              parlayMaxPayoutItem={item}
              uplineRoleId={generalInfo?.uplineRoleId}
              setErrorTooltip={(validationState: ValidationResult | undefined) => setValidationState(validationState)}
            />
          );
        })}
        {validationState && <WrapperTooltip type="error" anchorSelect={`#${fieldName}`} isOpen={!isSuccessful} />}
      </ValidationContext.Provider>
    </Block>
  );
});

export default ParlayMaxPayout;
