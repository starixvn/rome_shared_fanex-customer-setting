export enum UserLevel {
  Company = 5,
  Super = 4,
  Master = 3,
  Agent = 2,
  Member = 1,
}

export const UserLevelName: { [key: number]: string } = {
  1: 'Member',
  2: 'Agent',
  3: 'Master',
  4: 'Super',
  5: 'Company',
};

export const ProductGroupIds = {
  SportsBook: 1,
  RNGGames: 2,
  LiveAndRNGCasino: 3,
};

export const isSportsBookProduct = (productGroupId: number | undefined) =>
  productGroupId === ProductGroupIds.SportsBook;

export const GroupTypeIds = {
  SportsBookCommission: 150,
  SportsBookBetSetting: 151,
  ParlayMaxPayout: 152,
  PositionTaking: 153,
  RNGGamesBetSetting: 154,
  Commission: 156,
  OtherGamesBetSetting: 157,
  ProductStatus: 160,
  BetLimit: 158,
  WinLossLimit: 155,
  WinLimit: 161,
  PotentialWinLimit: 162,
};

export const ProductIds = {
  LiveCasino: 41,
};

export const SportTypeIds = {
  Soccer: 1,
  Basketball: 2,
  ESports: 43,
  MuayThai: 44,
  LiveCasino: 9911,
};

export const BetTypeIds = {
  Handicap: 1,
};

export enum SystemIds {
  NAP = 1,
  Alpha = 2,
  InternalAlpha = 3,
  ExternalNAP = 4,
  ExternalNAPBetSetting = 5,
  y789Alpha = 6,
  y789InternalAlpha = 7,
  ExternalNapSpecialSite = 8,
  ExternalNAPBetSettingSpecialSite = 9,
}
export const SpecialSystemIds = [
  SystemIds.y789Alpha,
  SystemIds.y789InternalAlpha,
  SystemIds.ExternalNapSpecialSite,
  SystemIds.ExternalNAPBetSettingSpecialSite,
];

export const AlphaSystemIds = [
  SystemIds.Alpha,
  SystemIds.y789Alpha,
  SystemIds.y789InternalAlpha,
  SystemIds.InternalAlpha,
];
export const ExternalNapSystemIds = [
  SystemIds.ExternalNAP,
  SystemIds.ExternalNAPBetSetting,
  SystemIds.ExternalNapSpecialSite,
  SystemIds.ExternalNAPBetSettingSpecialSite,
];

export const MaxNumberInputLength = 18;
export const i7Markets = ['789y'];
export const i7Sites = ['yUT', 'y789', 'y889', 'yOther', '789y'];

export const Currencies = {
  Won: '45',
};

export const DirectMemberAgentIds = {
  Super: -4,
  Master: -3,
};

export enum ServiceTypes {
  Credit = 0,
  Cash = 1,
}

export const PHONE_CODE_MAX_LENGTH = 3;
export const PHONE_NUMBER_MAX_LENGTH = 40;
export const NUMBER_REGEX = /[^0-9]/g;
export const EMAIL_REGEX = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
export const EMAIL_MAX_LENGTH = 50;
export const FIRST_NAME_MAX_LENGTH = 50;
export const LAST_NAME_MAX_LENGTH = 50;
export const REMARK_MAX_LENGTH = 50;