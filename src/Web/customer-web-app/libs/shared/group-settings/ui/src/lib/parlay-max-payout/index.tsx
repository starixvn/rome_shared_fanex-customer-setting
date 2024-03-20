import { forwardRef, useImperativeHandle, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { Result, ValidationRefType, ValidationResult } from '@customer-web-app/share-models';
import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import { ParlayMaxPayoutGroupItem } from '@customer-web-app/shared-group-settings-types';
import { Block, BlockColumn, BlockRefType, BlockRow } from '@customer-web-app/shared/base/block';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import { ValidationContext } from '../bet-setting/validation-context';
import scrollToBLock from '../scroll-helper';
import { GroupSettingBlockProps } from '../types';
import ParlayMaxPayoutRow from './parlay-max-payout-row';
import ParlayMaxPayoutValidator from './parlay-max-payout-validator';

const ParlayMaxPayoutBlockId = 'parlay-max-payout';

const ParlayMaxPayout = forwardRef<ValidationRefType, GroupSettingBlockProps>(function ParlayMaxPayout(props, ref) {
  const { t } = useTranslation();
  const blockRef = useRef<BlockRefType>(null);
  const { productGroups, generalInfo, isMultipleMode, handleUpdateSettingItems, isEditMode, updateGroupSetting } =
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

  if (parlayMaxPayouts.length === 0) {
    return null;
  }

  return (
    <Block
      id={ParlayMaxPayoutBlockId}
      title={t('ParlayMaxPayout')}
      ref={blockRef}
      isPartialUpdate={isEditMode === true && !props.isReadOnly}
      onSaveClick={() => updateGroupSetting?.(GroupTypeIds.SportsBookBetSetting)}
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
                <div>{t('SelectAll')}</div>
              </div>
            )
          }
        >
          <BlockColumn>{t('singleMatchParlay')}</BlockColumn>
          <BlockColumn>{t('combo2')}</BlockColumn>
          <BlockColumn>{t('combo35')}</BlockColumn>
          <BlockColumn>{t('comboOver6')}</BlockColumn>
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
