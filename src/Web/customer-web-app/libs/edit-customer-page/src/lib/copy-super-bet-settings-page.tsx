import { useEffect, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import {
  Result,
  SubMessagesResult,
  UpdateCustomerResult,
  ValidateRefResult,
  ValidationRefType,
} from '@customer-web-app/share-models';
import { BlockUI } from '@customer-web-app/shared-block-ui';
import { GroupTypeIds, UserLevel } from '@customer-web-app/shared-constants';
import { ErrorBlock } from '@customer-web-app/shared-error-block';
import { GeneralInfo } from '@customer-web-app/shared-general-information-state';
import { GroupSettingsContextProvider } from '@customer-web-app/shared-group-settings-context';
import { SettingService } from '@customer-web-app/shared-group-settings-services';
import { getChanges } from '@customer-web-app/shared-group-settings-state';
import {
  GroupSettingItem,
  ParlayMaxPayoutGroupItem,
  ProductGroup,
  SportsBookBetSettingGroupItem,
} from '@customer-web-app/shared-group-settings-types';
import { GroupSetting, scrollToTop } from '@customer-web-app/shared-group-settings-ui';
import { AppConfigHelper } from '@customer-web-app/shared-helpers';
import { Layout, useTheme } from '@customer-web-app/shared/base/layout';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import styles from './edit-customer-page.module.scss';
import { handleChangeGroupSettingValue, handleChangeItemValue } from './handler';
import { renderUpdateCustomerResult } from './update-customer-result';

const getSuperBetSettings = async () => {
  const { custId, roleId, uplineRoleId } = params;
  return await SettingService.GetCustomerSetting(custId, roleId, uplineRoleId, true);
};

const handleInitSuperBetSettings = (productGroups: ProductGroup[]) => {
  return productGroups?.map((productGroup) => {
    const groupTypes = productGroup.groupTypes
      .filter((groupType) => groupType != null && groupType.id === GroupTypeIds.SportsBookBetSetting)
      .map((groupType) => {
        if (groupType.id === GroupTypeIds.ParlayMaxPayout) {
          const groupItems = (groupType.groupItems as ParlayMaxPayoutGroupItem[]).map((item) => ({
            ...item,
            isMultipleModeChecked: true,
            original: { ...item },
            isAllowUnlimitCombo2: item.combo2 === 0,
            isAllowUnlimitCombo35: item.combo35 === 0,
            isAllowUnlimitComboOver6: item.comboOver6 === 0,
            isAllowUnlimitSingleMatchParlay: item.singleMatchParlay === 0,
            isDisableCombo2: true,
            isDisableCombo35: true,
            isDisableComboOver6: false,
            isDisableSingleMatchParlay: false,
          }));

          return {
            ...groupType,
            groupItems,
            combo2ColChecked: false,
            combo35ColChecked: false,
            comboOver6ColChecked: false,
            singleMatchParlayColChecked: false,
          };
        }

        if (groupType.id === GroupTypeIds.SportsBookBetSetting) {
          const groupItems = (groupType.groupItems as SportsBookBetSettingGroupItem[]).map((item) => ({
            ...item,
            isDisableMinBet: true,
            isDisableMaxBet: true,
            isDisableMaxPerMatch: false,
            isDisableMaxPayoutPerMatch: false,
          }));

          return {
            ...groupType,
            groupItems,
            minBetColChecked: false,
            maxBetColChecked: false,
            maxPerMatchColChecked: false,
            maxPayoutPerMatchColChecked: false,
          };
        }

        return {
          ...groupType,
          groupItems: groupType.groupItems.map((item) => ({
            ...item,
            original: { ...item },
            isMultipleModeChecked: true,
          })),
        };
      });

    return { ...productGroup, groupTypes };
  });
};

export function CopySuperBetSettingsPage() {
  const theme = useTheme();
  const { t } = useTranslation();

  const [isLoading, setIsLoading] = useState(true);
  const [refreshData, setRefreshData] = useState(true);
  const [isShowNoChangeMessage, setIsShowNoChangeMessage] = useState(false);

  const [generalInfo, setGeneralInfo] = useState<GeneralInfo>();
  const [productGroups, setProductGroups] = useState<ProductGroup[]>([]);
  const [updateCustomerResult, setUpdateCustomerResult] = useState<UpdateCustomerResult | null>();

  const groupSettingRef = useRef<ValidationRefType>(null);

  useEffect(() => {
    const fetchData = async () => {
      const { generalInfo, productGroups } = await getSuperBetSettings();
      const newProductGroups = handleInitSuperBetSettings(productGroups);
      console.log(newProductGroups);
      setGeneralInfo(generalInfo);
      setProductGroups(newProductGroups);
      setIsLoading(false);
    };

    fetchData();
  }, [refreshData]);

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

  const resetStateBeforeUpdate = () => {
    setIsLoading(true);
    setUpdateCustomerResult(null);
    setIsShowNoChangeMessage(false);
  };

  const showNoChangeMessage = () => {
    setIsShowNoChangeMessage(true);
    scrollToTop();
  };

  const showUpdateCustomerResult = (result: UpdateCustomerResult) => {
    setUpdateCustomerResult(result);
    scrollToTop();
  };

  const getCustomerSettingChange = (groupTypeId?: number) => {
    return getChanges(params.roleId, productGroups, groupTypeId);
  };

  const validateGroupTypes = async (groupTypeId?: number | undefined) =>
    groupSettingRef.current && (await groupSettingRef.current.validate(groupTypeId));

  const handleUpdateGroupType = async (groupTypeId: number) => {
    resetStateBeforeUpdate();
    const validateGroupSettingResult = (await validateGroupTypes(groupTypeId)) as ValidateRefResult;
    if (validateGroupSettingResult.isValid) {
      const newProductGroups = getCustomerSettingChange(groupTypeId);
      const isChanged = newProductGroups.some((x) => x.groupTypes.some((t) => t.groupItems.length > 0));
      if (isChanged) {
        setIsLoading(true);
        const result =
          generalInfo?.site &&
          generalInfo?.siteLabel &&
          (await SettingService.UpdateCustomer(
            null,
            newProductGroups,
            params.custId,
            params.roleId,
            params.uplineRoleId,
            generalInfo.site,
            generalInfo.siteLabel
          ));
        setTimeout(() => {
          setRefreshData(!refreshData);
          result && showUpdateCustomerResult(result);
          setIsLoading(false);
        }, 3000);
        return;
      } else {
        showNoChangeMessage();
        setIsLoading(false);
        return;
      }
    }
    setIsLoading(false);
  };

  return (
    <>
      {isLoading && <BlockUI />}
      <Layout theme={theme}>
        {isShowNoChangeMessage && <ErrorBlock error={new Result(1, t('NoChangeMessage')) as SubMessagesResult} />}
        {updateCustomerResult && renderUpdateCustomerResult(updateCustomerResult, t)}
        <GroupSettingsContextProvider
          value={{
            generalInfo,
            productGroups,
            isMultipleMode: false,
            mode: 'edit',
            handleSettingItemChange,
            handleUpdateSettingItems,
            productGroupId: 1,
            updateGroupSetting: handleUpdateGroupType,
            isEditMode:
              !(params.roleId === UserLevel.Super && AppConfigHelper.IsExternalNap()) ||
              AppConfigHelper.isEditBetSettingOnly(),
          }}
        >
          <GroupSetting ref={groupSettingRef} />
        </GroupSettingsContextProvider>
        <WrapperTooltip id="global-tooltip" className={styles['global-tooltip']} />
      </Layout>
    </>
  );
}

export default CopySuperBetSettingsPage;
