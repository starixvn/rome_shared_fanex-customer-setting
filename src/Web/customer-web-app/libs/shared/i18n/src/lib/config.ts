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

import adsCash from './en-US/ads-cash.json';
import adsCashKO from './ko-KR/ads-cash.json';
import adsCashVi from './vi-VN/ads-cash.json';
import adsCashThai from './th-TH/ads-cash.json';
import adsCashCN from './zh-CN/ads-cash.json';
import adsCashTW from './zh-TW/ads-cash.json';

export const defaultNS = 'translation';

export const resources = {
  'en-US': { translation: { ...enUS, ...productSettingInfoUs, ...potentialWinLimitSettingInfoUs, ...adsCash } },
  'th-TH': { translation: { ...thTh, ...productSettingInfoThai, ...potentialWinLimitSettingInfoThai, ...adsCashThai } },
  'vi-VN': { translation: { ...viVN, ...productSettingInfoVi, ...potentialWinLimitSettingInfoVi, ...adsCashVi } },
  'zh-CN': { translation: { ...zhCN, ...productSettingInfoCN, ...potentialWinLimitSettingInfoCN, ...adsCashCN } },
  'zh-TW': { translation: { ...zhTW, ...productSettingInfoTW, ...potentialWinLimitSettingInfoTW, ...adsCashTW } },
  'ko-KR': { translation: { ...koKR, ...productSettingInfoKO, ...potentialWinLimitSettingInfoKO, ...adsCashKO } },
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
