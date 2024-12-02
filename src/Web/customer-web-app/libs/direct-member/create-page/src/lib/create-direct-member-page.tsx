import { useEffect, useReducer, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import {
    handleChangeGroupSettingValue, handleChangeItemValue
} from '@customer-web-app/edit-customer-page';
import {
    CreateCustomerResult, ValidateRefResult, ValidationRefType
} from '@customer-web-app/share-models';
import { BlockUI } from '@customer-web-app/shared-block-ui';
import {
    Currencies, ProductGroupIds, SystemIds, UserLevel
} from '@customer-web-app/shared-constants';
import { ErrorBlock } from '@customer-web-app/shared-error-block';
import {
    GeneralInfo, GeneralInfoActionTypes, generalInformationReducer, initialState
} from '@customer-web-app/shared-general-information-state';
import { DirectMemberGeneralInformation } from '@customer-web-app/shared-general-information-ui';
import { GroupSettingsContextProvider } from '@customer-web-app/shared-group-settings-context';
import { SettingService } from '@customer-web-app/shared-group-settings-services';
import {
    initialState as initialProductGroupsState, ProductGroupsActionTypes, productGroupsReducer
} from '@customer-web-app/shared-group-settings-state';
import { GroupSettingItem } from '@customer-web-app/shared-group-settings-types';
import { GroupSetting, ProductTab, scrollToTop } from '@customer-web-app/shared-group-settings-ui';
import { encrypt } from '@customer-web-app/shared-helpers';
import { Layout, useTheme } from '@customer-web-app/shared/base/layout';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import styles from './create-direct-member-page.module.scss';

/* eslint-disable-next-line */
export interface CreateDirectMemberPageProps {}

export function CreateDirectMemberPage(props: CreateDirectMemberPageProps) {
  const { t } = useTranslation();
  const [isLoading, setIsLoading] = useState(true);
  const theme = useTheme();
  const [currentProductGroupId, setCurrentProductGroupId] = useState(ProductGroupIds.SportsBook);
  const [createCustomerResult, setCreateCustomerResult] = useState<CreateCustomerResult | null>();
  const [numberOfAccounts, setNumberOfAccounts] = useState(1);

  const [generalInformationState, generalInfoDispatch] = useReducer(generalInformationReducer, initialState);
  const [productGroupsState, productGroupsDispatch] = useReducer(productGroupsReducer, initialProductGroupsState);
  const { generalInfo } = generalInformationState;
  const { productGroups } = productGroupsState;
  const generalInfoRef = useRef<ValidationRefType>(null);
  const groupSettingRef = useRef<ValidationRefType>(null);

  const handleGeneralInfoChange = (payload: Partial<GeneralInfo>) => {
    generalInfoDispatch({ type: GeneralInfoActionTypes.Update, payload });
  };

  useEffect(() => {
    const fetchData = async () => {
      const { generalInfo, productGroups } = await SettingService.GetCustomerSetting(
        params.uplineId,
        params.uplineRoleId,
        0,
        false,
        params.siteLabel,
        true
      );
      if (appConfig.systemId !== SystemIds.NAP) {
        generalInfo.newUserName = params.suggestUsername;
      }
      productGroupsDispatch({ type: ProductGroupsActionTypes.Init, payload: productGroups });

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
          roleId: 1,
          uplineCustId: params.uplineId,
          uplineRoleId: params.uplineRoleId,
          group: generalInfo.currency.toString() === Currencies.Won.toString() ? 'dea' : 'a',
        },
      });
      setIsLoading(false);
    };

    fetchData();
  }, []);

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

    productGroupsDispatch({ type: ProductGroupsActionTypes.Update, payload: newProductGroups });
  };

  const handleUpdateSettingItems = (
    productGroupId: number,
    groupTypeId: number,
    groupItems: GroupSettingItem<unknown>[]
  ) => {
    const newProductGroups = handleChangeGroupSettingValue(productGroups, productGroupId, groupTypeId, groupItems);
    productGroupsDispatch({ type: ProductGroupsActionTypes.Update, payload: newProductGroups });
  };

  const handleSelectDirectMember = async (custId: number) => {
    const { productGroups } = await SettingService.GetCustomerSetting(
      custId,
      UserLevel.Member,
      params.uplineRoleId,
      true,
      params.siteLabel,
      true
    );

    productGroupsDispatch({ type: ProductGroupsActionTypes.Update, payload: productGroups });
  };

  const scrollToInvalidGroup = (isValidGeneralInfo: boolean, validateGroupSettingResult: ValidateRefResult) => {
    if (!isValidGeneralInfo) {
      generalInfoRef?.current?.scrollToBlock && generalInfoRef.current.scrollToBlock();
      return;
    }

    if (!validateGroupSettingResult.isValid) {
      groupSettingRef?.current?.scrollToBlock &&
        groupSettingRef.current.scrollToBlock(validateGroupSettingResult.groupTypeId);
    }
  };

  const validateGroupTypes = async () => groupSettingRef.current && (await groupSettingRef.current.validate());

  const handleCreateDirectMember = async () => {
    setIsLoading(true);
    const isValidGeneralInfo = generalInfoRef.current ? ((await generalInfoRef.current.validate()) as boolean) : true;
    const validateGroupSettingResult = (await validateGroupTypes()) as ValidateRefResult;

    if (isValidGeneralInfo && validateGroupSettingResult.isValid) {
      const secretKey = (document.getElementById('SecretKey') as HTMLInputElement).value;
      const encryptedPassword = encrypt(generalInfo.password, secretKey);
      const result = await SettingService.CreateCustomer(
        { ...generalInfo, password: encryptedPassword },
        productGroups,
        true,
        numberOfAccounts
      );
      if (result.isSuccessful) {
        setIsLoading(true);
        const createResult = result as CreateCustomerResult;
        setTimeout(() => {
          window.redirectToSettingPage({ username: generalInfo.userName, custId: createResult.newCustId });
        }, 3000);
        return;
      } else {
        setCreateCustomerResult(result);
        scrollToTop();
      }
    }
    scrollToInvalidGroup(isValidGeneralInfo, validateGroupSettingResult);
    setIsLoading(false);
  };

  const renderCreateButton = () => {
    return (
      <div className={styles.btn}>
        <button type="button" onClick={handleCreateDirectMember}>
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
        <DirectMemberGeneralInformation
          ref={generalInfoRef}
          generalInfo={generalInfo}
          onGeneralInfoChange={handleGeneralInfoChange}
          onSelectDirectMember={handleSelectDirectMember}
          numberOfAccounts={numberOfAccounts}
          setNumberOfAccounts={setNumberOfAccounts}
        />
        <GroupSettingsContextProvider
          value={{
            generalInfo,
            productGroups,
            isMultipleMode: false,
            mode: 'createCustomer',
            handleSettingItemChange: handleSettingItemChange,
            handleUpdateSettingItems: handleUpdateSettingItems,
            productGroupId: currentProductGroupId,
            handleChangeProductGroupId: setCurrentProductGroupId,
          }}
        >
          <ProductTab />
          <GroupSetting ref={groupSettingRef} />
        </GroupSettingsContextProvider>
        {renderCreateButton()}
      </Layout>
    </>
  );
}

export default CreateDirectMemberPage;
