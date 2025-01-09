import { useEffect, useReducer, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import {
  Params,
  Result,
  SubMessagesResult,
  UpdateCustomerResult,
  ValidateRefResult,
  ValidationRefType,
} from '@customer-web-app/share-models';
import { BlockUI } from '@customer-web-app/shared-block-ui';
import { GroupTypeIds, ProductGroupIds, UserLevel } from '@customer-web-app/shared-constants';
import { ErrorBlock } from '@customer-web-app/shared-error-block';
import {
  GeneralInfo,
  GeneralInfoActionTypes,
  generalInformationReducer,
  initialState,
} from '@customer-web-app/shared-general-information-state';
import {
  EditGeneralInformation,
  TransferCondition,
  TransferOptions,
} from '@customer-web-app/shared-general-information-ui';
import { GroupSettingsContextProvider } from '@customer-web-app/shared-group-settings-context';
import { SettingService } from '@customer-web-app/shared-group-settings-services';
import { getChanges } from '@customer-web-app/shared-group-settings-state';
import { GroupItem, GroupSettingItem, ProductGroup } from '@customer-web-app/shared-group-settings-types';
import { GroupSetting, scrollToTop, SetMemberPtPopup } from '@customer-web-app/shared-group-settings-ui';
import { AppConfigHelper } from '@customer-web-app/shared-helpers';
import { Layout, useTheme } from '@customer-web-app/shared/base/layout';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';
import NiceModal from '@ebay/nice-modal-react';

import styles from './edit-customer-page.module.scss';
import {
  getNewIndirectEditCustomerSettings,
  getUnCheckAutoPtItems,
  handleChangeGroupSettingValue,
  handleChangeItemValue,
  handleInitProducts,
  isGeneralInfoChanged,
} from './handler';
import { renderUpdateCustomerResult } from './update-customer-result';

export function EditCustomerPage() {
  const { t } = useTranslation();
  const theme = useTheme();
  const [generalInformationState, generalInfoDispatch] = useReducer(generalInformationReducer, initialState);
  const { generalInfo } = generalInformationState;

  const [isLoading, setIsLoading] = useState(true);
  const [refreshData, setRefreshData] = useState(true);
  const [isShowNoChangeMessage, setIsShowNoChangeMessage] = useState(false);
  const [originalGeneralInfo, setOriginalGeneralInfo] = useState<GeneralInfo>();
  const [productGroups, setProductGroups] = useState<ProductGroup[]>([]);
  const [updateCustomerResult, setUpdateCustomerResult] = useState<UpdateCustomerResult | null>();
  const [group, setGroup] = useState('');

  const generalInfoRef = useRef<ValidationRefType>(null);
  const groupSettingRef = useRef<ValidationRefType>(null);
  const transferRef = useRef<ValidationRefType>(null);

  useEffect(() => {
    const fetchData = async () => {
      const { generalInfo, group, productGroups } = await SettingService.GetCustomerSetting(
        params.custId,
        params.roleId,
        params.uplineRoleId,
        true
      );

      const newProductGroups = handleInitProducts(productGroups);
      setProductGroups(newProductGroups);

      setOriginalGeneralInfo(generalInfo);
      generalInfoDispatch({
        type: GeneralInfoActionTypes.Update,
        payload: {
          ...generalInfo,
          uplineRoleId: params.uplineRoleId,
          displayTransferCondition: generalInfo.transferOption === TransferOptions.Daily,
        },
      });
      setGroup(group);
      validateGeneralInfo();
      setIsLoading(false);
    };

    fetchData();
  }, [refreshData]);

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

  const validateGeneralInfo = async () => generalInfoRef.current && (await generalInfoRef.current.validate());

  const handleUpdateGeneralInfo = async () => {
    resetStateBeforeUpdate();
    if (await validateGeneralInfo()) {
      if (isGeneralInfoChanged(generalInfo, originalGeneralInfo)) {
        const result = await SettingService.UpdateCustomer(
          generalInfo,
          [],
          params.custId,
          params.roleId,
          params.uplineRoleId,
          generalInfo.site,
          generalInfo.siteLabel
        );
        showUpdateCustomerResult(result);
        setRefreshData(!refreshData);
      } else {
        showNoChangeMessage();
      }
    }
    setIsLoading(false);
  };

  const isIndirectUpdate = (params: Params) => {
    if (
      (params.uplineRoleId === UserLevel.Super || params.uplineRoleId === UserLevel.Master) &&
      params.roleId === UserLevel.Member
    ) {
      return true;
    }

    return false;
  };

  const getCustomerSettingChange = (groupTypeId?: number) => {
    return isIndirectUpdate(params)
      ? getNewIndirectEditCustomerSettings(productGroups, params.uplineRoleId, GroupTypeIds.PositionTaking)
      : getChanges(params.roleId, productGroups, groupTypeId);
  };

  const validateGroupTypes = async (groupTypeId?: number | undefined) =>
    groupSettingRef.current && (await groupSettingRef.current.validate(groupTypeId));

  const getProductGroupWithPTOnMembers = async (newProductGroups: ProductGroup[]) => {
    if (params.roleId === UserLevel.Master || params.roleId === UserLevel.Agent) {
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
    }

    return newProductGroups;
  };

  const handleUpdateGroupType = async (groupTypeId: number) => {
    resetStateBeforeUpdate();
    const validateGroupSettingResult = (await validateGroupTypes(groupTypeId)) as ValidateRefResult;
    if (validateGroupSettingResult.isValid) {
      const newProductGroups = getCustomerSettingChange(groupTypeId);
      const isChanged = newProductGroups.some((x) => x.groupTypes.some((t) => t.groupItems.length > 0));
      if (isChanged) {
        setIsLoading(false);
        const productGroupWithPTOnMembers = await getProductGroupWithPTOnMembers(newProductGroups);
        if (productGroupWithPTOnMembers.length === 0) {
          setIsLoading(false);
          return;
        }
        setIsLoading(true);
        const result = AppConfigHelper.isEditBetSettingOnly()
          ? await SettingService.UpdateSuperBetSetting(
              productGroupWithPTOnMembers,
              params.custId,
              params.roleId,
              params.uplineRoleId,
              generalInfo.site,
              generalInfo.siteLabel
            )
          : await SettingService.UpdateCustomer(
              null,
              productGroupWithPTOnMembers,
              params.custId,
              params.roleId,
              params.uplineRoleId,
              generalInfo.site,
              generalInfo.siteLabel
            );
        setTimeout(() => {
          setRefreshData(!refreshData);
          showUpdateCustomerResult(result);
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

  const scrollToInvalidGroup = (isValidGeneralInfo: boolean, validateGroupSettingResult: ValidateRefResult) => {
    if (!isValidGeneralInfo) {
      generalInfoRef?.current?.scrollToBlock && generalInfoRef.current.scrollToBlock();
      return;
    }

    !validateGroupSettingResult.isValid &&
      groupSettingRef?.current?.scrollToBlock &&
      groupSettingRef.current.scrollToBlock(validateGroupSettingResult.groupTypeId);
  };

  const handleUpdate = async () => {
    resetStateBeforeUpdate();
    const isEditBetSetting = AppConfigHelper.isEditBetSettingOnly();
    const isValidGeneralInfo =
      !isEditBetSetting && params.productGroupId === ProductGroupIds.SportsBook ? await validateGeneralInfo() : true;
    const validateGroupSettingResult = (await validateGroupTypes()) as ValidateRefResult;
    if (isValidGeneralInfo && validateGroupSettingResult.isValid) {
      const newProductGroups = getCustomerSettingChange();
      const isGeneralInfoDataChanged = isGeneralInfoChanged(generalInfo, originalGeneralInfo);
      if (isGeneralInfoDataChanged || newProductGroups.some((x) => x.groupTypes.some((t) => t.groupItems.length > 0))) {
        const newGeneralInfo = !isEditBetSetting && isGeneralInfoDataChanged ? generalInfo : null;
        setIsLoading(false);
        const productGroupWithPTOnMembers = await getProductGroupWithPTOnMembers(newProductGroups);
        if (productGroupWithPTOnMembers.length === 0) {
          setIsLoading(false);
          return;
        }
        setIsLoading(true);
        const result = isEditBetSetting
          ? await SettingService.UpdateSuperBetSetting(
              productGroupWithPTOnMembers,
              params.custId,
              params.roleId,
              params.uplineRoleId,
              generalInfo.site,
              generalInfo.siteLabel
            )
          : await SettingService.UpdateCustomer(
              newGeneralInfo,
              productGroupWithPTOnMembers,
              params.custId,
              params.roleId,
              params.uplineRoleId,
              generalInfo.site,
              generalInfo.siteLabel
            );
        setTimeout(() => {
          setRefreshData(!refreshData);
          showUpdateCustomerResult(result);
          setIsLoading(false);
        }, 3000);
        return;
      } else {
        showNoChangeMessage();
        setIsLoading(false);
        return;
      }
    } else {
      scrollToInvalidGroup(isValidGeneralInfo as boolean, validateGroupSettingResult);
      setIsLoading(false);
    }
  };

  const renderGeneralInfo = () => {
    if (!AppConfigHelper.isEditBetSettingOnly()) {
      return (
        <EditGeneralInformation
          ref={generalInfoRef}
          generalInfo={generalInfo}
          productGroupId={params.productGroupId}
          isReadOnly={
            params.productGroupId !== ProductGroupIds.SportsBook ||
            (params.roleId === UserLevel.Super && AppConfigHelper.IsExternalNap()) ||
            isIndirectUpdate(params)
          }
          onGeneralInfoChange={handleGeneralInfoChange}
          onUpdate={handleUpdateGeneralInfo}
        />
      );
    }

    return null;
  };

  const renderUpdateButton = () => {
    return (
      (!(params.roleId === UserLevel.Super && AppConfigHelper.IsExternalNap()) ||
        AppConfigHelper.isEditBetSettingOnly()) && (
        <div className={styles.btn}>
          <button type="button" onClick={() => handleUpdate()}>
            {t('Update')}
          </button>
        </div>
      )
    );
  };

  return (
    <>
      {isLoading && <BlockUI />}
      <Layout theme={theme}>
        {renderUpdateButton()}
        {isShowNoChangeMessage && <ErrorBlock error={new Result(1, t('NoChangeMessage')) as SubMessagesResult} />}
        {updateCustomerResult && renderUpdateCustomerResult(updateCustomerResult, t)}
        {renderGeneralInfo()}
        <GroupSettingsContextProvider
          value={{
            generalInfo,
            group,
            handleMemberCommGroupChange: setGroup,
            productGroups,
            isMultipleMode: false,
            mode: 'edit',
            handleSettingItemChange,
            handleUpdateSettingItems,
            productGroupId: params.productGroupId,
            updateGroupSetting: handleUpdateGroupType,
            isEditMode:
              !(params.roleId === UserLevel.Super && AppConfigHelper.IsExternalNap()) ||
              AppConfigHelper.isEditBetSettingOnly(),
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
        <WrapperTooltip id="global-tooltip" className={styles['global-tooltip']} />
        {renderUpdateButton()}
      </Layout>
    </>
  );
}

export default EditCustomerPage;
