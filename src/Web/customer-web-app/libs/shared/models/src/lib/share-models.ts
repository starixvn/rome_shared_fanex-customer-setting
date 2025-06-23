export interface UsernameSuggestion {
  UserName: string;
  FirstName: string;
  LastName: string;
  UserName2: string;
}

export interface CashServiceStatus {
  isOfferedCashService: boolean;
  isCanUpdateCashServiceStatus: boolean;
  isCashServiceEnabled: boolean;
}

export class Result {
  public code: number;
  public message: string;

  constructor(code = 0, message = '') {
    this.code = code;
    this.message = message;
  }

  public get isSuccessful(): boolean {
    return this.code === 0;
  }

  public static CreateSuccessfulResult() {
    return new Result();
  }

  public static get UnAuthorizedResult() {
    return new Result(-2, 'YouAreNotAuthorized');
  }
}

export class ValidationResult extends Result {
  public fieldName: string;
  public msgParams?: { [key: string]: string };

  constructor(fieldName: string, message: string, msgParams?: { [key: string]: string }) {
    super(-8000, message);

    this.fieldName = fieldName;
    this.msgParams = msgParams;
  }
}

export class SubMessagesResult extends Result {
  subMessages?: SubMessage[];
}

export class CreateCustomerResult extends SubMessagesResult {
  newCustIds?: number[];
  newUserNames?: string[];
  availableAlphaDomain?: string[];
}

export interface SubMessage {
  isLive: boolean;
  message: string;
}

export class SubResult extends Result {
  public groupTypeId?: number;
  public groupTypeName?: string;
  public subMessages?: SubMessage[];
}

export class UpdateCustomerResult extends Result {
  public subResults?: SubResult[];
  public custName?: string;
}

export interface ValidateRefResult {
  isValid: boolean;
  groupTypeId?: number;
}

export interface ValidationRefType {
  validate: (groupTypeId?: number) => boolean | Promise<boolean> | ValidateRefResult;
  scrollToBlock?: (groupTypeId?: number) => void;
  expandBlock?: () => void;
}

export interface ApplicationConfig {
  applicationPath: string;
  systemId: number;
}

export interface Params {
  isCopyBetSetting: boolean;
  uplineId: number;
  uplineRoleId: number;
  suggestUsername: string;
  site: string;
  siteLabel: string;
  custIds: string;
  usernames: string;
  roleId: number;
  custId: number;
  noPermissionEditCredit: boolean;
  productGroupId: number;
  groupName: string;
  agentId: number;
  ServiceType: number;
}

export interface Note {
  [key: string]: Note[] | string[] | Note;
}

export interface TabOptions {
  name: string;
  id: string;
  url: string;
  isActive: boolean;
  contents: string[];
  index: number;
}
export interface InformationPageParams {
  tabs: TabOptions[];
  is789y?: boolean;
  roleId?: number;

  site?: string;
  siteLabel?: string;
  isCashMember?: boolean;
}

declare global {
  const appConfig: ApplicationConfig;
  const params: Params;
  const informationPageParams: InformationPageParams;
  const cdnUri: string;

  interface Window {
    redirectToSettingPage: (response: any) => string;
  }
}
