import axios from 'axios';
export const axiosInstance = axios.create({
    headers: {
        __RequestVerificationToken: (document.getElementsByName('__RequestVerificationToken')[0] as HTMLInputElement).value,
        RequestVerificationToken: (document.getElementsByName('__RequestVerificationToken')[0] as HTMLInputElement).value,
        'X-Requested-With': 'XMLHttpRequest',
    },
    baseURL: updateInfoCustomerPageConfig.applicationPath,
});

const GetCustomerInfo = async <T>(params?: Record<string, any>): Promise<T> => {
    const { data } = await axiosInstance.get<T>(`Customer/GetCustomerInfo`, {
        params
    });
    return data;
}
const UpdateCustomerInfo = async <T>(params?: Record<string, any>): Promise<T> => {
    const { data } = await axiosInstance.post<T>(`Customer/EditInfoPost`, params, {
        headers: {
            //    ...getHeadersForPostRequest(),
            'Content-Type': 'application/json'
        }
    });
    console.log('data', data);
    return data;
}

const GetCustomerBankInfo = async <T>(params?: Record<string, any>): Promise<T> => {
    const { data } = await axiosInstance.get<T>(`${updateInfoCustomerPageConfig.serverRootUrl}Customer/GetCustomerBankInfo`, {
        params
    });
    return data;
}

const GetBanks = async <T>(params?: Record<string, any>): Promise<T> => {
    const { data } = await axiosInstance.get<T>(`${updateInfoCustomerPageConfig.serverRootUrl}Customer/GetBanks`, {
        params
    });
    return data;
}

const AddMemberBankInfo = async <T>(params?: Record<string, any>): Promise<T> => {
    const { data } = await axiosInstance.post<T>(`${updateInfoCustomerPageConfig.serverRootUrl}Customer/AddMemberBankInfo`, params, {
        headers: {
            //getHeadersForPostRequest(),
            'Content-Type': 'application/json'
        }
    });
    return data;
}

const UpdateMemberBankInfo = async <T>(params?: Record<string, any>): Promise<T> => {
    const { data } = await axiosInstance.post<T>(`${updateInfoCustomerPageConfig.serverRootUrl}Customer/UpdateMemberBankInfo`, params, {
        headers: {
            //getHeadersForPostRequest(),
            'Content-Type': 'application/json'
        }
    });
    return data;
}

const RemoveMemberBankInfo = async <T>(params?: Record<string, any>): Promise<T> => {
    const { data } = await axiosInstance.post<T>(`${updateInfoCustomerPageConfig.serverRootUrl}Customer/RemoveMemberBankInfo`, params, {
        headers: {
            //getHeadersForPostRequest(),
            'Content-Type': 'application/json'
        }
    });
    return data;
}

export {
    GetCustomerInfo,
    UpdateCustomerInfo,
    GetCustomerBankInfo,
    GetBanks,
    AddMemberBankInfo,
    UpdateMemberBankInfo,
    RemoveMemberBankInfo
};
