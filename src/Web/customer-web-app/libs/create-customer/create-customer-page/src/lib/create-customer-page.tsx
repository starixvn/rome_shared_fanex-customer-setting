import { useEffect, useReducer, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import {
    handleChangeGroupSettingValue, handleChangeItemValue
} from '@customer-web-app/edit-customer-page';
import {
    CreateCustomerResult, ValidateRefResult, ValidationRefType
} from '@customer-web-app/share-models';
import { BlockUI } from '@customer-web-app/shared-block-ui';
import { Currencies, ProductGroupIds, SystemIds } from '@customer-web-app/shared-constants';
import { ErrorBlock } from '@customer-web-app/shared-error-block';
import {
    GeneralInfo, GeneralInfoActionTypes, generalInformationReducer, initialState
} from '@customer-web-app/shared-general-information-state';
import {
    GeneralInformation, TransferCondition, TransferOptions
} from '@customer-web-app/shared-general-information-ui';
import { GroupSettingsContextProvider } from '@customer-web-app/shared-group-settings-context';
import { SettingService } from '@customer-web-app/shared-group-settings-services';
import { GroupSettingItem, ProductGroup } from '@customer-web-app/shared-group-settings-types';
import { GroupSetting, scrollToTop } from '@customer-web-app/shared-group-settings-ui';
import { encrypt } from '@customer-web-app/shared-helpers';
import { Layout, useTheme } from '@customer-web-app/shared/base/layout';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import styles from './create-customer-page.module.scss';
import { handleInitProducts } from './handler';

export function CreateCustomerPage() {
  const { t } = useTranslation();
  const theme = useTheme();
  const [generalInformationState, generalInfoDispatch] = useReducer(generalInformationReducer, initialState);
  const { generalInfo } = generalInformationState;

  const [isLoading, setIsLoading] = useState(true);
  const [createCustomerResult, setCreateCustomerResult] = useState<CreateCustomerResult | null>();
  const [productGroups, setProductGroups] = useState<ProductGroup[]>([]);
  const [group, setGroup] = useState<string>('');

  const generalInfoRef = useRef<ValidationRefType>(null);
  const groupSettingRef = useRef<ValidationRefType>(null);
  const transferRef = useRef<ValidationRefType>(null);

  useEffect(() => {
    const fetchData = async () => {
      const { generalInfo, productGroups } = await SettingService.GetCustomerSetting(
        params.uplineId,
        params.uplineRoleId,
        0,
        params.agentId,
        false,
        params.siteLabel
      );
      const newProductGroups = handleInitProducts(productGroups, params.uplineRoleId);
      if (appConfig.systemId !== SystemIds.NAP) {
        generalInfo.newUserName = params.suggestUsername;
      } else if (params.site && params.siteLabel) {
        generalInfo.site = params.site;
        generalInfo.siteLabel = params.siteLabel;
      }
      setProductGroups(newProductGroups);
      generalInfoDispatch({
        type: GeneralInfoActionTypes.Update,
        payload: {
          ...generalInfo,
          userName:
            appConfig.systemId !== SystemIds.NAP
              ? (generalInfo.uplineUserName + generalInfo.newUserName).toUpperCase()
              : '',
          maxCredit: 0,
          memberMaxCredit: 0,
          uplineMaxCredit: generalInfo.maxCredit,
          uplineMemberMaxCredit: generalInfo.memberMaxCredit,
          roleId: params.uplineRoleId - 1,
          uplineCustId: params.uplineId,
          uplineRoleId: params.uplineRoleId,
          displayTransferCondition: generalInfo.transferOption === TransferOptions.Daily,
        },
      });
      setGroup(generalInfo.currency.toString() === Currencies.Won.toString() ? 'dea' : 'a');
      setIsLoading(false);
    };

    fetchData();
  }, []);

  const handleGeneralInfoChange = (payload: Partial<GeneralInfo>) => {
    generalInfoDispatch({ type: GeneralInfoActionTypes.Update, payload });
  };

  const handleSettingItemChange = (
    productGroupId: number,
    groupTypeId: number,
    sportTypeId: number,
    betTypeId: number,
    data: Partial<GroupSettingItem<unknown>>
  ) => {
    const newProductGroups = handleChangeItemValue(
      productGroups,
      productGroupId,
      groupTypeId,
      sportTypeId,
      betTypeId,
      data
    );
    setProductGroups(newProductGroups);
  };

  const handleUpdateSettingItems = (
    productGroupId: number,
    groupTypeId: number,
    groupItems: GroupSettingItem<unknown>[]
  ) => {
    const newProductGroups = handleChangeGroupSettingValue(productGroups, productGroupId, groupTypeId, groupItems);
    setProductGroups(newProductGroups);
  };

  const scrollToInvalidGroup = (
    isValidGeneralInfo: boolean,
    validateGroupSettingResult: ValidateRefResult,
    isValidTransfer: boolean
  ) => {
    if (!isValidGeneralInfo) {
      generalInfoRef?.current?.scrollToBlock && generalInfoRef.current.scrollToBlock();
      return;
    }

    if (!validateGroupSettingResult.isValid) {
      groupSettingRef?.current?.scrollToBlock &&
        groupSettingRef.current.scrollToBlock(validateGroupSettingResult.groupTypeId);
    }

    !isValidTransfer && transferRef?.current?.scrollToBlock && transferRef?.current?.scrollToBlock();
  };

  const validateGroupTypes = async () => groupSettingRef.current && (await groupSettingRef.current.validate());

  const handleCreateCustomer = async () => {
    setIsLoading(true);
    const isValidGeneralInfo = generalInfoRef.current ? ((await generalInfoRef.current.validate()) as boolean) : true;
    const validateGroupSettingResult = (await validateGroupTypes()) as ValidateRefResult;
    const isValidTransfer = generalInfo.displayTransferCondition ? (transferRef.current?.validate() as boolean) : true;

    if (isValidGeneralInfo && validateGroupSettingResult.isValid && isValidTransfer) {
      const secretKey = (document.getElementById('SecretKey') as HTMLInputElement).value;
      const encryptedPassword = encrypt(generalInfo.password, secretKey);
      const result = await SettingService.CreateCustomer(
        { ...generalInfo, password: encryptedPassword },
        group,
        productGroups
      );
      if (result.isSuccessful) {
        setIsLoading(true);
        const createResult = result as CreateCustomerResult;
        setTimeout(() => {
          if (createResult.newCustIds && createResult.newCustIds.length > 0) {
            window.redirectToSettingPage({ username: generalInfo.userName, custId: createResult.newCustIds[0] });
          }
        }, 3000);
        return;
      } else {
        setCreateCustomerResult(result);
        scrollToTop();
      }
    }
    scrollToInvalidGroup(isValidGeneralInfo, validateGroupSettingResult, isValidTransfer);
    setIsLoading(false);
  };

  const renderCreateButton = () => {
    return (
      <div className={styles.btn}>
        <button type="button" onClick={handleCreateCustomer}>
          {t('CreateAccount')}
        </button>
      </div>
    );
  };

  return (
    <>
      {isLoading && <BlockUI />}
      <WrapperTooltip id="global-tooltip" className={styles['global-tooltip']} />
      <Layout theme={theme}>
        {renderCreateButton()}
        {createCustomerResult && <ErrorBlock error={createCustomerResult} />}
        <GeneralInformation
          ref={generalInfoRef}
          generalInfo={generalInfo}
          onGeneralInfoChange={handleGeneralInfoChange}
        />
        <GroupSettingsContextProvider
          value={{
            generalInfo,
            group,
            handleMemberCommGroupChange: setGroup,
            productGroups,
            isMultipleMode: false,
            mode: 'createCustomer',
            handleSettingItemChange,
            handleUpdateSettingItems,
            productGroupId: ProductGroupIds.SportsBook,
          }}
        >
          <GroupSetting ref={groupSettingRef} />
        </GroupSettingsContextProvider>
        {generalInfo.displayTransferCondition && (
          <TransferCondition
            ref={transferRef}
            generalInfo={generalInfo}
            onGeneralInfoChange={handleGeneralInfoChange}
          />
        )}
        {renderCreateButton()}
      </Layout>
    </>
  );
}

export default CreateCustomerPage;
