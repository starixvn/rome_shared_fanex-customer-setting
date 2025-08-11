declare global {
    const updateInfoCustomerPageConfig: {
        applicationPath: string;
        serverRootUrl: string;
        isCashMember: boolean;
        username: string;
        custId: number;
        cdnUri: string;
        agentId: number;
        systemId: number;
    }
}
export interface MemberBankAccountRecord {   
    agentBankAccountID: number;
    agentBankAccountNumber: string;
    agentBankAccountStatus: string;
    agentBankCode: string;
    agentBankID: number;
    memberBankAccountID: number;
    memberBankAccountNumber: string;
    memberBankCode: string;
    memberBankID: number;
}

export interface BaseResponse {
    errorCode: string;
    code: number;
    errorMessage: string;
    message: string;
    isSuccess: boolean;
    isSuccessful: boolean;
}

export type UpdateCustomerInfoResponse = BaseResponse;
export type UpdateMemberBankInfoResponse = BaseResponse;
export type AddMemberBankInfoResponse = BaseResponse;
export type RemoveMemberBankInfoResponse = BaseResponse;

export interface BaseCustomerInfo {
    firstName: string;
    lastName: string;
    mobileCode: string;
    mobileNumber: string;
    phone: string;
    email: string;
    fax: string;
}