import { ProductGroup } from '@customer-web-app/shared-group-settings-types';

export enum ProductGroupsActionTypes {
  InitCreate = '[ProductGroups] InitCreate',
  InitEdit = '[ProductGroups] InitEdit',
  Update = '[ProductGroups] Update',
}

export interface ProductGroupsAction {
  type: ProductGroupsActionTypes;
  payload: ProductGroup[];
}
