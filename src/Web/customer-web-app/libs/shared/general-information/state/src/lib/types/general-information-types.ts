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
  remark?: string;
  maxCredit: number;
  maxCreditMin?: number;
  maxCreditMax?: number;
  memberMaxCredit?: number;
  memberMaxCreditMin?: number;
  uplineMaxCredit?: number;
  uplineMemberMaxCredit?: number;
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
  commissions: {
    discount: number;
    name: string;
  }[];
  bank?: {
    bankId?: number;
    accountNumber?: string;
  }
}
export type UserInformation = Pick<GeneralInfo, 'firstName' | 'lastName' | 'phone' | 'mobilePhone' | 'fax' | 'remark'>;
