import classNames from 'classnames';
import React, { forwardRef, useImperativeHandle, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { NumericFormat } from 'react-number-format';

import { ValidationRefType, ValidationResult } from '@customer-web-app/share-models';
import {
  isSportsBookProduct,
  MaxNumberInputLength,
  UserLevel,
  UserLevelName,
} from '@customer-web-app/shared-constants';
import { GeneralInfo } from '@customer-web-app/shared-general-information-state';
import { scrollToBLock } from '@customer-web-app/shared-group-settings-ui';
import { NumberHelper } from '@customer-web-app/shared-helpers';
import { Block, BlockItem, BlockRefType } from '@customer-web-app/shared/base/block';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import { errorTooltip, iconTrueFalse, maxCreditValidateFiled } from './common';
import CreditNote from './credit-note';
import CreditRange from './credit-range';
import GeneralInformationValidator, { generalInformationValidatorFields } from './general-information-validator';
import styles from './general-information.module.scss';
import UserInfo from './user-info';

interface EditGeneralInformationProps {
  generalInfo: GeneralInfo;
  productGroupId: number;
  onGeneralInfoChange: (data: Partial<GeneralInfo>) => void;
  children?: React.ReactNode;
  onUpdate?: () => void;
  isReadOnly?: boolean;
}

export const EditGeneralInformation = forwardRef<ValidationRefType, EditGeneralInformationProps>(
  function EditGeneralInformation(
    { productGroupId, generalInfo, onGeneralInfoChange, children, onUpdate, isReadOnly = false },
    ref
  ) {
    const [collapse, setCollapse] = useState(true);
    const { t } = useTranslation();
    const blockRef = useRef<BlockRefType>(null);
    const [validationResults, setValidationResults] = useState<{ [key: string]: ValidationResult | boolean }>({});
    const [locationTooltip, setLocationTooltip] = useState<string>('');

    useImperativeHandle(ref, () => {
      const validate = async () => {
        const validator = new GeneralInformationValidator(generalInfo);
        let validateResult = true;
        const validateItemResult = await validator.validateAllForEdit();
        const isErrorField = Object.values(validateItemResult).find((item) => item !== true) as ValidationResult;

        if (isErrorField) {
          setValidationResults(validateItemResult);
          if (isErrorField.fieldName === 'maxCredit') {
            setLocationTooltip(maxCreditValidateFiled(generalInfo).toString());
          } else {
            setLocationTooltip(isErrorField.fieldName);
          }
          validateResult = false;
          blockRef.current?.expand();
        }
        return validateResult;
      };

      const expandBlock = () => {
        blockRef.current?.expand();
      };

      return {
        validate,
        scrollToBlock: () => scrollToBLock('edit-general-info'),
        expandBlock,
      };
    });

    const validateGeneralInfo = async (property: generalInformationValidatorFields, data?: Partial<GeneralInfo>) => {
      const validator = new GeneralInformationValidator({ ...generalInfo, ...data });
      const result = await validator.validateField(property, true);
      return result;
    };

    const getErrorByField = (fieldName: string) => {
      return validationResults[fieldName] && validationResults[fieldName] !== true;
    };

    const napCanNotEditCredit = params.noPermissionEditCredit;
    let maxCreditMax = generalInfo.maxCreditMax;
    if (generalInfo.roleId === UserLevel.Super) {
      maxCreditMax = undefined;
    }

    const isShowCreditHint = generalInfo.roleId === UserLevel.Member && generalInfo.memberMaxCredit;

    if (isShowCreditHint) {
      maxCreditMax = Math.min(generalInfo.maxCreditMax || 0, generalInfo.memberMaxCredit || 0);
    }

    const renderCreditHint = () => {
      if (isShowCreditHint) {
        return (
          <CreditNote
            availableCredit={generalInfo.maxCreditMax || 0}
            memberMaxCredit={generalInfo.memberMaxCredit || 0}
          />
        );
      }
      return null;
    };

    return (
      <Block
        id="edit-general-info"
        title={t('general-information')}
        ref={blockRef}
        isPartialUpdate={!isReadOnly}
        onSaveClick={() => onUpdate?.()}
      >
        <div className={styles['container']}>
          <div className={styles['subheader']}>{t('AccountDetails')}</div>
          {children}
          <div className={styles['row']}>
            <BlockItem label={t('username')}>
              <span className={styles['username-edit']}>{generalInfo.userName}</span>
            </BlockItem>
            {isSportsBookProduct(productGroupId) && (
              <div className={styles['col-max-credit']}>
                <BlockItem
                  label={
                    generalInfo.roleId === UserLevel.Member
                      ? t('GivenCredit')
                      : t(`${UserLevelName[params.roleId]?.toLowerCase()}MaxCredit`)
                  }
                  isRequired={!(isReadOnly || napCanNotEditCredit)}
                >
                  <div>
                    <NumericFormat
                      className={classNames({
                        error: getErrorByField(maxCreditValidateFiled(generalInfo)),
                      })}
                      id="maxCredit"
                      name="maxCredit"
                      maxLength={MaxNumberInputLength}
                      allowNegative={false}
                      decimalScale={2}
                      thousandSeparator=","
                      valueIsNumericString
                      value={generalInfo.maxCredit}
                      onChange={async (e) => {
                        const data = { maxCredit: NumberHelper.parseNumber(e.target.value) };
                        onGeneralInfoChange?.(data);
                        const validateMaxCreditResult = await validateGeneralInfo(
                          maxCreditValidateFiled(generalInfo),
                          data
                        );
                        const validateMemberMaxCreditResult = await validateGeneralInfo('memberMaxCredit', data);
                        setValidationResults({
                          ...validationResults,
                          [maxCreditValidateFiled(generalInfo)]: validateMaxCreditResult,
                          memberMaxCredit: validateMemberMaxCreditResult,
                        });
                      }}
                      onFocus={() => {
                        setLocationTooltip(maxCreditValidateFiled(generalInfo));
                      }}
                      onBlur={() => setLocationTooltip('')}
                      disabled={isReadOnly || napCanNotEditCredit}
                    />
                    {napCanNotEditCredit && (
                      <WrapperTooltip type="warning" anchorSelect="#maxCredit" content={t('CannotUpdateCredit')} />
                    )}
                    {generalInfo.roleId !== UserLevel.Super && !isReadOnly && !napCanNotEditCredit && (
                      <span>{iconTrueFalse(getErrorByField(maxCreditValidateFiled(generalInfo)))}</span>
                    )}
                  </div>
                  <div className={styles['max-credit']}>
                    <CreditRange min={generalInfo.maxCreditMin ?? 0} max={maxCreditMax} />
                    {renderCreditHint()}
                  </div>
                </BlockItem>
              </div>
            )}
            {generalInfo.roleId === UserLevel.Agent && isSportsBookProduct(productGroupId) && (
              <BlockItem label={t(`memberMaxCredit`)} isRequired={!(isReadOnly || napCanNotEditCredit)}>
                <div>
                  <NumericFormat
                    maxLength={MaxNumberInputLength}
                    value={generalInfo.memberMaxCredit}
                    className={classNames({
                      error: getErrorByField('memberMaxCredit'),
                    })}
                    onChange={async (e) => {
                      const data = { memberMaxCredit: NumberHelper.parseNumber(e.target.value) };
                      onGeneralInfoChange?.(data);
                      const validateMemberMaxCreditResult = await validateGeneralInfo('memberMaxCredit', data);
                      setValidationResults({ ...validationResults, memberMaxCredit: validateMemberMaxCreditResult });
                    }}
                    allowNegative={false}
                    decimalScale={2}
                    thousandSeparator=","
                    valueIsNumericString
                    id="memberMaxCredit"
                    onFocus={() => {
                      setLocationTooltip('memberMaxCredit');
                    }}
                    onBlur={() => setLocationTooltip('')}
                    disabled={isReadOnly || napCanNotEditCredit}
                  />
                  {napCanNotEditCredit && (
                    <WrapperTooltip type="warning" anchorSelect="#memberMaxCredit" content={t('CannotUpdateCredit')} />
                  )}
                  {!napCanNotEditCredit && <span>{iconTrueFalse(getErrorByField('memberMaxCredit'))}</span>}
                </div>
                <div className={styles['max-credit']}>
                  <CreditRange min={generalInfo.memberMaxCreditMin ?? 0} max={generalInfo.maxCredit ?? 0} />
                </div>
              </BlockItem>
            )}
          </div>
          <UserInfo
            generalInfo={generalInfo}
            handleGeneralInfoChange={(fieldName, value) => {
              const data = { [fieldName]: value };
              onGeneralInfoChange?.(data);
            }}
            setCollapse={setCollapse}
            collapse={collapse}
            isReadOnly={isReadOnly}
          />
        </div>
        {errorTooltip(locationTooltip, validationResults, t)}
      </Block>
    );
  }
);
