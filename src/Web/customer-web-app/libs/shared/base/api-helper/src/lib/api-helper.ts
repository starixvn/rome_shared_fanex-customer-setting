import axios from 'axios';

export const axiosInstance = axios.create({
  baseURL: appConfig.applicationPath,
});

export const getHeadersForPostRequest = () => ({
  __RequestVerificationToken: (document.getElementsByName('__RequestVerificationToken')[0] as HTMLInputElement).value,
  'X-Requested-With': 'XMLHttpRequest',
});
