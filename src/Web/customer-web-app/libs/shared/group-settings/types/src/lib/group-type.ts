import { GroupItem } from './group-item';

export interface GroupType {
  id: number;
  name?: string;
  displayType?: number;
  groupItems: GroupItem[];
  isReadOnly: boolean;
}
