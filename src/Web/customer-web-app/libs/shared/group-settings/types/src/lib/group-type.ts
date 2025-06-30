import { GroupItem } from './group-item';

export interface GroupType {
  id: number;
  name?: string;
  displayType?: number;
  groupItems: GroupItem[];
  isReadOnly: boolean;
  isAllowUncheckAll: boolean;
}

export interface CurrentWinAmount {
  maxWinAmount: number;
  winPercent: number;
  sportType: number;
  betType: number;
  currentAmount: number;
  sportTypeName: number;
}

export interface CurrentWinLossAmount extends CurrentWinAmount {
  lossPercent: number;
  maxLossAmount: number;
}
