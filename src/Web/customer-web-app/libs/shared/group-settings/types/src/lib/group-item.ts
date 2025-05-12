export interface GroupItem {  
  sportTypeId: number;
  sportTypeName?: string;
  betTypeId: number;
  betTypeName?: string;
  displayName?: string;
  subGroupId?: number;
  subGroupName?: string;
  isLive: boolean;
  isMultipleModeChecked: boolean;

  minBetMultipleMode: boolean;
  maxBetMultipleMode: boolean;
  maxPerMatchMultipleMode: boolean;
  maxPayoutPerMatchMultipleMode: boolean;
  singleMatchParlayMultipleMode: boolean;
  combo2MultipleMode: boolean;
  combo35MultipleMode: boolean;
  comboOver6MultipleMode: boolean;
  maxPerBallMultipleMode: boolean;
}

export interface GroupSettingItem<T> extends GroupItem {
  upline: T;
  original: T;
}

export interface ProductStatusProps {
  isDisabled: boolean;
  isDisabledUpline: boolean;
  parentSportType?: number;
  parentBetType?: number;
  readonly: boolean;
}

export interface ProductStatusGroupItem extends ProductStatusProps, GroupSettingItem<ProductStatusProps> {}

export interface BetLimitProps {
  isChecked: boolean;
  isMultipleChoice: number;
  maxChoice: number;
  productId: number;
  productName: string;
  stakeLevelId: number;
  stakeLevelName: number;
}

export interface BetLimitGroupItem extends BetLimitProps, GroupSettingItem<BetLimitProps> {}

export interface WinLossLimitProps {
  isAutoReset: boolean;
  isAutoResetUpline: boolean;
  lossLimit: number;
  lossLimitValue: number;
  maxLossRange: number;
  maxWinRange: number;
  minLossRange: number;
  productId: number;
  productName: string;
  winLimit: number;
}
export interface WinLimitProps extends WinLossLimitProps {
  winLimitValue: number;
  minWinRange: number;
}

export interface WinLossLimitGroupItem extends WinLossLimitProps, GroupSettingItem<WinLossLimitProps> {}

export interface WinLimitGroupItem extends WinLimitProps, GroupSettingItem<WinLimitProps> {}

export interface CommissionProps {
  discount?: number;
  discountMember?: number;
  upline: CommissionProps;
}

export interface CommissionGroupItem extends CommissionProps, GroupSettingItem<CommissionProps> {}

export interface BetSettingsProps {
  minBet: number;
  maxBet: number;
  maxPerMatch: number;
}

export interface SportsBookBetSettingProps extends BetSettingsProps {
  maxPayoutPerMatch?: number;
}

export interface SportsBookBetSettingGroupItem
  extends SportsBookBetSettingProps,
    GroupSettingItem<SportsBookBetSettingProps> {}

export interface RNGGamesBetSettingProps extends BetSettingsProps {
  maxPerBall?: number;
}

export interface RNGGamesBetSettingGroupItem
  extends RNGGamesBetSettingProps,
    GroupSettingItem<RNGGamesBetSettingProps> {}

export interface ParlayMaxPayoutProps {
  combo2: number;
  isAllowUnlimitCombo2: boolean;
  combo35: number;
  isAllowUnlimitCombo35: boolean;
  comboOver6: number;
  isAllowUnlimitComboOver6: boolean;
  singleMatchParlay: number;
  isAllowUnlimitSingleMatchParlay: boolean;
}


export type ParlayMaxPayoutKey = keyof ParlayMaxPayoutProps;

export const ColumnsParlayMaxPayout: ReadonlyArray<{
  inputFieldName: 'singleMatchParlay' | 'combo2' | 'combo35' | 'comboOver6';
  checkBoxFieldName:
    | 'isAllowUnlimitSingleMatchParlay'
    | 'isAllowUnlimitCombo2'
    | 'isAllowUnlimitCombo35'
    | 'isAllowUnlimitComboOver6';
}> = [
  {
    inputFieldName: 'singleMatchParlay',
    checkBoxFieldName: 'isAllowUnlimitSingleMatchParlay',
  },
  {
    inputFieldName: 'combo2',
    checkBoxFieldName: 'isAllowUnlimitCombo2',
  },
  {
    inputFieldName: 'combo35',
    checkBoxFieldName: 'isAllowUnlimitCombo35',
  },
  {
    inputFieldName: 'comboOver6',
    checkBoxFieldName: 'isAllowUnlimitComboOver6',
  },
];

export interface ParlayMaxPayoutGroupItem extends ParlayMaxPayoutProps, GroupSettingItem<ParlayMaxPayoutProps> {
  combo2: number;
  isAllowUnlimitCombo2: boolean;
  combo35: number;
  isAllowUnlimitCombo35: boolean;
  comboOver6: number;
  isAllowUnlimitComboOver6: boolean;
  singleMatchParlay: number;
  isAllowUnlimitSingleMatchParlay: boolean;
}


