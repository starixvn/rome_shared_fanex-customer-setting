import { GroupItem } from './group-item';

export interface GroupType {
  id: number;
  name?: string;
  displayType?: number;
  groupItems: GroupItem[];
  isReadOnly: boolean;
  isAllowUncheckAll: boolean;
}

export interface CurrentWinLossAmount {
  currentAmount: number;
  lossPercent: number;
  maxLossAmount: number;
  maxMinAmount: number;
  sportType: number;
  sportTypeName: number;
  winPercent: number;
}
