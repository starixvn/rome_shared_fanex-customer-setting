import { Tolgee, FormatSimple, DevBackend  } from '@tolgee/react';

const searchParams = new URLSearchParams(document.location.search);

function mapLanguageCode(code: string | null) {
  switch (code) {
    case 'zh-CN':
      return 'zh-Hans';
    case 'zh-TW':
      return 'zh-Hant';
    default:
      return code ?? 'en-US';
  }
}

const isUseStatic = process.env.USE_TOLGEE_STATIC === 'true';

export const tolgeeInit = Tolgee()
  .use(FormatSimple())
  .use(DevBackend())
  .init({
    availableLanguages: ['en-US', 'th-TH', 'vi-VN', 'ja-JP', 'ko-KR', 'zh-Hans', 'zh-Hant'],
    defaultLanguage: 'en-US',
    defaultNs: 'translation',
    language: mapLanguageCode(searchParams.get('lng')),
    ns: ['translation'],
    apiKey: 'tgpak_g5pwomtqg5tgkobsgnwgw5lbnq3xmytcnnsgw2ttguygg',
    apiUrl: isUseStatic ? '' : 'https://tolgee-alpha.nexdev.net',
    staticData: {
      'en-US:translation': () => import('../data/translation/en-US.json'),
      'th-TH:translation': () => import('../data/translation/th-TH.json'),
      'ja-JP:translation': () => import('../data/translation/ja-JP.json'),
      'ko-KR:translation': () => import('../data/translation/ko-KR.json'),
      'vi-VN:translation': () => import('../data/translation/vi-VN.json'),
      'zh-Hant:translation': () => import('../data/translation/zh-Hant.json'),
      'zh-Hans:translation': () => import('../data/translation/zh-Hans.json'),
    },
  });