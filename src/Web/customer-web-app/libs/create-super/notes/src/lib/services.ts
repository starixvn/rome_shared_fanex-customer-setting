import { axiosInstance } from '@customer-web-app/shared-api-helper';

import { HeadlineBelongSite, SuperPrefixes } from './types';

export async function getHeadlineBelongSite() {
  const response = await axiosInstance.get<HeadlineBelongSite[]>('Customer/GetHeadlineBelongSite');
  return response.data;
}

export async function getNotAllowedSuperPrefixes() {
  const response = await axiosInstance.get<SuperPrefixes[]>('Customer/GetNotAllowedSuperPrefixes');
  return response.data;
}
