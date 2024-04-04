import { useEffect, useReducer, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import {
    getUnCheckAutoPtItems, handleChangeGroupSettingValue, handleChangeItemValue, renderUpdateCustomerResult, scrollToTop
} from '@customer-web-app/edit-customer-page';
import {
    Result, SubMessagesResult, UpdateCustomerResult, ValidateRefResult, ValidationRefType
} from '@customer-web-app/share-models';
import { BlockUI } from '@customer-web-app/shared-block-ui';
import { GroupTypeIds, ProductGroupIds, UserLevel } from '@customer-web-app/shared-constants';
import { ErrorBlock } from '@customer-web-app/shared-error-block';
import {
    GeneralInfoActionTypes, generalInformationReducer, initialState
} from '@customer-web-app/shared-general-information-state';
import { GroupSettingsContextProvider } from '@customer-web-app/shared-group-settings-context';
import { SettingService } from '@customer-web-app/shared-group-settings-services';
import { GroupItem, GroupSettingItem, ProductGroup } from '@customer-web-app/shared-group-settings-types';
import { GroupSetting, SetMemberPtPopup } from '@customer-web-app/shared-group-settings-ui';
import { AppConfigHelper } from '@customer-web-app/shared-helpers';
import { Layout, useTheme } from '@customer-web-app/shared/base/layout';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';
import NiceModal from '@ebay/nice-modal-react';

import styles from './edit-multiple-customer-page.module.scss';
import { getNewMultipleEditCustomerSettings, handleInitProducts } from './handler';

export function EditMultipleCustomerPage() {
  const { t } = useTranslation();
  const theme = useTheme();
  const [generalInformationState, generalInfoDispatch] = useReducer(generalInformationReducer, initialState);
  const { generalInfo } = generalInformationState;

  const [isLoading, setIsLoading] = useState(true);
  const [refreshData, setRefreshData] = useState(true);
  const [isShowNoChangeMessage, setIsShowNoChangeMessage] = useState(false);
  const [productGroups, setProductGroups] = useState<ProductGroup[]>([]);
  const [updateCustomerResult, setUpdateCustomerResult] = useState<UpdateCustomerResult[] | Result | null>();

  const groupSettingRef = useRef<ValidationRefType>(null);

  useEffect(() => {
    const fetchData = async () => {
      const firstCustomerId = params.isCopyBetSetting ? params.custId : parseInt(params.custIds.split(',')[0]);
      const { generalInfo, productGroups } = await SettingService.GetCustomerSetting(
        firstCustomerId,
        params.roleId,
        params.uplineRoleId,
        true
      );
      const newProductGroups = handleInitProducts(productGroups);
      setProductGroups(newProductGroups);

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
      .find((productGroup) => productGroup.id === ProductGroupIds.SportsBook)
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
      ProductGroupIds.SportsBook,
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
        setIsLoading(true);
        const result = AppConfigHelper.isEditBetSettingOnly()
          ? await SettingService.UpdateMultipleSuperBetSetting(
              productGroupWithPTOnMembers,
              params.custId,
              params.custIds,
              params.usernames,
              params.roleId,
              params.uplineRoleId
            )
          : await SettingService.UpdateMultipleCustomers(
              productGroupWithPTOnMembers,
              params.custId,
              params.custIds,
              params.usernames,
              params.roleId,
              params.uplineRoleId
            );
        setTimeout(() => {
          showUpdateCustomerResult(result);
          setRefreshData(!refreshData);
          setIsLoading(false);
        }, 3000);
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
