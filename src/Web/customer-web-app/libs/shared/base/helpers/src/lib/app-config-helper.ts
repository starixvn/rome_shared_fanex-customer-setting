import '@customer-web-app/share-models';

import { SystemIds } from '@customer-web-app/shared-constants';

export const AppConfigHelper = {
  isNapSystem() {
    return appConfig.systemId === SystemIds.NAP;
  },
  isEditBetSettingOnly() {
    return (
      appConfig.systemId === SystemIds.ExternalNAPBetSetting ||
      appConfig.systemId === SystemIds.ExternalNAPBetSettingSpecialSite
    );
  },
  IsExternalNap() {
    return appConfig.systemId === SystemIds.ExternalNAP || appConfig.systemId === SystemIds.ExternalNapSpecialSite;
  },
};
