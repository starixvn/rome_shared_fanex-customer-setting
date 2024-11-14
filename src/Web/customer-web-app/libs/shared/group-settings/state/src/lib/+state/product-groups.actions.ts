import { ProductGroup } from '@customer-web-app/shared-group-settings-types';

export enum ProductGroupsActionTypes {
  Init = '[ProductGroups] Init',
  Update = '[ProductGroups] Update',
}

export interface ProductGroupsAction {
  type: ProductGroupsActionTypes;
  payload: ProductGroup[];
}
