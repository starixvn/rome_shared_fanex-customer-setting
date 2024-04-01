/* eslint-disable import/extensions */
import i18next from 'i18next';
import LanguageDetector from 'i18next-browser-languagedetector';
import { initReactI18next } from 'react-i18next';

import productSettingInfoUs from './en-US/product-setting-info.json';
import enUS from './en-US/translation.json';
import productSettingInfoThai from './th-TH/product-setting-info.json';
import thTh from './th-TH/translation.json';
import productSettingInfoVi from './vi-VN/product-setting-info.json';
import viVN from './vi-VN/translation.json';
import productSettingInfoCN from './zh-CN/product-setting-info.json';
import zhCN from './zh-CN/translation.json';
import productSettingInfoTW from './zh-TW/product-setting-info.json';
import zhTW from './zh-TW/translation.json';

export const defaultNS = 'translation';

export const resources = {
  'en-US': { translation: { ...enUS, ...productSettingInfoUs } },
  'th-TH': { translation: { ...thTh, ...productSettingInfoThai } },
  'vi-VN': { translation: { ...viVN, ...productSettingInfoVi } },
  'zh-CN': { translation: { ...zhCN, ...productSettingInfoCN } },
  'zh-TW': { translation: { ...zhTW, ...productSettingInfoTW } },
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
