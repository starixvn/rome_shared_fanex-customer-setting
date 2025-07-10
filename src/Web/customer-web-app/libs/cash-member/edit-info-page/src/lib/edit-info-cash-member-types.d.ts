declare global {
    const updateInfoCustomerPageConfig: {
        applicationPath: string;
        serverRootUrl: string;
        isCashMember: boolean;
        username: string;
        custId: number;
        cdnUri: string;
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

export {
    updateInfoCustomerPageConfig
};