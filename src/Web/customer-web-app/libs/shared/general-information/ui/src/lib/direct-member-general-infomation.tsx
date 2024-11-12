import classNames from 'classnames';
import { forwardRef, useImperativeHandle, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { NumericFormat } from 'react-number-format';

import { ValidationRefType, ValidationResult } from '@customer-web-app/share-models';
import { MaxNumberInputLength, SystemIds, UserLevel } from '@customer-web-app/shared-constants';
import { GeneralInfo } from '@customer-web-app/shared-general-information-state';
import { scrollToBLock } from '@customer-web-app/shared-group-settings-ui';
import { AppConfigHelper, NumberHelper } from '@customer-web-app/shared-helpers';
import { Block, BlockItem, BlockRefType } from '@customer-web-app/shared/base/block';
import { IconCorrect, IconWrong } from '@customer-web-app/shared/base/common-icon';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import CommissionGroup from './commission-group';
import { errorTooltip } from './common';
import CopySettingFromNote from './copy-setting-from-note';
import CreditNote from './credit-note';
import CreditRange from './credit-range';
import { GeneralInformationProps } from './general-information';
import GeneralInformationValidator, {
    generalInformationValidatorFields, validateMaxCredit
} from './general-information-validator';
import styles from './general-information.module.scss';
import NoOfAccounts from './no-of-accounts';
import { PasswordField } from './password-field';
import PasswordRule from './password-rule';
import UserInfo from './user-info';
import Username from './username-field';
import UsernameSuggestionSelect from './username-suggestion-select';

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

export const DirectMemberGeneralInformation = forwardRef<ValidationRefType, GeneralInformationProps>(
  function DirectMemberGeneralInformation({ generalInfo, onGeneralInfoChange, children }, ref) {
    const [collapse, setCollapse] = useState(true);
    const { t } = useTranslation();
    const blockRef = useRef<BlockRefType>(null);
    const [validationResults, setValidationResults] = useState<{ [key: string]: ValidationResult | boolean }>({});
    const [locationTooltip, setLocationTooltip] = useState<string>('');
    const { roleId, uplineMaxCredit, uplineMemberMaxCredit } = generalInfo;
    const isNotNAP = appConfig.systemId !== SystemIds.NAP;

    useImperativeHandle(ref, () => {
      const validate = async () => {
        const validator = new GeneralInformationValidator(generalInfo);
        let validateResult = true;
        const validateItemResult = await validator.validateAll();
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
        scrollToBlock: () => scrollToBLock('general-information'),
        expandBlock,
      };
    });

    const validateGeneralInfo = async (property: generalInformationValidatorFields, data?: Partial<GeneralInfo>) => {
      const validator = new GeneralInformationValidator({ ...generalInfo, ...data });
      const result = await validator.validateField(property);
      return result;
    };

    const renderCreditHint = () => {
      if (roleId === UserLevel.Member && uplineMemberMaxCredit) {
        return <CreditNote availableCredit={uplineMaxCredit || 0} memberMaxCredit={uplineMemberMaxCredit || 0} />;
      }
      return null;
    };

    const getErrorByField = (fieldName: string) =>
      validationResults[fieldName] && validationResults[fieldName] !== true;
    const napCanNotEditCredit = params.noPermissionEditCredit;

    return (
      <Block id="general-information" title={t('general-information')} ref={blockRef} isPartialUpdate={false}>
        <div className={styles['container']}>
          <div className={styles['subheader']}>{t('AccountDetails')}</div>
          {children}
          <div className={styles['row']}>
            {/*   username  */}
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
          <div className={styles['row']}>
            <BlockItem
              label={
                <>
                  {t('CopySettingFrom')}
                  {<CopySettingFromNote />}
                </>
              }
            >
              <UsernameSuggestionSelect />
            </BlockItem>
            <BlockItem label={t('NoOfAccounts')}>
              <NoOfAccounts />
            </BlockItem>
            <div className={styles['col-max-credit']}>
              <BlockItem label={t('GivenCreditPerAccount')} isRequired>
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
                    data-tooltip-id="not-edit-max-credit"
                    disabled={napCanNotEditCredit}
                  />
                  {napCanNotEditCredit && (
                    <WrapperTooltip type="warning" id="not-edit-max-credit" content={t('CannotUpdateCredit')} />
                  )}
                  {roleId !== UserLevel.Super && !napCanNotEditCredit && (
                    <span>
                      {getErrorByField(maxCreditValidateFiled(generalInfo)) ? <IconWrong /> : <IconCorrect />}
                    </span>
                  )}
                </div>
                {roleId < UserLevel.Super && (
                  <div className={styles['max-credit']}>
                    <CreditRange
                      min={0}
                      max={
                        uplineMemberMaxCredit
                          ? Math.min(uplineMaxCredit || 0, uplineMemberMaxCredit)
                          : uplineMaxCredit || 0
                      }
                    />
                    {renderCreditHint()}
                  </div>
                )}
              </BlockItem>
            </div>
            {/*   CommissionGroup */}
            <div className={styles['col-group']}>
              <BlockItem label={t('Group')}>
                <CommissionGroup
                  site={generalInfo.site}
                  currency={generalInfo.currency.toString()}
                  value={generalInfo.group}
                  onChange={(e) => {
                    const data = { group: e.target.value };
                    onGeneralInfoChange?.(data);
                  }}
                />
              </BlockItem>
            </div>
          </div>
          <UserInfo
            generalInfo={generalInfo}
            handleGeneralInfoChange={(fieldName, value) => {
              const data = { [fieldName]: value };
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
