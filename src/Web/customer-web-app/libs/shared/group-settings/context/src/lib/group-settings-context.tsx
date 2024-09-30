import React, { useContext } from 'react';

import { GeneralInfo } from '@customer-web-app/shared-general-information-state';
import { GroupSettingItem, ProductGroup } from '@customer-web-app/shared-group-settings-types';

export type GroupSettingsContextType = {
  generalInfo: GeneralInfo;
  productGroupId: number;
  productGroups: ProductGroup[];
  isMultipleMode: boolean;
  isEditMode: boolean;
  roleId: number;
  updateGroupSetting: (groupTypeId: number) => void;
  handleSettingItemChange: (
    groupId: number,
    groupTypeId: number,
    sportTypeId: number,
    betTypeId: number,
    data: Partial<GroupSettingItem<unknown>>
  ) => void;
  handleUpdateSettingItems: (
    productGroupId: number,
    groupTypeId: number,
    groupItems: GroupSettingItem<unknown>[]
  ) => void;
  mode: 'createSuper' | 'createCustomer' | 'edit' | 'editMultipleCustomers';
};

export const GroupSettingsContext = React.createContext<Partial<GroupSettingsContextType>>({});
GroupSettingsContext.displayName = 'GroupSettingsContext';

export const GroupSettingsContextProvider = GroupSettingsContext.Provider;
export const GroupSettingsContextConsumer = GroupSettingsContext.Consumer;
export const useGroupSettingContext = () => useContext(GroupSettingsContext);
