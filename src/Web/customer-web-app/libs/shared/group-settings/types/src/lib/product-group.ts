import { GroupType } from './group-type';

export interface ProductGroup {
  id: number;
  name?: string;
  groupTypes: GroupType[];
}
