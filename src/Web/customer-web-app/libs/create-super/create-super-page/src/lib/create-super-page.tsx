import { useEffect, useReducer, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import {
  CreateSuperNotes,
  getHeadlineBelongSite,
  getNotAllowedSuperPrefixes,
  HeadlineBelongSite,
  SuperPrefixes,
} from '@customer-web-app/create-super-notes';
import { handleChangeGroupSettingValue, handleChangeItemValue } from '@customer-web-app/edit-customer-page';
import { CreateCustomerResult, Result, ValidateRefResult, ValidationRefType } from '@customer-web-app/share-models';
import { BlockUI } from '@customer-web-app/shared-block-ui';
import { ProductGroupIds } from '@customer-web-app/shared-constants';
import { ErrorBlock } from '@customer-web-app/shared-error-block';
import {
  GeneralInfo,
  GeneralInfoActionTypes,
  generalInformationReducer,
  initialState,
} from '@customer-web-app/shared-general-information-state';
import { GeneralInformation } from '@customer-web-app/shared-general-information-ui';
import { GroupSettingsContextProvider } from '@customer-web-app/shared-group-settings-context';
import { SettingService } from '@customer-web-app/shared-group-settings-services';
import { GroupSettingItem, ProductGroup } from '@customer-web-app/shared-group-settings-types';
import { GroupSetting, scrollToTop } from '@customer-web-app/shared-group-settings-ui';
import { encrypt } from '@customer-web-app/shared-helpers';
import { Layout, useTheme } from '@customer-web-app/shared/base/layout';
import { Currency, getSitesAndCurrencies, Site, SiteCurrencySection } from '@customer-web-app/site-currency-section';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import styles from './create-super-page.module.scss';

export function CreateSuperPage() {
  const { t } = useTranslation();
  const theme = useTheme();
  const [generalInformationState, generalInfoDispatch] = useReducer(generalInformationReducer, initialState);
  const { generalInfo } = generalInformationState;
  const { site, currency, siteLabel } = generalInfo;

  const [isLoading, setIsLoading] = useState(true);
  const [createCustomerResult, setCreateCustomerResult] = useState<Result>();
  const [productGroups, setProductGroups] = useState<ProductGroup[]>([]);
  const [sites, setSites] = useState<Site[]>([]);
  const [currencies, setCurrencies] = useState<Currency[]>([]);
  const [belongSites, setBelongSites] = useState<HeadlineBelongSite[]>([]);
  const [notAllowedPrefixes, setNotAllowedPrefixes] = useState<SuperPrefixes[]>([]);

  const siteCurrencyRef = useRef<ValidationRefType>(null);
  const generalInfoRef = useRef<ValidationRefType>(null);
  const groupSettingRef = useRef<ValidationRefType>(null);
  const transferRef = useRef<ValidationRefType>(null);

  useEffect(() => {
    const fetchData = async () => {
      const response = await getSitesAndCurrencies();
      setSites(response.sites);
      setCurrencies(response.currencies);
      setBelongSites(await getHeadlineBelongSite());
      setNotAllowedPrefixes(await getNotAllowedSuperPrefixes());
      setIsLoading(false);
    };

    fetchData();
  }, []);

  useEffect(() => {
    if (site && currency && siteLabel) {
      setIsLoading(true);
      SettingService.GetGlobalSetting(siteLabel, site, currency.toString()).then((data) => {
        setProductGroups(data.productGroups);
        setIsLoading(false);
      });
    } else {
      setProductGroups([]);
    }
  }, [site, currency, siteLabel]);

  const handleGeneralInfoChange = (payload: Partial<GeneralInfo>) => {
    generalInfoDispatch({ type: GeneralInfoActionTypes.Update, payload });
  };

  const handleSettingItemChange = (
    productGroupId: number,
    groupTypeId: number,
    sportTypeId: number,
    betTypeId: number,
    data: Partial<GroupSettingItem<unknown>>
  ) => {
    const newProductGroups = handleChangeItemValue(
      productGroups,
      productGroupId,
      groupTypeId,
      sportTypeId,
      betTypeId,
      data
    );
    setProductGroups(newProductGroups);
  };

  const handleUpdateSettingItems = (
    productGroupId: number,
    groupTypeId: number,
    groupItems: GroupSettingItem<unknown>[]
  ) => {
    const newProductGroups = handleChangeGroupSettingValue(productGroups, productGroupId, groupTypeId, groupItems);
    setProductGroups(newProductGroups);
  };

  const scrollToInvalidGroup = (
    isValidSiteCurrency: boolean,
    isValidGeneralInfo: boolean,
    validateGroupSettingResult: ValidateRefResult,
    isValidTransfer: boolean
  ) => {
    if (!isValidSiteCurrency) {
      scrollToTop();
      return;
    }

    if (!isValidGeneralInfo) {
      generalInfoRef?.current?.scrollToBlock && generalInfoRef.current.scrollToBlock();
      return;
    }

    if (!validateGroupSettingResult.isValid) {
      groupSettingRef?.current?.scrollToBlock &&
        groupSettingRef.current.scrollToBlock(validateGroupSettingResult.groupTypeId);
    }

    !isValidTransfer && transferRef?.current?.scrollToBlock && transferRef?.current?.scrollToBlock();
  };

  const validateGroupTypes = async () => groupSettingRef.current && (await groupSettingRef.current.validate());

  const handleCreateSuper = async () => {
    setIsLoading(true);
    const isValidSiteCurrency = siteCurrencyRef.current ? (siteCurrencyRef.current.validate() as boolean) : true;
    const isValidGeneralInfo = generalInfoRef.current ? ((await generalInfoRef.current.validate()) as boolean) : true;
    const validateGroupSettingResult = (await validateGroupTypes()) as ValidateRefResult;
    const isValidTransfer = generalInfo.displayTransferCondition ? (transferRef.current?.validate() as boolean) : true;

    if (isValidSiteCurrency && isValidGeneralInfo && validateGroupSettingResult.isValid && isValidTransfer) {
      const secretKey = (document.getElementById('SecretKey') as HTMLInputElement).value;
      const encryptedPassword = encrypt(generalInfo.password, secretKey);
      const result = await SettingService.CreateCustomer(
        { ...generalInfo, password: encryptedPassword },
        '',
        productGroups
      );
      if (result.isSuccessful) {
        const createResult = result as CreateCustomerResult;
        setIsLoading(true);
        setTimeout(() => {
          const redirectUrl = `${appConfig.applicationPath}Customer/V2/UpdatePT.aspx?custid=${createResult?.newCustId}&insert=true&roleid=4&uplineid=0&exchangeId=${generalInfo.currency}`;
          window.location.replace(redirectUrl);
        }, 3000);
        return;
      } else {
        setCreateCustomerResult(result);
        scrollToTop();
      }
    }
    scrollToInvalidGroup(isValidSiteCurrency, isValidGeneralInfo, validateGroupSettingResult, isValidTransfer);
    setIsLoading(false);
  };

  const renderCreateButton = () => {
    return (
      <div className={styles.btn}>
        <button type="button" onClick={handleCreateSuper}>
          {t('CreateAccount')}
        </button>
      </div>
    );
  };

  return (
    <>
      {isLoading && <BlockUI />}
      <WrapperTooltip id="global-tooltip" className={styles['global-tooltip']} />
      <Layout theme={theme}>
        {renderCreateButton()}
        {createCustomerResult && <ErrorBlock error={createCustomerResult} />}
        <CreateSuperNotes belongSites={belongSites} notAllowedPrefixes={notAllowedPrefixes} />
        <GeneralInformation
          ref={generalInfoRef}
          generalInfo={generalInfo}
          onGeneralInfoChange={handleGeneralInfoChange}
        >
          <SiteCurrencySection
            sites={sites}
            site={generalInfo.site}
            currencies={currencies}
            currency={generalInfo.currency.toString()}
            isInternalUser={generalInfo.isInternal}
            handleChangeSite={(siteLabel, site) => handleGeneralInfoChange({ siteLabel, site })}
            handleChangeCurrency={(value) => handleGeneralInfoChange({ currency: value })}
            handleChangeInternalUser={(value) => handleGeneralInfoChange({ isInternal: value })}
            ref={siteCurrencyRef}
            extraRef={generalInfoRef}
          />
        </GeneralInformation>
        <GroupSettingsContextProvider
          value={{
            generalInfo,
            productGroups,
            handleSettingItemChange,
            handleUpdateSettingItems,
            mode: 'createSuper',
            productGroupId: ProductGroupIds.SportsBook,
          }}
        >
          <GroupSetting ref={groupSettingRef} />
        </GroupSettingsContextProvider>
        {renderCreateButton()}
      </Layout>
    </>
  );
}

export default CreateSuperPage;