export interface PotentialWinLimitProps {
  winLimit: number;
  isAllowUnlimitWinLimit: boolean;
  isAutoReset: boolean;
}

export type PotentialWinLimitKey = keyof PotentialWinLimitProps;

export const ColumnsPotentialWinLimit: ReadonlyArray<{
  inputFieldName: 'winLimit';
  checkBoxFieldName:
    | 'isAllowUnlimitWinLimit'
}> = [
  {
    inputFieldName: 'winLimit',
    checkBoxFieldName: 'isAllowUnlimitWinLimit',
  }
];

export interface PotentialWinLimitGroupItem extends PotentialWinLimitProps, GroupSettingItem<PotentialWinLimitProps> {  
  winLimit: number;
  isAllowUnlimitWinLimit: boolean;
  isAutoReset: boolean;
  isAutoResetUpline: boolean;
  accumAmount: number;
  accumToWin: number;
  accumWinLoss: number;
  productId: number;
  productName: string;
  isHideAccumSection: boolean;
}


export interface PositionTakingProps {
  step: number;
}

export interface SuperEditDirectMemberPositionTakingProps extends PositionTakingProps {
  s4: number;
  n4: number;
  iS3: boolean;
  s3: number;
  uS3: number;
  m3: number;
  iS1: boolean;
  uS1: number;
  s1: number;
  m1: number;
}

export interface SuperEditDirectMemberPositionTakingGroupItem
  extends SuperEditDirectMemberPositionTakingProps,
    GroupSettingItem<SuperEditDirectMemberPositionTakingProps> {}

export interface MasterDirectMemberPositionTakingProps extends PositionTakingProps {
  m3: number;
  n3: number;
  m1: number;
}

export interface MasterDirectMemberPositionTakingGroupItem
  extends MasterDirectMemberPositionTakingProps,
    GroupSettingItem<MasterDirectMemberPositionTakingProps> {}

export interface SuperDirectMemberPositionTakingProps extends PositionTakingProps {
  s4: number;
  n4: number;
  s1: number;
}

export interface SuperDirectMemberPositionTakingGroupItem
  extends SuperDirectMemberPositionTakingProps,
    GroupSettingItem<SuperDirectMemberPositionTakingProps> {}

export interface SuperPositionTakingProps extends PositionTakingProps {
  isAuto: boolean;
  s4: number;
  m4: number;
  a4: number;
  n4: number;
  defaultMaxPT: number;
}

export interface SuperPositionTakingGroupItem
  extends SuperPositionTakingProps,
    GroupSettingItem<SuperPositionTakingProps> {}

export interface SuperEditMemberPositionTakingProps extends PositionTakingProps {
  a1: number;
  iS1: boolean;
  iS3: boolean;
  m1: number;
  m3: number;
  n4: number;
  s1: number;
  s4: number;
  uS1: number;
}

export interface SuperEditMemberPositionTakingGroupItem
  extends SuperEditMemberPositionTakingProps,
    GroupSettingItem<SuperEditMemberPositionTakingProps> {}

export interface MasterPositionTakingProps extends PositionTakingProps {
  iS3: boolean;
  m3: number;
  m4: number;
  n3: number;
  n4: number;
  s3: number;
  s4: number;
  uS3: number;
  isUpdateMinPtOnly: boolean;
  isUpdateSuperPTOnMembers: boolean;
  superPTOnMembers: number;
}

export interface MasterPositionTakingGroupItem
  extends MasterPositionTakingProps,
    GroupSettingItem<MasterPositionTakingProps> {}

export interface MasterEditMemberPositionTakingProps extends PositionTakingProps {
  a1: number;
  iM1: boolean;
  iM2: boolean;
  m1: number;
  m3: number;
  n3: number;
  uM1: number;
}

export interface MasterEditMemberPositionTakingGroupItem
  extends MasterEditMemberPositionTakingProps,
    GroupSettingItem<MasterEditMemberPositionTakingProps> {}

export interface AgentPositionTakingProps extends PositionTakingProps {
  a2: number;
  a4: number;
  iM2: boolean;
  m2: number;
  m3: number;
  n2: number;
  n3: number;
  uM2: number;
  isUpdateMinPtOnly: boolean;
  isUpdateMasterPTOnMembers: boolean;
  masterPTOnMembers: number;
}

export interface AgentPositionTakingGroupItem
  extends AgentPositionTakingProps,
    GroupSettingItem<AgentPositionTakingProps> {}

export interface MemberPositionTakingProps extends PositionTakingProps {
  a1: number;
  a2: number;
  n2: number;
}

export interface MemberPositionTakingGroupItem
  extends MemberPositionTakingProps,
    GroupSettingItem<MemberPositionTakingProps> {}
