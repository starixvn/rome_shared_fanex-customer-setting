import { FormProvider, useForm, Controller } from 'react-hook-form';
import styles from './edit-info-cash-member-page.module.scss';
import { ButtonFull, ButtonOutline, Label, useResultMessage, useSystems } from '@customer-web-app/shared/base/components';
import { useEffect, useState } from 'react';
import { GetCustomerInfo, UpdateCustomerInfo } from './edit-info-cash-member-services';
import { splitMobile } from '@customer-web-app/shared-helpers';
import { useTranslation } from 'react-i18next';
import {
    PHONE_CODE_MAX_LENGTH,
    NUMBER_REGEX,
    PHONE_NUMBER_MAX_LENGTH,
    EMAIL_REGEX,
    EMAIL_MAX_LENGTH,
    LAST_NAME_MAX_LENGTH,
    FIRST_NAME_MAX_LENGTH,
    REMARK_MAX_LENGTH
} from '@customer-web-app/shared-constants';
import classNames from 'classnames';

export const EditInfoCustomerPage = () => {
    const { t } = useTranslation();
    const { isAlpha } = useSystems(updateInfoCustomerPageConfig.systemId);
    const [originalCustomerInfo, setOriginalCustomerInfo] = useState<any>(null);
    const methods = useForm<any>({
        defaultValues: {},
        mode: 'onChange',
        criteriaMode: 'all',
    });
    const { showResult, renderResultMessage } = useResultMessage();
    const { control, reset, formState: { errors } } = methods;
    useEffect(() => {
        const fetchCustomerInfo = async () => {
            const data = await GetCustomerInfo<{
                custId: number;
                firstName: string;
                lastName: string;
                mobilePhone: string;
                mobileCode: string;
                mobileNumber: string;
                phone: string;
                email: string;
                fax: string;
                remark: string;
            }>({
                custId: updateInfoCustomerPageConfig.custId
            });
            const mobile = splitMobile(data.mobilePhone);
            data.mobileCode = mobile.countryCode;
            data.mobileNumber = mobile.mobile;
            reset(data);
            setOriginalCustomerInfo(data);
        }
        fetchCustomerInfo();
    }, [reset, updateInfoCustomerPageConfig.custId]);
    const onSubmit = async (data: any) => {
        if (errors.firstName || errors.lastName || errors.mobileCode || errors.mobileNumber || errors.email || errors.fax) {
            return;
        }
        const res = await UpdateCustomerInfo<{ errorCode: string, errorMessage: string, isSuccess: boolean, isSuccessful: boolean }>({
            custId: updateInfoCustomerPageConfig.custId,
            ...data,
            mobilePhone: `${data.mobileCode}-${data.mobileNumber}`
        });
        if (res.isSuccess || res.isSuccessful) {
            showResult('success', t('UpdateSuccess'), 12000);
        } else {
            showResult('error', res.errorMessage);
        }
    }
    return (
        <div className={styles['edit-info-cash-member-form']}>
            {renderResultMessage()}
            <FormProvider {...methods}>
                <form onSubmit={methods.handleSubmit(onSubmit)}>
                    <div className={styles['form-group']}>
                        <div className={styles['form-group-row']}>
                            <div className={styles['form-group-item']}>
                                <Label required>{t('FirstName')}</Label>
                                <Controller
                                    name="firstName"
                                    render={({ field }) =>
                                        <input
                                            {...field}
                                            maxLength={FIRST_NAME_MAX_LENGTH}
                                            className={classNames(styles['input'], errors.firstName && styles['input-error'])}
                                            onBlur={(e) => {
                                                const trimmedValue = e.target.value.trim();
                                                field.onChange(trimmedValue);
                                            }}
                                        />}
                                />
                            </div>
                            <div className={styles['form-group-item']}>
                                <Label required>{t('LastName')}</Label>
                                <Controller
                                    name="lastName"
                                    render={({ field }) =>
                                        <input
                                            {...field}
                                            maxLength={LAST_NAME_MAX_LENGTH}
                                            className={classNames(styles['input'], errors.lastName && styles['input-error'])}
                                            onBlur={(e) => {
                                                const trimmedValue = e.target.value.trim();
                                                field.onChange(trimmedValue);
                                            }}
                                        />}
                                />
                            </div>
                        </div>
                        <div className={styles['form-group-row']}>
                            <div className={styles['form-group-item']}>
                                <Label required>{t('Mobile')}</Label>
                                <Controller
                                    name="mobilePhone"
                                    render={({ field }) =>
                                        <input {...field}
                                            onChange={(e) => {
                                                e.preventDefault();
                                                const value = e.target.value.replace(NUMBER_REGEX, '').slice(0, PHONE_NUMBER_MAX_LENGTH);
                                                field.onChange(value);
                                            }}
                                        />}
                                />
                                {/* <div className={classNames("control-field", styles['control-field-mobile'], (errors.mobileCode || errors.mobileNumber) && styles['input-error'])}>
                                    <span className="text-gray-500">+</span>
                                    <Controller
                                        name="mobileCode"
                                        control={control}
                                        rules={{ required: t('MobileNumberRequired') }}
                                        render={({ field }) => (
                                            <input
                                                {...field}
                                                id="mobileCode"
                                                type="text"
                                                style={{ border: 'none', outline: 'none' }}
                                                className={styles['mobile-code']}
                                                maxLength={PHONE_CODE_MAX_LENGTH}
                                                onChange={(e) => {
                                                    e.preventDefault();
                                                    const value = e.target.value.replace(NUMBER_REGEX, '').slice(0, PHONE_CODE_MAX_LENGTH);
                                                    field.onChange(value);
                                                }}
                                            />
                                        )}
                                    />
                                    <div className="w-px h-4 sm:h-6 bg-gray-300"></div>
                                    <Controller
                                        name="mobileNumber"
                                        control={control}
                                        rules={{ required: t('MobileNumberRequired') }}
                                        render={({ field }) => (
                                            <input
                                                {...field}
                                                id="mobileNumber"
                                                className={styles['mobile-number']}
                                                type="text"
                                                maxLength={PHONE_NUMBER_MAX_LENGTH}
                                                onChange={(e) => {
                                                    e.preventDefault();
                                                    const value = e.target.value.replace(NUMBER_REGEX, '').slice(0, PHONE_NUMBER_MAX_LENGTH);
                                                    field.onChange(value);
                                                }}
                                            />
                                        )}
                                    />
                                </div> */}
                                {(errors.mobileCode || errors.mobileNumber) && <div style={{ color: 'red', fontSize: '12px' }}>
                                    {(errors.mobileCode?.message || errors.mobileNumber?.message) as string}
                                </div>}
                            </div>
                            <div className={styles['form-group-item']}>
                                <Label>{t('Phone')}</Label>
                                <Controller
                                    name="phone"
                                    render={({ field }) =>
                                        <input {...field}
                                            onChange={(e) => {
                                                e.preventDefault();
                                                const value = e.target.value.replace(NUMBER_REGEX, '').slice(0, PHONE_NUMBER_MAX_LENGTH);
                                                field.onChange(value);
                                            }}
                                        />}
                                />
                            </div>
                        </div>
                        <div className={styles['form-group-row']}>
                            <div className={styles['form-group-item']}>
                                <Label>{t('Email')}</Label>
                                <Controller
                                    name="email"
                                    rules={{
                                        pattern: {
                                            value: EMAIL_REGEX,
                                            message: t('EmailNotValid')
                                        }
                                    }}
                                    render={({ field }) =>
                                        <input
                                            maxLength={EMAIL_MAX_LENGTH}
                                            {...field}
                                            onBlur={(e) => {
                                                const trimmedValue = e.target.value.trim();
                                                field.onChange(trimmedValue);
                                            }}
                                        />}
                                />
                                {errors.email && <div style={{ color: 'red', fontSize: '12px' }}>{errors.email.message as string}</div>}
                            </div>
                            <div className={styles['form-group-item']}>
                                <Label>{t('Fax')}</Label>
                                <Controller
                                    name="fax"
                                    render={({ field }) =>
                                        <input
                                            {...field}
                                            onBlur={(e) => {
                                                const trimmedValue = e.target.value.trim();
                                                field.onChange(trimmedValue);
                                            }}
                                        />}
                                />
                            </div>
                        </div>
                        {!isAlpha && <div className={styles['form-group-item']}>
                            <Label>{t('Remark')}</Label>
                            <Controller
                                name="remark"
                                render={({ field }) =>
                                    <textarea
                                        {...field}
                                        maxLength={REMARK_MAX_LENGTH}
                                        style={{ height: '100px' }}
                                        onBlur={(e) => {
                                            const trimmedValue = e.target.value.trim();
                                            field.onChange(trimmedValue);
                                        }}
                                    />}
                            />
                        </div>}
                        <div className={styles['form-group-row']}>
                            <ButtonFull text={t('Update')} onClick={methods.handleSubmit(onSubmit)} />
                            <ButtonOutline text={t('Reset')} onClick={() => {
                                reset(originalCustomerInfo);
                            }} />
                        </div>
                    </div>
                </form>
            </FormProvider>
        </div>
    )
}