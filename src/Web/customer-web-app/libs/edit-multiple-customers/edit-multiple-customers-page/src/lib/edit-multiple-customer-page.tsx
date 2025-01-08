import { useEffect, useReducer, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import {
    getUnCheckAutoPtItems, handleChangeGroupSettingValue, handleChangeItemValue,
    renderUpdateCustomerResult
} from '@customer-web-app/edit-customer-page';
import {
    Result, SubMessagesResult, UpdateCustomerResult, ValidateRefResult, ValidationRefType
} from '@customer-web-app/share-models';
import { BlockUI } from '@customer-web-app/shared-block-ui';
import { GroupTypeIds, UserLevel } from '@customer-web-app/shared-constants';
import { ErrorBlock } from '@customer-web-app/shared-error-block';
import {
    GeneralInfoActionTypes, generalInformationReducer, initialState
} from '@customer-web-app/shared-general-information-state';
import { GroupSettingsContextProvider } from '@customer-web-app/shared-group-settings-context';
import { SettingService } from '@customer-web-app/shared-group-settings-services';
import {
    GroupItem, GroupSettingItem, ProductGroup
} from '@customer-web-app/shared-group-settings-types';
import {
    GroupSetting, scrollPageToTop, SetMemberPtPopup
} from '@customer-web-app/shared-group-settings-ui';
import { AppConfigHelper } from '@customer-web-app/shared-helpers';
import { IconClose } from '@customer-web-app/shared/base/common-icon';
import { Layout, useTheme } from '@customer-web-app/shared/base/layout';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';
import NiceModal from '@ebay/nice-modal-react';

import styles from './edit-multiple-customer-page.module.scss';
import { getNewMultipleEditCustomerSettings, handleInitProducts } from './handler';
import popUpStyles from './popup.module.scss';


      if (confirmTimer === 0) {


                    isCancel: true,
                    isCancel: true,
                  if (confirmTimer == 0) {
                      isCancel: false,
export function EditMultipleCustomerPage() {
  const { t } = useTranslation();
  const theme = useTheme();
  const [generalInformationState, generalInfoDispatch] = useReducer(generalInformationReducer, initialState);
  const { generalInfo } = generalInformationState;
  const [group, setGroup] = useState('');
  const [selectedCommGroup, setSelectedCommGroup] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [refreshData, setRefreshData] = useState(true);
  const [isShowNoChangeMessage, setIsShowNoChangeMessage] = useState(false);
  const [productGroups, setProductGroups] = useState<ProductGroup[]>([]);
  const [updateCustomerResult, setUpdateCustomerResult] = useState<UpdateCustomerResult[] | Result | null>();

  const groupSettingRef = useRef<ValidationRefType>(null);

  useEffect(() => {
    const fetchData = async () => {
      const firstCustomerId = params.isCopyBetSetting ? params.custId : parseInt(params.custIds.split(',')[0]);
      const { generalInfo, group, productGroups } = await SettingService.GetCustomerSetting(
        firstCustomerId,
        params.roleId,
        params.uplineRoleId,
        params.agentId,
        true,
        '',
        true,
        params.groupName,
        params.isCopyBetSetting
      );

      setGroup(group);

      const newProductGroups = handleInitProducts(productGroups);
      setProductGroups(newProductGroups);
      setSelectedCommGroup(false);

      generalInfoDispatch({
        type: GeneralInfoActionTypes.Update,
        payload: {
          ...generalInfo,
          uplineRoleId: params.uplineRoleId,
        },
      });

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

  const showUpdateCustomerResult = (result: UpdateCustomerResult[] | Result) => {
    setUpdateCustomerResult(result);
    scrollToTop();
  };

  const showNoChangeMessage = () => {
    setIsShowNoChangeMessage(true);
    scrollToTop();
  };

  const scrollToTop = () => {
    scrollPageToTop(window);

    const parentWindow = window.parent as any;
    if (parentWindow.scrollPageToTop) {
      parentWindow.scrollPageToTop();
    }
  };

  const validateGroupTypes = async (groupTypeId?: number | undefined) =>
    groupSettingRef.current && (await groupSettingRef.current.validate(groupTypeId));

  const scrollToInvalidGroup = (validateGroupSettingResult: ValidateRefResult) => {
    !validateGroupSettingResult.isValid &&
      groupSettingRef?.current?.scrollToBlock &&
      groupSettingRef.current.scrollToBlock(validateGroupSettingResult.groupTypeId);
  };

  const getProductGroupWithPTOnMembers = async (newProductGroups: ProductGroup[]) => {
    const unCheckAutoPtItems = getUnCheckAutoPtItems(
      newProductGroups,
      params.roleId,
      params.productGroupId,
      GroupTypeIds.PositionTaking
    );

    if (unCheckAutoPtItems.length === 0) return newProductGroups;

    const newMemberPts: { isCancel: boolean; groupItems: GroupItem[] } = await NiceModal.show(SetMemberPtPopup, {
      roleId: params.roleId,
      productGroupId: params.productGroupId,
      groupItems: unCheckAutoPtItems,
    });

    if (newMemberPts.isCancel) {
      return [];
    }

    const currentPTGroup = newProductGroups
      .find((productGroup) => productGroup.id === params.productGroupId)
      ?.groupTypes.find((groupType) => groupType.id === GroupTypeIds.PositionTaking);

    const newPTGroup = currentPTGroup?.groupItems.map((item) => {
      const newMemberPtItem = newMemberPts.groupItems.find(
        (newMemberPt) => newMemberPt.sportTypeId === item.sportTypeId && newMemberPt.betTypeId === item.betTypeId
      );
      if (newMemberPtItem) {
        return { ...newMemberPtItem };
      }

      return item;
    });

    return handleChangeGroupSettingValue(
      newProductGroups,
      params.productGroupId,
      GroupTypeIds.PositionTaking,
      newPTGroup as []
    );
  };

  const handleUpdateCustomer = async (id?: number) => {
    resetStateBeforeUpdate();
    const validateGroupSettingResult = (await validateGroupTypes(id)) as ValidateRefResult;
    if (validateGroupSettingResult.isValid) {
      const newProductGroups = getNewMultipleEditCustomerSettings(productGroups, params.roleId, id);
      const isChangeSetting = newProductGroups.some((productGroup) =>
        productGroup.groupTypes.some((groupType) => groupType.groupItems.length > 0)
      );

      if (isChangeSetting) {
        setIsLoading(false);
        const productGroupWithPTOnMembers = await getProductGroupWithPTOnMembers(newProductGroups);
        if (productGroupWithPTOnMembers.length === 0) {
          setIsLoading(false);
          return;
        }

        if (params.roleId === UserLevel.Super) {
          const settingChangedCount = countSettingChanged(id);
          if (settingChangedCount === 0) {
            setIsShowNoChangeMessage(true);
            return;
          }

          scrollToTop();
          const confirmation: { isCancel: boolean } = await NiceModal.show(UpdateSettingConfirmationPopup, {
            productGroups,
            sportChangedCount: countSportChangedOfSetting(id),
            settingChangedCount: settingChangedCount,
            custCount: params.custIds.split(',').length,
            isCopyBetSetting: params.isCopyBetSetting,
          });

          if (confirmation.isCancel) {
            return;
          }
        }

        await updateSettings(productGroupWithPTOnMembers);
        return;
      } else {
        showNoChangeMessage();
        setIsLoading(false);
        return;
      }
    }

    scrollToInvalidGroup(validateGroupSettingResult);
    setIsLoading(false);
  };

  const updateSettings = async (productGroupWithPTOnMembers: any) => {
    setIsLoading(true);
    const result = AppConfigHelper.isEditBetSettingOnly()
      ? await SettingService.UpdateMultipleSuperBetSetting(
          productGroupWithPTOnMembers,
          params.custId,
          params.custIds,
          params.usernames,
          params.roleId,
          params.uplineRoleId,
          generalInfo.site,
          generalInfo.siteLabel
        )
      : await SettingService.UpdateMultipleCustomers(
          productGroupWithPTOnMembers,
          params.custIds,
          params.usernames,
          params.roleId,
          params.uplineRoleId,
          generalInfo.site,
          generalInfo.siteLabel,
          selectedCommGroup ? group : undefined
        );
    setTimeout(() => {
      showUpdateCustomerResult(result);
      setRefreshData(!refreshData);
      setIsLoading(false);
    }, 3000);
  };

  const countSportChangedOfSetting = (id?: number) => {
    let result = 0;
    for (let i = 0; i < productGroups.length; i++) {
      const group = productGroups[i];
      result += countSportChangedOfProduct(group, id);
    }

    return result;
  };

  const countSportChangedOfProduct = (group: any, id?: number) => {
    let result = 0;
    const settingTypes = [
      GroupTypeIds.SportsBookBetSetting,
      GroupTypeIds.RNGGamesBetSetting,
      GroupTypeIds.OtherGamesBetSetting,
      GroupTypeIds.ParlayMaxPayout,
      GroupTypeIds.BetLimit,
    ];

    for (let i = 0; i < group.groupTypes.length; i++) {
      const g = group.groupTypes[i];
      if (id && id !== g.id) {
        continue;
      }

      if (settingTypes.some((s) => s == g.id) && (!id || id === g.id)) {
        result += countSportChangedOfGroupType(g, id);
      }
    }

    return result;
  };

  const countSportChangedOfGroupType = (group: any, id?: number) => {
    let result = 0;
    const checkedSports: any = {};
    const isExtBetSetting = AppConfigHelper.isEditBetSettingOnly();

    for (let i = 0; i < group.groupItems.length; i++) {
      const gi = group.groupItems[i];

      const sportTypeId = gi.sportTypeId.toString();
      if (group.id === GroupTypeIds.BetLimit && !checkedSports[sportTypeId]) {
        result++;
        checkedSports[sportTypeId] = true;
        continue;
      }

      if (!gi.isMultipleModeChecked) {
        continue;
      }

      if (group.id === GroupTypeIds.ParlayMaxPayout) {
        if ((isExtBetSetting || gi.singleMatchParlayMultipleMode) && gi.singleMatchParlay != null) {
          result++;
          continue;
        }

        if ((isExtBetSetting || gi.combo2MultipleMode) && gi.combo2 != null) {
          result++;
          continue;
        }

        if ((isExtBetSetting || gi.combo35MultipleMode) && gi.combo35 != null) {
          result++;
          continue;
        }

        if ((isExtBetSetting || gi.comboOver6MultipleMode) && gi.comboOver6 != null) {
          result++;
          continue;
        }

        continue;
      }

      if ((isExtBetSetting || gi.minBetMultipleMode) && gi.minBet != null) {
        result++;
        continue;
      }

      if ((isExtBetSetting || gi.maxBetMultipleMode) && gi.maxBet != null) {
        result++;
        continue;
      }

      if ((isExtBetSetting || gi.maxPerMatchMultipleMode) && gi.maxPerMatch != null) {
        result++;
        continue;
      }

      if ((isExtBetSetting || gi.maxPayoutPerMatchMultipleMode) && gi.maxPayoutPerMatch != null) {
        result++;
        continue;
      }

      if ((isExtBetSetting || gi.maxPerBallMultipleMode) && gi.maxPerBall != null) {
        result++;
        continue;
      }

      continue;
    }

    return result;
  };

  const countSettingChanged = (id?: number) => {
    let result = 0;
    for (let i = 0; i < productGroups.length; i++) {
      const group = productGroups[i];
      result += countSettingChangedOfProduct(group, id);
    }

    return result;
  };

  const countSettingChangedOfProduct = (group: any, id?: number) => {
    let result = 0;
    for (let i = 0; i < group.groupTypes.length; i++) {
      const g = group.groupTypes[i];
      if (id && id !== g.id) {
        continue;
      }

      result += countSettingChangedOfGroupType(g, id);
    }

    return result;
  };

  const countSettingChangedOfGroupType = (group: any, id?: number) => {
    let result = 0;

    for (let i = 0; i < group.groupItems.length; i++) {
      const gi = group.groupItems[i];

      if (!gi.isMultipleModeChecked) {
        continue;
      }

      const isExtBetSetting = AppConfigHelper.isEditBetSettingOnly();

      switch (group.id) {
        case GroupTypeIds.SportsBookBetSetting:
        case GroupTypeIds.RNGGamesBetSetting:
        case GroupTypeIds.OtherGamesBetSetting:
          if ((isExtBetSetting || gi.minBetMultipleMode) && gi.minBet != null) {
            result++;
          }

          if ((isExtBetSetting || gi.maxBetMultipleMode) && gi.maxBet != null) {
            result++;
          }

          if ((isExtBetSetting || gi.maxPerMatchMultipleMode) && gi.maxPerMatch != null) {
            result++;
          }

          if ((isExtBetSetting || gi.maxPayoutPerMatchMultipleMode) && gi.maxPayoutPerMatch != null) {
            result++;
          }

          if ((isExtBetSetting || gi.maxPerBallMultipleMode) && gi.maxPerBall != null) {
            result++;
          }

          break;
        case GroupTypeIds.ParlayMaxPayout:
          if ((isExtBetSetting || gi.singleMatchParlayMultipleMode) && gi.singleMatchParlay != null) {
            result++;
          }

          if ((isExtBetSetting || gi.combo2MultipleMode) && gi.combo2 != null) {
            result++;
          }

          if ((isExtBetSetting || gi.combo35MultipleMode) && gi.combo35 != null) {
            result++;
          }

          if ((isExtBetSetting || gi.comboOver6MultipleMode) && gi.comboOver6 != null) {
            result++;
          }

          break;
        case GroupTypeIds.BetLimit:
          result++;
          break;
        default:
          break;
      }
    }

    return result;
  };

  const renderUpdateButton = () => {
    return (
      <div className={styles.btn}>
        <button type="button" onClick={() => handleUpdateCustomer()}>
          {t('Update')}
        </button>
      </div>
    );
  };

  const renderResult = (result: UpdateCustomerResult[] | Result) => {
    if ((result as UpdateCustomerResult[]).length) {
      return (
        <div className={styles['list-status-user-update']}>
          {(result as UpdateCustomerResult[]).map((item) => {
            return (
              <div key={item.custName}>
                {item.message ||
                  (item.subResults && item.subResults.length > 0 && (
                    <div className={styles['name-user-update']}>{item.custName}</div>
                  ))}
                {renderUpdateCustomerResult(item, t)}
              </div>
            );
          })}
        </div>
      );
    }

    if (!(result as Result).isSuccessful) {
      return <ErrorBlock error={result as Result} />;
    }

    return null;
  };

  return (
    <>
      {isLoading && <BlockUI />}
      <Layout theme={theme}>
        <div className={styles['list-user-update']}>
          {params.usernames.split(',').map((item, index) => `${index > 0 ? ', ' : ''}${item}`)}
        </div>
        {renderUpdateButton()}
        {isShowNoChangeMessage && <ErrorBlock error={new Result(1, t('NoChangeMessage')) as SubMessagesResult} />}
        {updateCustomerResult && renderResult(updateCustomerResult)}
        <GroupSettingsContextProvider
          value={{
            generalInfo,
            group,
            handleMemberCommGroupChange: setGroup,
            selectedCommGroup,
            handleSelectCommGroup: setSelectedCommGroup,
            productGroups,
            isMultipleMode: true,
            productGroupId: params.productGroupId,
            mode: 'editMultipleCustomers',
            handleSettingItemChange,
            handleUpdateSettingItems,
            updateGroupSetting: handleUpdateCustomer,
            isEditMode:
              !(params.roleId === UserLevel.Super && AppConfigHelper.IsExternalNap()) ||
              AppConfigHelper.isEditBetSettingOnly(),
            roleId: params.roleId,
            isDirectMember: params.agentId === -3 || params.agentId === -4,
          }}
        >
          <GroupSetting ref={groupSettingRef} />
        </GroupSettingsContextProvider>
        {renderUpdateButton()}
        <WrapperTooltip id="global-tooltip" className={styles['global-tooltip']} />
      </Layout>
    </>
  );
}

export default EditMultipleCustomerPage;
