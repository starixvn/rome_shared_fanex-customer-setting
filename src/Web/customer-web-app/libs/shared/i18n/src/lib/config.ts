/* eslint-disable import/extensions */
import i18next from 'i18next';
import LanguageDetector from 'i18next-browser-languagedetector';
import { initReactI18next } from 'react-i18next';

import potentialWinLimitSettingInfoUs from './en-US/potential-win-limit.json';
import productSettingInfoUs from './en-US/product-setting-info.json';
import enUS from './en-US/translation.json';
import potentialWinLimitSettingInfoThai from './th-TH/potential-win-limit.json';
import productSettingInfoThai from './th-TH/product-setting-info.json';
import thTh from './th-TH/translation.json';
import potentialWinLimitSettingInfoVi from './vi-VN/potential-win-limit.json';
import productSettingInfoVi from './vi-VN/product-setting-info.json';
import viVN from './vi-VN/translation.json';
import potentialWinLimitSettingInfoCN from './zh-CN/potential-win-limit.json';
import productSettingInfoCN from './zh-CN/product-setting-info.json';
import zhCN from './zh-CN/translation.json';
import potentialWinLimitSettingInfoTW from './zh-TW/potential-win-limit.json';
import productSettingInfoTW from './zh-TW/product-setting-info.json';
import zhTW from './zh-TW/translation.json';

import potentialWinLimitSettingInfoKO from './ko-KR/potential-win-limit.json';
import productSettingInfoKO from './ko-KR/product-setting-info.json';
import koKR from './ko-KR/translation.json';

export const defaultNS = 'translation';

export const resources = {
  'en-US': { translation: { ...enUS, ...productSettingInfoUs, ...potentialWinLimitSettingInfoUs } },
  'th-TH': { translation: { ...thTh, ...productSettingInfoThai, ...potentialWinLimitSettingInfoThai } },
  'vi-VN': { translation: { ...viVN, ...productSettingInfoVi, ...potentialWinLimitSettingInfoVi } },
  'zh-CN': { translation: { ...zhCN, ...productSettingInfoCN, ...potentialWinLimitSettingInfoCN } },
  'zh-TW': { translation: { ...zhTW, ...productSettingInfoTW, ...potentialWinLimitSettingInfoTW } },
  'ko-KR': { translation: { ...koKR, ...productSettingInfoKO, ...potentialWinLimitSettingInfoKO } },
};

i18next
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    supportedLngs: ['en-US', 'th-TH', 'vi-VN', 'zh-CN', 'zh-TW', 'ja-JP', 'ko-KR'],
    fallbackLng: 'en-US',
    resources,
    defaultNS,
  });

export const i18nInstance = i18next;
