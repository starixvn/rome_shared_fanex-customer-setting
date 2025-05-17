import classNames from 'classnames';
import { forwardRef, useImperativeHandle, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { InputActionMeta, SingleValue } from 'react-select';

import { ValidationRefType, ValidationResult } from '@customer-web-app/share-models';
import { SystemIds, UserLevel } from '@customer-web-app/shared-constants';
import { GeneralInfo } from '@customer-web-app/shared-general-information-state';
import { SettingService } from '@customer-web-app/shared-group-settings-services';
import { scrollToBLock } from '@customer-web-app/shared-group-settings-ui';
import { AppConfigHelper } from '@customer-web-app/shared-helpers';
import { Block, BlockItem, BlockRefType } from '@customer-web-app/shared/base/block';
import { IconCorrect, IconWrong } from '@customer-web-app/shared/base/common-icon';

import { errorTooltip } from './common';
import { CopyCashSettingFromNote } from './copy-setting-from-note';
import CashMemberGeneralInformationValidator from './cash-member-general-information-validator';
import { GeneralInformationProps } from './general-information';
import { generalInformationValidatorFields, validateMaxCredit } from './general-information-validator';
import styles from './general-information.module.scss';
import { PasswordField } from './password-field';
import PasswordRule from './password-rule';
import Username from './username-field';
import UsernameSuggestionSelect, { UsernameSuggestionOption, getCashMemberPromiseOptions } from './username-suggestion-select';
import { BankInfomation } from './bank-info';
import CashUserInfo from './cash-user-info';

function maxCreditValidateFiled(obj: GeneralInfo): validateMaxCredit {
  const maxCredits: Record<UserLevel, validateMaxCredit> = {
    [UserLevel.Company]: 'companyMaxCredit',
    [UserLevel.Super]: 'superMaxCredit',
    [UserLevel.Master]: 'masterMaxCredit',
    [UserLevel.Agent]: 'agentMaxCredit',
    [UserLevel.Member]: 'memberMaxCredit',
  };
  return maxCredits[obj.roleId as UserLevel];
}

export interface CashMemberGeneralInformationProps extends GeneralInformationProps {
  onSelectCashMember: (custId: number) => void;
  numberOfAccounts: number;
  setNumberOfAccounts: (value: number) => void;
}

let selectedCashMember = '';
let selectedValue = '';

export const CashMemberGeneralInformation = forwardRef<ValidationRefType, CashMemberGeneralInformationProps>(
  function CashMemberGeneralInformation(
    { generalInfo, onGeneralInfoChange, children, onSelectCashMember, numberOfAccounts, setNumberOfAccounts },
    ref
  ) {
    const [collapse, setCollapse] = useState(false);
    const { t } = useTranslation();
    const blockRef = useRef<BlockRefType>(null);
    const [validationResults, setValidationResults] = useState<{ [key: string]: ValidationResult | boolean }>({});
    const [locationTooltip, setLocationTooltip] = useState<string>('');
    const [value, setValue] = useState('');
    const isNotNAP = appConfig.systemId !== SystemIds.NAP;

    useImperativeHandle(ref, () => {
      const validate = async () => {
        const validator = new CashMemberGeneralInformationValidator(
          generalInfo,
          selectedCashMember
        );
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
      const validator = new CashMemberGeneralInformationValidator(
        { ...generalInfo, ...data },
        selectedCashMember
      );
      const result = await validator.validateField(property);
      return result;
    };

    const getErrorByField = (fieldName: string) =>
      validationResults[fieldName] && validationResults[fieldName] !== true;

    const handleCheckIsCashMember = async (username: string) => {
      const custId = await SettingService.GetCashMember(username);
      if (custId > 0) {
        onSelectCashMember(custId);
        setValidationResults({
          ...validationResults,
          selectedCashMember: true,
        });
      } else {
        setValidationResults({
          ...validationResults,
          selectedCashMember: new ValidationResult(
            'selectedCashMember',
            custId === -1 ? 'ThisAccountWasDisabled' : 'ThisUserIsNotYourCashMember'
          ),
        });
      }
    };

    const handleUserSuggestionInputChange = async (newValue: SingleValue<UsernameSuggestionOption>) => {
      if (newValue) {
        setLocationTooltip('selectedCashMember');
        const selectedUsername = newValue.value;
        setValue(selectedUsername);
        selectedValue = selectedUsername;
        selectedCashMember = selectedUsername;
        await handleCheckIsCashMember(selectedUsername);
      }
    };

    const handleInputChange = async (newValue: string, { action }: InputActionMeta) => {
      if (action === 'input-change') {
        setLocationTooltip('selectedCashMember');
        setValue(newValue);
        selectedValue = newValue;
        if (!newValue) {
          selectedCashMember = '';
          setValidationResults({ ...validationResults, selectedCashMember: true });
        }
      }

      if (action === 'input-blur') {
        if (selectedValue && selectedValue !== selectedCashMember) {
          selectedCashMember = selectedValue;
          await handleCheckIsCashMember(selectedValue);
        }
      }
    };

    return (
      <Block id="general-information" title={t('general-information')} ref={blockRef} isPartialUpdate={false}>
        <div className={styles['container']}>
          <div className={styles['subheader']}>{t('AccountDetails')}</div>
          {children}
          <div className={styles['row']}>
            <BlockItem
              label={
                <>
                  {t('CopySettingFrom')}
                  {<CopyCashSettingFromNote />}
                </>
              }
            >
              <div>
                <UsernameSuggestionSelect
                  id="selectedCashMember"
                  value={value}
                  onChange={handleUserSuggestionInputChange}
                  onInputChange={handleInputChange}
                  promiseOptions={getCashMemberPromiseOptions}
                />
                {validationResults['selectedCashMember'] !== undefined && (
                  <span>{getErrorByField('selectedCashMember') ? <IconWrong /> : <IconCorrect />}</span>
                )}
              </div>
            </BlockItem>
            <BlockItem label={t('username')} isRequired>
              <div>
                {AppConfigHelper.isNapSystem() ? (
                  <>
                    <input
                      className={classNames({
                        error: getErrorByField('userName'),
                      })}
                      type="text"
                      id="userName"
                      name="username"
                      maxLength={30}
                      value={generalInfo.userName}
                      onChange={async (e) => {
                        const data = { userName: e.target.value };
                        onGeneralInfoChange?.(data);

                        const result = await validateGeneralInfo('userName', data);
                        setValidationResults({ ...validationResults, userName: result });
                      }}
                      onFocus={() => {
                        setLocationTooltip('userName');
                      }}
                      onBlur={() => setLocationTooltip('')}
                      autoComplete="new-username"
                    />
                    {validationResults['userName'] !== undefined && (
                      <span>{getErrorByField('userName') ? <IconWrong /> : <IconCorrect />}</span>
                    )}
                  </>
                ) : (
                  <Username
                    error={getErrorByField('userName')}
                    uplineUserName={generalInfo.uplineUserName}
                    newUserName={generalInfo.newUserName}
                    onChangeUserName={async (value) => {
                      setLocationTooltip('userName');
                      const data = {
                        userName: value,
                        newUserName: value.substring(value.length - generalInfo.newUserName.length),
                      };
                      onGeneralInfoChange?.(data);
                      const result = await validateGeneralInfo('userName', data);
                      setValidationResults({ ...validationResults, userName: result });
                    }}
                  />
                )}
              </div>
            </BlockItem>
            {/*   password*/}
            <BlockItem
              label={
                <>
                  {t('password')}
                  <span className="require">*</span>
                  {isNotNAP && <PasswordRule roleId={generalInfo.roleId} />}
                </>
              }
            >
              <div>
                <PasswordField
                  isError={getErrorByField('password')}
                  onPasswordChange={async (e) => {
                    const data = { password: e.target.value };
                    onGeneralInfoChange?.(data);
                    const result = await validateGeneralInfo('password', data);
                    setValidationResults({ ...validationResults, password: result });
                  }}
                  onFocus={() => {
                    setLocationTooltip('password');
                  }}
                  value={generalInfo.password}
                  onBlur={() => setLocationTooltip('')}
                />
                {isNotNAP && validationResults['password'] !== undefined && (
                  <span>
                    {getErrorByField('password') ? (
                      <IconWrong className={styles['icon-left']} />
                    ) : (
                      <IconCorrect className={styles['icon-left']} />
                    )}
                  </span>
                )}
              </div>
            </BlockItem>
          </div>

          <BankInfomation
            bankId={generalInfo.bank?.bankId}
            bankAccountNumber={generalInfo.bank?.accountNumber}
            onChangeBankId={bankId => onGeneralInfoChange?.({ bank: { ...generalInfo.bank, bankId: bankId } })}
            onChangeBankAccountNumber={bankAccountNumber => onGeneralInfoChange?.({ bank: { ...generalInfo.bank, accountNumber: bankAccountNumber } })}
          />

          <CashUserInfo
            userInformationFields={[
              { field: 'firstName', description: t('FirstNameDescription'), required: true, onFocus: () => setLocationTooltip('firstName'), onBlur: () => { setLocationTooltip('') }, isError: getErrorByField('firstName'), maxInputLength: 50 },
              { field: 'lastName', description: t('LastNameDescription'), required: true, onFocus: () => setLocationTooltip('lastName'), onBlur: () => { setLocationTooltip('') }, isError: getErrorByField('lastName'), maxInputLength: 50 },
              { field: 'mobilePhone', label: t('mobile'), description: t('MobilePhoneDescription'), required: true, component: 'phoneWithCountryCode', onFocus: () => setLocationTooltip('mobilePhone'), onBlur: () => { setLocationTooltip('') }, isError: getErrorByField('mobilePhone'), maxInputLength: 45 },
              { field: 'phone', onFocus: () => setLocationTooltip('phone'), onBlur: () => { setLocationTooltip('') }, maxInputLength: 50 },
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
          />
        </div>
        {errorTooltip(locationTooltip, validationResults, t)}
      </Block>
    );
  }
);
