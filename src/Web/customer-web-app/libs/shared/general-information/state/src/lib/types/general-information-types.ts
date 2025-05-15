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
  bankId?: string;
  accountNumber?: string;
  // public int BankId { get; set; }
  // public string AccountNumber { get; set; }
}
export type UserInformation = Pick<GeneralInfo, 'firstName' | 'lastName' | 'phone' | 'mobilePhone' | 'fax'>;
