import axios from 'axios';

import { AlphaSystemIds } from '@customer-web-app/shared-constants';

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
    params,
  });
  return data;
};
const UpdateCustomerInfo = async <T>(params?: Record<string, any>): Promise<T> => {
  let url = 'Customer/EditInfoPost'; // default
  if (AlphaSystemIds.includes(updateInfoCustomerPageConfig.systemId)) {
    url = `Customer/UpdateInfoPost`;
  }

  const { data } = await axiosInstance.post<T>(url, params, {
    headers: {
      'Content-Type': 'application/json',
    },
  });
  return data;
};

const GetCustomerBankInfo = async <T>(params?: Record<string, any>): Promise<T> => {
  const { data } = await axiosInstance.get<T>(`Customer/GetCustomerBankInfo`, {
    params,
  });
  return data;
};

const GetBanks = async <T>(params?: Record<string, any>): Promise<T> => {
  const { data } = await axiosInstance.get<T>(`Customer/GetBanks`, {
    params,
  });
  return data;
};

const AddMemberBankInfo = async <T>(params?: Record<string, any>): Promise<T> => {
  const { data } = await axiosInstance.post<T>(`Customer/AddMemberBankInfo`, params, {
    headers: {
      'Content-Type': 'application/json',
    },
  });
  return data;
};

const UpdateMemberBankInfo = async <T>(params?: Record<string, any>): Promise<T> => {
  const { data } = await axiosInstance.post<T>(`Customer/UpdateMemberBankInfo`, params, {
    headers: {
      'Content-Type': 'application/json',
    },
  });
  return data;
};

const RemoveMemberBankInfo = async <T>(params?: Record<string, any>): Promise<T> => {
  const { data } = await axiosInstance.post<T>(`Customer/RemoveMemberBankInfo`, params, {
    headers: {
      'Content-Type': 'application/json',
    },
  });
  return data;
};

export {
  GetCustomerInfo,
  UpdateCustomerInfo,
  GetCustomerBankInfo,
  GetBanks,
  AddMemberBankInfo,
  UpdateMemberBankInfo,
  RemoveMemberBankInfo,
};
