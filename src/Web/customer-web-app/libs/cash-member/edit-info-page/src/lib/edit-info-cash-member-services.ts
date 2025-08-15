import axios from 'axios';
import { AxiosError } from 'axios';
import { AlphaSystemIds } from '@customer-web-app/shared-constants';

export const axiosInstance = axios.create({
  headers: {
    __RequestVerificationToken: (document.getElementsByName('__RequestVerificationToken')[0] as HTMLInputElement).value,
    RequestVerificationToken: (document.getElementsByName('__RequestVerificationToken')[0] as HTMLInputElement).value,
    'X-Requested-With': 'XMLHttpRequest',
  },
  baseURL: updateInfoCustomerPageConfig.applicationPath,
});

const GetCustomerInfo = async <T>(params?: Record<string, unknown>): Promise<T> => {
  const { data } = await axiosInstance.get<T>(`Customer/GetCustomerInfo`, {
    params,
  });
  return data;
};

const UpdateCustomerInfo = async <T>(params?: Record<string, unknown>): Promise<T> => {
  try {
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
  } catch (error) {
    const axiosError = error as AxiosError<{ message: string }>;
    if (axiosError.response?.data.message === 'NoPermission') {
      return axiosError.response.data as T;
    }
    throw error;
  }
};

const GetCustomerBankInfo = async <T>(params?: Record<string, unknown>): Promise<T> => {
  const { data } = await axiosInstance.get<T>(`Customer/GetCustomerBankInfo`, {
    params,
  });
  return data;
};

const GetBanks = async <T>(params?: Record<string, unknown>): Promise<T> => {
  const { data } = await axiosInstance.get<T>(`Customer/GetBanks`, {
    params,
  });
  return data;
};

const AddMemberBankInfo = async <T>(params?: Record<string, unknown>): Promise<T> => {
  try {
    const { data } = await axiosInstance.post<T>(`Customer/AddMemberBankInfo`, params, {
      headers: {
        'Content-Type': 'application/json',
      },
    });
    return data;
  } catch (error) {
    const axiosError = error as AxiosError<{ message: string }>;
    if (axiosError.response?.data.message === 'NoPermission') {
      return axiosError.response.data as T;
    }
    throw error;
  }
};

const UpdateMemberBankInfo = async <T>(params?: Record<string, unknown>): Promise<T> => {
  try {
    const { data } = await axiosInstance.post<T>(`Customer/UpdateMemberBankInfo`, params, {
      headers: {
        'Content-Type': 'application/json',
      },
    });
    return data;
  } catch (error) {
    const axiosError = error as AxiosError<{ message: string }>;
    if (axiosError.response?.data.message === 'NoPermission') {
      return axiosError.response.data as T;
    }
    throw error;
  }
};

const RemoveMemberBankInfo = async <T>(params?: Record<string, unknown>): Promise<T> => {
  try {
    const { data } = await axiosInstance.post<T>(`Customer/RemoveMemberBankInfo`, params, {
      headers: {
        'Content-Type': 'application/json',
      },
    });
    return data;
  } catch (error) {
    const axiosError = error as AxiosError<{ message: string }>;
    if (axiosError.response?.data.message === 'NoPermission') {
      return axiosError.response.data as T;
    }
    throw error;
  }
};

const GetCashServiceStatus = async <T>(params?: Record<string, unknown>): Promise<T> => {
  const { data } = await axiosInstance.get<T>(`Customer/GetCashServiceStatus`, {
    params,
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
  GetCashServiceStatus,
};
