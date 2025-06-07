import { forwardRef, useImperativeHandle, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { Result, ValidationRefType, ValidationResult } from '@customer-web-app/share-models';
import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import { PotentialWinLimitGroupItem } from '@customer-web-app/shared-group-settings-types';
import { scrollToBLock } from '@customer-web-app/shared-group-settings-ui';
import { Block, BlockRefType, BlockRow } from '@customer-web-app/shared/base/block';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import { ValidationContext } from '../bet-setting/validation-context';
import { GroupSettingBlockProps } from '../types';
import PotentialWinLimitInfo from './potential-win-limit-info';
import PotentialWinLimitRow from './potential-win-limit-row';
import PotentialWinLimitValidator from './potential-win-limit-validator';

const PotentialWinLimitBlockId = 'potential-win-limit';

const PotentialWinLimit = forwardRef<ValidationRefType, GroupSettingBlockProps>(function PotentialWinLimit(props, ref) {
  const { t } = useTranslation();
  const blockRef = useRef<BlockRefType>(null);
  const { productGroups, generalInfo, 
    isMultipleMode, handleUpdateSettingItems, 
    isEditMode, updateGroupSetting, roleId } =
    useGroupSettingContext();
  const potentialWinLimits = getSettingItems<PotentialWinLimitGroupItem>(
    productGroups,
    ProductGroupIds.SportsBook,
    GroupTypeIds.PotentialWinLimit
  );
  const [validationState, setValidationState] = useState<ValidationResult | undefined>(
    Result.CreateSuccessfulResult() as ValidationResult
  );
  const { fieldName, isSuccessful } = validationState ?? {};

  useImperativeHandle(ref, () => {
    const validate = () => {
      let validateResult = true;

      if (props.isReadOnly) return validateResult;

      for (const element of potentialWinLimits) {
        if (element.isMultipleModeChecked) {
          const validator = new PotentialWinLimitValidator(element, t, generalInfo?.uplineRoleId);
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
      scrollToBlock: () => scrollToBLock(PotentialWinLimitBlockId),
    };
  });

  const handleSelectAll = (isChecked: boolean) => {
    const newPotentialWinLimits = potentialWinLimits.map((item) => ({ ...item, isMultipleModeChecked: isChecked }));
    handleUpdateSettingItems?.(ProductGroupIds.SportsBook, GroupTypeIds.PotentialWinLimit, newPotentialWinLimits);
  };

  if (potentialWinLimits.length === 0) {
    return null;
  }
  return (
    <Block
      id={PotentialWinLimitBlockId}
      title={
        <>
          {t('PotentialWinLimit')}
          <PotentialWinLimitInfo />
        </>
      }      
      ref={blockRef}
      isPartialUpdate={isEditMode === true && !props.isReadOnly}
      onSaveClick={() => updateGroupSetting?.(GroupTypeIds.PotentialWinLimit)}
    >
      <ValidationContext.Provider value={{ validationState, setValidationState }}>
        { isMultipleMode && !props.isReadOnly &&(
          <BlockRow
          isHeaderRow
          label={(
              <div style={{ display: 'flex', marginLeft: '2px' }}>
                <input
                  type="checkbox"
                  style={{ top: '0' }}
                  onChange={(e) => handleSelectAll(e.target.checked)}
                  checked={potentialWinLimits.every((item) => item.isMultipleModeChecked === true)}
                />
                <div>{t('SelectAll')}</div>
              </div>
            )
          }
        >
          
        </BlockRow>
        )}
        
        {potentialWinLimits?.map((item) => {
          return (
            <PotentialWinLimitRow
              isReadOnly={props.isReadOnly}
              key={`${item.sportTypeId}-${item.betTypeId}`}
              potentialWinLimitItem={item}
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

export default PotentialWinLimit;
