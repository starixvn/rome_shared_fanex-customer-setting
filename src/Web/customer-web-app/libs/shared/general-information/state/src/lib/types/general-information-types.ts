export interface GeneralInfo {
  site: string;
  siteLabel: string;
  isSpecialSite: boolean;
  currency: string;
  isInternal: boolean;
  userName: string;
  password: string;
  firstName?: string;
  lastName?: string;
  phone?: string;
  mobilePhone?: string;
  fax?: string;
  maxCredit: number;
  maxCreditMin?: number;
  maxCreditMax?: number;
  memberMaxCredit?: number;
  memberMaxCreditMin?: number;
  uplineMaxCredit?: number;
  roleId: number;
  custId: number;
  uplineRoleId: number;
  uplineCustId: number;
  uplineUserName: string;
  newUserName: string;
  isAutomateTransfer?: boolean;
  transferOption?: string;
  displayTransferCondition: boolean;
  disableAutoMPT: boolean;
  group: string;
  commissions: {
    discount: number;
    name: string;
  }[];
}
export type UserInformation = Pick<GeneralInfo, 'firstName' | 'lastName' | 'phone' | 'mobilePhone' | 'fax'>;
