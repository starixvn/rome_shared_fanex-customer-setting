import { Tolgee, FormatSimple, DevBackend  } from '@tolgee/react';

const searchParams = new URLSearchParams(document.location.search);

const isUseStatic = import.meta.env.USE_TOLGEE_STATIC === 'true';

export const tolgeeInit = Tolgee()
  .use(FormatSimple())
  .use(DevBackend())
  .init({
    availableLanguages: ['en-US', 'th-TH', 'vi-VN', 'zh-CN', 'zh-TW', 'ja-JP', 'ko-KR'],
    defaultLanguage: 'en-US',
    defaultNs: 'translation',
    language: searchParams.get('lng') ?? 'en-US',
    ns: ['translation', 'pt-template', 'product-setting-info'],
    apiKey: 'tgpak_gnpwq4jvojvdsn3gmz2da5tggvzwknttozzda23gn4yda',
    apiUrl: isUseStatic ? '' : 'https://tolgee-alpha.nexdev.net',
    staticData: {
      'en-US:translation': () => import('../messages/translation/en-US.json'),
      'en-US:pt-template': () => import('../messages/pt-template/en-US.json'),
      'en-US:product-setting-info': () => import('../messages/product-setting-info/en-US.json'),
      'th-TH:translation': () => import('../messages/translation/th-TH.json'),
      'th-TH:pt-template': () => import('../messages/pt-template/th-TH.json'),
      'th-TH:product-setting-info': () => import('../messages/product-setting-info/th-TH.json'),
      'vi-VN:translation': () => import('../messages/translation/vi-VN.json'),
      'vi-VN:pt-template': () => import('../messages/pt-template/vi-VN.json'),
      'vi-VN:product-setting-info': () => import('../messages/product-setting-info/vi-VN.json'),
      'zh-CN:translation': () => import('../messages/translation/zh-CN.json'),
      'zh-CN:pt-template': () => import('../messages/pt-template/zh-CN.json'),
      'zh-CN:product-setting-info': () => import('../messages/product-setting-info/zh-CN.json'),
      'zh-TW:translation': () => import('../messages/translation/zh-TW.json'),
      'zh-TW:pt-template': () => import('../messages/pt-template/zh-TW.json'),
      'zh-TW:product-setting-info': () => import('../messages/product-setting-info/zh-TW.json'),
      'ja-JP:translation': () => import('../messages/translation/ja-JP.json'),
      'ja-JP:pt-template': () => import('../messages/pt-template/ja-JP.json'),
      'ja-JP:product-setting-info': () => import('../messages/product-setting-info/ja-JP.json'),
      'ko-KR:translation': () => import('../messages/translation/ko-KR.json'),
      'ko-KR:pt-template': () => import('../messages/pt-template/ko-KR.json'),
      'ko-KR:product-setting-info': () => import('../messages/product-setting-info/ko-KR.json'),
    },
  });