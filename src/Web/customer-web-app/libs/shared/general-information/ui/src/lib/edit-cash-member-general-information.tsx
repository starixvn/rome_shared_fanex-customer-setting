import { forwardRef, useImperativeHandle, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { ValidationRefType, ValidationResult } from '@customer-web-app/share-models';
import { GeneralInfo } from '@customer-web-app/shared-general-information-state';
import { scrollToBLock } from '@customer-web-app/shared-group-settings-ui';
import { Block, BlockItem, BlockRefType } from '@customer-web-app/shared/base/block';

import { errorTooltip } from './common';
import { generalInformationValidatorFields } from './general-information-validator';
import styles from './general-information.module.scss';
import CashUserInfo from './cash-user-info';
import EditCashMemberGeneralInformationValidator from './edit-cash-member-general-information-validator';

interface EditCashMemberGeneralInformationProps {
  generalInfo: GeneralInfo;
  onGeneralInfoChange: (data: Partial<GeneralInfo>) => void;
  children?: React.ReactNode;
  onUpdate?: () => void;
  isReadOnly?: boolean;
}

export const EditCashMemberGeneralInformation = forwardRef<ValidationRefType, EditCashMemberGeneralInformationProps>(
  function EditCashMemberGeneralInformation(
    { generalInfo, onGeneralInfoChange, children, onUpdate, isReadOnly = false  },
    ref
  ) {
    const [collapse, setCollapse] = useState(false);
    const { t } = useTranslation();
    const blockRef = useRef<BlockRefType>(null);
    const [validationResults, setValidationResults] = useState<{ [key: string]: ValidationResult | boolean }>({});
    const [locationTooltip, setLocationTooltip] = useState<string>('');

    useImperativeHandle(ref, () => {
      const validate = async () => {
        const validator = new EditCashMemberGeneralInformationValidator(generalInfo);
        let validateResult = true;
        const validateItemResult = await validator.validateAllRules();
        const isErrorField = Object.values(validateItemResult).find((item) => item !== true) as ValidationResult;
        if (isErrorField) {
          setValidationResults(validateItemResult);
          if (['firstName', 'lastName', 'mobilePhone'].includes(isErrorField.fieldName)) {
            setCollapse(false);
          }
          blockRef.current?.expand();
          setLocationTooltip(isErrorField.fieldName);
          validateResult = false;
        }
        return validateResult;
      };

      const expandBlock = () => {
        blockRef.current?.expand();
      };

      return {
        validate,
        scrollToBlock: () => scrollToBLock('general-information'),
        expandBlock,
      };
    });

    const validateGeneralInfo = async (property: generalInformationValidatorFields, data?: Partial<GeneralInfo>) => {
        const validator = new EditCashMemberGeneralInformationValidator({ ...generalInfo, ...data });
          const result = await validator.validateField(property, true);
          return result;
        };

    const getErrorByField = (fieldName: string) =>
      validationResults[fieldName] && validationResults[fieldName] !== true;

    return (
      <Block 
        id="general-information" 
        title={t('general-information')} 
        ref={blockRef} 
        isPartialUpdate={!isReadOnly} 
        onSaveClick={() => onUpdate?.()}>
        <div className={styles['container']}>
          <div className={styles['subheader']}>{t('AccountDetails')}</div>
          {children}
          <div className={styles['row']}>
            <BlockItem label={t('username')}>
              <span className={styles['username-edit']}>{generalInfo.userName}</span>
            </BlockItem>
          </div>

          <CashUserInfo
            userInformationFields={[
              {
                field: 'firstName',
                row: 0,
                description: t('FirstNameDescription'),
                required: true,
                onFocus: () => setLocationTooltip('firstName'),
                onBlur: () => { setLocationTooltip('') },
                isError: getErrorByField('firstName'),
                maxInputLength: 50
              },
              {
                field: 'lastName',
                row: 0,
                description: t('LastNameDescription'),
                required: true,
                onFocus: () => setLocationTooltip('lastName'),
                onBlur: () => { setLocationTooltip('') },
                isError: getErrorByField('lastName'),
                maxInputLength: 50
              },
              {
                field: 'mobilePhone',
                row: 0,
                label: t('mobile'),
                description: t('MobilePhoneDescription'),
                required: true,
                component: 'phoneWithCountryCodeModeEdit',
                onFocus: () => setLocationTooltip('mobilePhone'),
                onBlur: () => { setLocationTooltip('') },
                isError: getErrorByField('mobilePhone'),
                maxInputLength: 45
              },
              {
                field: 'phone',
                row: 0,
                onFocus: () => setLocationTooltip('phone'),
                onBlur: () => { setLocationTooltip('') },
                maxInputLength: 50
              },
              {
                field: 'remark',
                row: 1,
                component: 'textArea',
                onFocus: () => setLocationTooltip('remark'),
                onBlur: () => { setLocationTooltip('') },
                maxInputLength: 500
              }
            ]}
            generalInfo={generalInfo}
            handleGeneralInfoChange={async (field, value) => {
              const data = { [field]: value };
              const result = await validateGeneralInfo(field as generalInformationValidatorFields, data);
              setValidationResults({ ...validationResults, [field]: result });
              onGeneralInfoChange?.(data);
            }}
            setCollapse={setCollapse}
            collapse={collapse}
            isReadOnly={isReadOnly}
          />
          {errorTooltip(locationTooltip, validationResults, t)}
        </div>
      </Block>
    );
  }
);
