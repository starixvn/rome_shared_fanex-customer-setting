import { useEffect, useState } from 'react';
import { Controller, FormProvider, useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';

import {
  EMAIL_MAX_LENGTH,
  EMAIL_REGEX,
  FIRST_NAME_MAX_LENGTH,
  LAST_NAME_MAX_LENGTH,
  NUMBER_REGEX,
  PHONE_NUMBER_MAX_LENGTH,
  REMARK_MAX_LENGTH,
} from '@customer-web-app/shared-constants';
import {
  ButtonFull,
  ButtonOutline,
  Label,
  TableSkeleton,
  useResultMessage,
} from '@customer-web-app/shared/base/components';

import { GetCustomerInfo, UpdateCustomerInfo } from './edit-info-cash-member-services';
import { InfoNormalMember, UpdateCustomerInfoResponse } from './edit-info-cash-member-types';
import styles from './edit-info-normal-member-alpha.module.scss';
import { PageTitle } from './page-title';

export const EditInfoNormalMemberAlpha = () => {
  const NumberCols = 4;
  const { t } = useTranslation();
  const [originalCustomerInfo, setOriginalCustomerInfo] = useState<InfoNormalMember>({
    firstName: '',
    lastName: '',
    mobilePhone: '',
    phone: '',
    email: '',
    fax: '',
    remark: '',
  });
  const [loading, setLoading] = useState(false);
  const { showResult, renderResultMessage } = useResultMessage();

  const methods = useForm<InfoNormalMember>({
    defaultValues: originalCustomerInfo,
    mode: 'onChange',
    criteriaMode: 'all',
  });

  const {
    reset,
    control,
    formState: { isDirty },
    handleSubmit,
  } = methods;

  useEffect(() => {
    const fetchCustomerInfo = async () => {
      const data = await GetCustomerInfo<InfoNormalMember>({
        custId: updateInfoCustomerPageConfig.custId,
      });
      reset(data);
      setOriginalCustomerInfo(data);
    };
    fetchCustomerInfo();
  }, [reset]);

  const onSubmit = async (formData: InfoNormalMember) => {
    setLoading(true);
    try {
      if (!isDirty) {
        showResult('error', t('NoChanges'));
        return;
      }
      if (!isValidEmail(formData)) {
        return;
      }

      const response = await UpdateCustomerInfo<UpdateCustomerInfoResponse>({
        CustId: updateInfoCustomerPageConfig.custId,
        FirstName: formData.firstName,
        LastName: formData.lastName,
        Phone: formData.phone,
        MobilePhone: formData.mobilePhone,
        Email: formData.email,
        Fax: formData.fax,
        Remark: formData.remark,
      });

      if (response.isSuccessful) {
        setOriginalCustomerInfo(formData);
        reset(formData);
        showResult('success', t('UpdateSuccess'));
      } else {
        showResult('error', response.message === 'NoPermission' ? t('YouAreNotAuthorized') : response.message);
      }
    } catch (error) {
      showResult('error', t('UpdateFailed'));
    } finally {
      setLoading(false);
    }
  };

  const isValidEmail = (formData: InfoNormalMember) => {
    if (formData.email && !EMAIL_REGEX.test(formData.email)) {
      showResult('error', t('EmailNotValid'));
      return false;
    }
    return true;
  };

  return (
    <>
      <PageTitle />
      <FormProvider {...methods}>
        <form onSubmit={handleSubmit(onSubmit)} className={styles.form}>
          <div className={styles.resultMessageContainer}>{renderResultMessage()}</div>
          <TableSkeleton>
            <thead className={styles.tableHeader}>
              <tr>
                <th colSpan={NumberCols} scope="col">
                  {t('CustomerInformation')}
                </th>
              </tr>
            </thead>
            <tbody className={styles.tableBody}>
              <tr>
                <td className={styles.labelCell}>
                  <Label>{t('firstName')}</Label>
                </td>
                <td>
                  <Controller
                    name="firstName"
                    control={control}
                    render={({ field }) => (
                      <input
                        {...field}
                        type="text"
                        maxLength={FIRST_NAME_MAX_LENGTH}
                        className={styles.input}
                        onBlur={(e) => {
                          const trimmedValue = e.target.value.trim();
                          field.onChange(trimmedValue);
                        }}
                      />
                    )}
                  />
                </td>
                <td className={styles.labelCell}>
                  <Label>{t('lastName')}</Label>
                </td>
                <td>
                  <Controller
                    name="lastName"
                    control={control}
                    render={({ field }) => (
                      <input
                        {...field}
                        type="text"
                        maxLength={LAST_NAME_MAX_LENGTH}
                        className={styles.input}
                        onBlur={(e) => {
                          const trimmedValue = e.target.value.trim();
                          field.onChange(trimmedValue);
                        }}
                      />
                    )}
                  />
                </td>
              </tr>
              <tr>
                <td className={styles.labelCell}>
                  <Label>{t('phone')}</Label>
                </td>
                <td>
                  <Controller
                    name="phone"
                    control={control}
                    render={({ field }) => (
                      <input
                        {...field}
                        type="text"
                        maxLength={PHONE_NUMBER_MAX_LENGTH}
                        className={styles.input}
                        onChange={(e) => {
                          e.preventDefault();
                          const value = e.target.value.replace(NUMBER_REGEX, '').slice(0, PHONE_NUMBER_MAX_LENGTH);
                          field.onChange(value);
                        }}
                        onBlur={(e) => {
                          const trimmedValue = e.target.value.trim();
                          field.onChange(trimmedValue);
                        }}
                      />
                    )}
                  />
                </td>
                <td className={styles.labelCell}>
                  <Label>{t('mobilePhone')}</Label>
                </td>
                <td>
                  <Controller
                    name="mobilePhone"
                    control={control}
                    render={({ field }) => (
                      <input
                        {...field}
                        type="text"
                        maxLength={PHONE_NUMBER_MAX_LENGTH}
                        className={styles.input}
                        onChange={(e) => {
                          e.preventDefault();
                          const value = e.target.value.replace(NUMBER_REGEX, '').slice(0, PHONE_NUMBER_MAX_LENGTH);
                          field.onChange(value);
                        }}
                        onBlur={(e) => {
                          const trimmedValue = e.target.value.trim();
                          field.onChange(trimmedValue);
                        }}
                      />
                    )}
                  />
                </td>
              </tr>
              <tr>
                <td className={styles.labelCell}>
                  <Label>{t('Email')}</Label>
                </td>
                <td>
                  <Controller
                    name="email"
                    control={control}
                    render={({ field }) => (
                      <input
                        {...field}
                        type="text"
                        maxLength={EMAIL_MAX_LENGTH}
                        className={styles.input}
                        onBlur={(e) => {
                          const trimmedValue = e.target.value.trim();
                          field.onChange(trimmedValue);
                        }}
                      />
                    )}
                  />
                </td>
                <td className={styles.labelCell}>
                  <Label>{t('fax')}</Label>
                </td>
                <td>
                  <Controller
                    name="fax"
                    control={control}
                    render={({ field }) => (
                      <input
                        {...field}
                        type="text"
                        maxLength={PHONE_NUMBER_MAX_LENGTH}
                        className={styles.input}
                        onBlur={(e) => {
                          const trimmedValue = e.target.value.trim();
                          field.onChange(trimmedValue);
                        }}
                      />
                    )}
                  />
                </td>
              </tr>
              <tr>
                <td className={styles.labelCell}>
                  <Label>{t('Remark')}</Label>
                </td>
                <td colSpan={3}>
                  <Controller
                    name="remark"
                    control={control}
                    render={({ field }) => (
                      <textarea
                        {...field}
                        rows={3}
                        maxLength={REMARK_MAX_LENGTH}
                        className={styles.textarea}
                        onBlur={(e) => {
                          const trimmedValue = e.target.value.trim();
                          field.onChange(trimmedValue);
                        }}
                      />
                    )}
                  />
                </td>
              </tr>
            </tbody>
          </TableSkeleton>
          <div className={styles.buttonContainer}>
            <ButtonFull text={t('Update')} onClick={handleSubmit(onSubmit)} disabled={loading} />
            <ButtonOutline
              text={t('Reset')}
              onClick={() => {
                reset(originalCustomerInfo);
              }}
              disabled={loading}
            />
          </div>
        </form>
      </FormProvider>
    </>
  );
};
