import { useEffect, useReducer, useRef, useState } from 'react';

import { ValidationRefType } from '@customer-web-app/share-models';
import { BlockUI } from '@customer-web-app/shared-block-ui';
import { Currencies, SystemIds } from '@customer-web-app/shared-constants';
import {
    GeneralInfo, GeneralInfoActionTypes, generalInformationReducer, initialState
} from '@customer-web-app/shared-general-information-state';
import { DirectMemberGeneralInformation } from '@customer-web-app/shared-general-information-ui';
import { SettingService } from '@customer-web-app/shared-group-settings-services';
import { Layout, useTheme } from '@customer-web-app/shared/base/layout';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import styles from './create-direct-member-page.module.scss';

/* eslint-disable-next-line */
export interface CreateDirectMemberPageProps {}

export function CreateDirectMemberPage(props: CreateDirectMemberPageProps) {
  const [isLoading, setIsLoading] = useState(true);
  const theme = useTheme();

  const [generalInformationState, generalInfoDispatch] = useReducer(generalInformationReducer, initialState);
  const { generalInfo } = generalInformationState;
  const generalInfoRef = useRef<ValidationRefType>(null);

  const handleGeneralInfoChange = (payload: Partial<GeneralInfo>) => {
    generalInfoDispatch({ type: GeneralInfoActionTypes.Update, payload });
  };

  useEffect(() => {
    const fetchData = async () => {
      const { generalInfo, productGroups } = await SettingService.GetCustomerSetting(
        params.uplineId,
        params.uplineRoleId,
        0,
        false,
        params.siteLabel
      );
      if (appConfig.systemId !== SystemIds.NAP) {
        generalInfo.newUserName = params.suggestUsername;
      }

      generalInfoDispatch({
        type: GeneralInfoActionTypes.Update,
        payload: {
          ...generalInfo,
          userName:
            appConfig.systemId !== SystemIds.NAP
              ? (generalInfo.uplineUserName + generalInfo.newUserName).toUpperCase()
              : '',
          maxCredit: 0,
          memberMaxCredit: 0,
          uplineMaxCredit: generalInfo.maxCredit,
          roleId: 1,
          uplineCustId: params.uplineId,
          uplineRoleId: params.uplineRoleId,
          group: generalInfo.currency.toString() === Currencies.Won.toString() ? 'dea' : 'a',
        },
      });
      setIsLoading(false);
    };

    fetchData();
  }, []);

  return (
    <>
      {isLoading && <BlockUI />}
      <WrapperTooltip id="global-tooltip" className={styles['global-tooltip']} />
      <Layout theme={theme}>
        <DirectMemberGeneralInformation
          ref={generalInfoRef}
          generalInfo={generalInfo}
          onGeneralInfoChange={handleGeneralInfoChange}
        />
      </Layout>
    </>
  );
}

export default CreateDirectMemberPage;
