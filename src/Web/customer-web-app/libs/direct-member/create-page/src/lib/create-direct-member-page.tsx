import _ from 'lodash';
import { useEffect, useReducer, useRef, useState } from 'react';

import {
    handleChangeGroupSettingValue, handleChangeItemValue
} from '@customer-web-app/edit-customer-page';
import { ValidationRefType } from '@customer-web-app/share-models';
import { BlockUI } from '@customer-web-app/shared-block-ui';
import {
    Currencies, ProductGroupIds, SystemIds, UserLevel
} from '@customer-web-app/shared-constants';
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
import { GroupSetting, ProductTab } from '@customer-web-app/shared-group-settings-ui';
import { Layout, useTheme } from '@customer-web-app/shared/base/layout';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import styles from './create-direct-member-page.module.scss';

/* eslint-disable-next-line */
export interface CreateDirectMemberPageProps {}

export function CreateDirectMemberPage(props: CreateDirectMemberPageProps) {
  const [isLoading, setIsLoading] = useState(true);
  const theme = useTheme();
  const [currentProductGroupId, setCurrentProductGroupId] = useState(ProductGroupIds.SportsBook);

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
    const { productGroups: selectedProductGroups } = await SettingService.GetCustomerSetting(
      custId,
      UserLevel.Member,
      params.uplineRoleId,
      true,
      params.siteLabel,
      true
    );

    const newProductGroups = _.merge(productGroups, selectedProductGroups);
    console.log(newProductGroups);
  };

  return (
    <>
      {isLoading && <BlockUI />}
      <WrapperTooltip id="global-tooltip" className={styles['global-tooltip']} />
      <Layout theme={theme}>
        <DirectMemberGeneralInformation
          ref={generalInfoRef}
          generalInfo={generalInfo}
          onGeneralInfoChange={handleGeneralInfoChange}
          onSelectDirectMember={handleSelectDirectMember}
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
      </Layout>
    </>
  );
}

export default CreateDirectMemberPage;
