import React, { ChangeEvent, forwardRef, useImperativeHandle, useRef, useState } from 'react';

import { BlockItem, BlockRefType } from '@customer-web-app/shared/base/block';

import styles from './site-currency-section.module.scss';
import { Currency, Site } from './site-currency-service';
import { ValidationRefType, ValidationResult } from '@customer-web-app/share-models';
import SiteCurrencySectionValidator from './site-currency-section-validator';
import { useTranslation } from 'react-i18next';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';
import classNames from 'classnames';
import CustomDropdown from './custom-dropdown';

/* eslint-disable-next-line */
export interface SiteCurrencySectionProps {
  site: string;
  sites: Site[];
  currency: string;
  currencies: Currency[];
  isInternalUser: boolean;
  handleChangeSite: (siteLabel: string, site: string) => void;
  handleChangeCurrency: (value: string) => void;
  handleChangeInternalUser: (value: boolean) => void;
  extraRef?: React.RefObject<ValidationRefType>;
}

export const SiteCurrencySection = forwardRef<ValidationRefType, SiteCurrencySectionProps>(function SiteCurrencySection(
  props: SiteCurrencySectionProps,
  ref
) {
  const { sites, currencies, extraRef } = props;
  const [isInternalUser, setIsInternalUser] = useState(props.isInternalUser);
  const { t } = useTranslation();
  const blockRef = useRef<BlockRefType>(null);
  const [validationResults, setValidateResults] = useState<{ [key: string]: ValidationResult | boolean }>({});
  const [locationTooltip, setLocationTooltip] = useState<string>('');

  useImperativeHandle(ref, () => {
    const validate = () => {
      const validator = new SiteCurrencySectionValidator(props, t);
      let validateResult = true;
      const validateItemResult = validator.validate();
      const isErrorField = Object.values(validateItemResult).find((item) => item !== true) as ValidationResult;

      if (isErrorField) {
        setValidateResults(validateItemResult);
        setLocationTooltip(isErrorField.fieldName);
        validateResult = false;

        blockRef.current?.expand();
        extraRef?.current?.expandBlock && extraRef.current.expandBlock();
      }
      return validateResult;
    };

    return {
      validate,
    };
  });
  const getErrorByField = (fieldName: string) => validationResults[fieldName] && validationResults[fieldName] !== true;

  const renderErrorTooltip = () => {
    if (!locationTooltip) {
      return null;
    }
    const tooltipInfo = validationResults[locationTooltip] as ValidationResult;

    if (!tooltipInfo) {
      return null;
    }

    const { fieldName, isSuccessful, message, msgParams } = tooltipInfo;
    return (
      <WrapperTooltip
        type="error"
        anchorSelect={`#${fieldName}`}
        isOpen={!isSuccessful}
        content={t(message, msgParams)}
      />
    );
  };

  const validateSite = (site: string) => {
    const result = new SiteCurrencySectionValidator({ site }, t).validateSite();
    setValidateResults({ ...validationResults, site: result });
  };

  const validateCurrency = (currency: string) => {
    const result = new SiteCurrencySectionValidator({ currency }, t).validateCurrency();
    setValidateResults({ ...validationResults, currency: result });
  };

  const onSiteChange = (event: ChangeEvent<HTMLSelectElement>) => {
    event.preventDefault();
    const { selectedIndex, options } = event.currentTarget;
    const selectedOption = options[selectedIndex];
    const siteLabel = selectedOption.closest('optgroup')?.label;
    setLocationTooltip('site');
    if (!siteLabel || siteLabel === 'Select') {
      props.handleChangeSite('', '');
      validateSite('');
    } else {
      props.handleChangeSite(siteLabel, event.target.value);
      validateSite(event.target.value);
    }
  };

  const onCurrencyChange = (option: { value: string; text: string }) => {
    const newSelectedCurrency = option.value;
    setLocationTooltip('currency');
    if (!newSelectedCurrency || newSelectedCurrency === 'Select') {
      props.handleChangeCurrency('');
      validateCurrency('');
    } else {
      props.handleChangeCurrency(newSelectedCurrency);
      validateCurrency(newSelectedCurrency);
    }
  };
  const onInternalUserChange = () => {
    props.handleChangeInternalUser(!isInternalUser);
    setIsInternalUser(!isInternalUser);
  };

  return (
    <div className={styles['container']}>
      <BlockItem label={'Site'}>
        <div
          className={classNames({
            errorSelect: getErrorByField('site'),
          })}
        >
          <select
            className={classNames(styles['select-group'])}
            name="listSite"
            id="site"
            onChange={onSiteChange}
            value={props.site}
            onBlur={() => setLocationTooltip('')}
          >
            <option>Select</option>
            {sites?.map(({ id, name, childSites }) => (
              <optgroup label={name} key={id}>
                {childSites?.map((child) => (
                  <option key={child.id} value={child.name}>
                    {child.name}
                  </option>
                ))}
              </optgroup>
            ))}
          </select>
        </div>
      </BlockItem>
      <BlockItem label={'Currency'}>
        <div
          className={classNames({
            errorSelect: getErrorByField('currency'),
          })}
        >
          <CustomDropdown id="currency" options={currencies} onChange={onCurrencyChange} />
        </div>
      </BlockItem>

      <BlockItem label={'\u00A0'}>
        <label className={styles.checkbox} htmlFor="internal-user-checkbox">
          <input id="internal-user-checkbox" type="checkbox" checked={isInternalUser} onChange={onInternalUserChange} />
          <span className="unselectable">Internal User</span>
        </label>
      </BlockItem>
      {renderErrorTooltip()}
    </div>
  );
});

export default SiteCurrencySection;
