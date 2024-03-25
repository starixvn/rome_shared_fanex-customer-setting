import { axiosInstance } from '@customer-web-app/shared-api-helper';

export interface Site {
  id: string;
  name: string;
  childSites: Site[];
  isParent: boolean;
}

export interface Currency {
  value: string;
  text: string;
}

export interface SiteCurrencyResponse {
  sites: Site[];
  currencies: Currency[];
}

export async function getSitesAndCurrencies() {
  const response = await axiosInstance.get<SiteCurrencyResponse>('Customer/GetSitesAndCurrencies');
  return response.data;
}
