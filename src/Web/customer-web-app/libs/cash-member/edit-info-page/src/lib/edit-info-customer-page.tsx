import { FormProvider, useForm, Controller } from 'react-hook-form';
import classNames from 'classnames';
import styles from './edit-info-cash-member-page.module.scss';
import { ButtonFull, ButtonOutline, ErrorFieldMessage, Label, Title, useResultMessage, useSystems } from '@customer-web-app/shared/base/components';
import { useEffect, useState } from 'react';
import { GetCustomerInfo, UpdateCustomerInfo } from './edit-info-cash-member-services';
import { useTranslation } from 'react-i18next';
import {
    NUMBER_REGEX,
    PHONE_NUMBER_MAX_LENGTH,
    EMAIL_REGEX,
    EMAIL_MAX_LENGTH,
    LAST_NAME_MAX_LENGTH,
    FIRST_NAME_MAX_LENGTH,
    REMARK_MAX_LENGTH
} from '@customer-web-app/shared-constants';
import { EditInfoTitle } from './edit-info-title';


export const EditInfoCustomerPage = () => {
    const { t } = useTranslation();
    const { isAlpha } = useSystems(updateInfoCustomerPageConfig.systemId);
    const [originalCustomerInfo, setOriginalCustomerInfo] = useState<any>(null);
    const [isLoading, setIsLoading] = useState(false);
    const methods = useForm<any>({
        defaultValues: {},
        mode: 'onChange',
        criteriaMode: 'all',
    });
    const { showResult, renderResultMessage } = useResultMessage();
    const { reset, formState: { errors } } = methods;
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
            reset(data);
            setOriginalCustomerInfo(data);
        }
        fetchCustomerInfo();
    }, [reset, updateInfoCustomerPageConfig.custId]);
    const onSubmit = async (data: any) => {
        setIsLoading(true);
        if (errors.firstName || errors.lastName || errors.mobileCode || errors.mobileNumber || errors.email || errors.fax) {
            return;
        }
        const res = await UpdateCustomerInfo<{ errorCode: string, errorMessage: string, isSuccess: boolean, isSuccessful: boolean }>({
            custId: updateInfoCustomerPageConfig.custId,
            ...data,
        });
        if (res.isSuccess || res.isSuccessful) {
            setOriginalCustomerInfo(data);
            reset(data);
            showResult('success', t('UpdateSuccess'), 1200);
        } else {
            showResult('error', res.errorMessage);
        }
        setIsLoading(false);
    }
    return (
        <>
            <EditInfoTitle />
            <div className={classNames(styles['edit-info-cash-member-form'], isAlpha ? styles['w-50'] : styles['w-100'])}>
                <div style={{ marginBottom: '5px' }}>
                    {renderResultMessage()}
                </div>
                <FormProvider {...methods}>
                    <form onSubmit={methods.handleSubmit(onSubmit)}>
                        <div className={styles['form-group']}>
                            <div className={styles['form-group-row']}>
                                <div className={styles['form-group-item']}>
                                    <Label >{t('firstName')}</Label>
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
                                    <Label >{t('lastName')}</Label>
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
                                    <Label>{t('mobilePhone')}</Label>
                                    <Controller
                                        name="mobilePhone"
                                        render={({ field }) =>
                                            <input
                                                {...field}
                                                maxLength={PHONE_NUMBER_MAX_LENGTH}
                                                onChange={(e) => {
                                                    e.preventDefault();
                                                    const value = e.target.value.replace(NUMBER_REGEX, '').slice(0, PHONE_NUMBER_MAX_LENGTH);
                                                    field.onChange(value);
                                                }}
                                            />}
                                    />
                                    {errors.mobilePhone && <ErrorFieldMessage message={errors.mobilePhone.message as string} />}
                                </div>
                                <div className={styles['form-group-item']}>
                                    <Label>{t('phone')}</Label>
                                    <Controller
                                        name="phone"
                                        render={({ field }) =>
                                            <input
                                                {...field}
                                                maxLength={PHONE_NUMBER_MAX_LENGTH}
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
                                    <Label>{t('email')}</Label>
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
                                                {...field}
                                                maxLength={EMAIL_MAX_LENGTH}
                                                className={classNames(styles['input'], errors.email && styles['input-error'])}
                                                onBlur={(e) => {
                                                    const trimmedValue = e.target.value.trim();
                                                    field.onChange(trimmedValue);
                                                }}
                                            />}
                                    />
                                    {errors.email && <ErrorFieldMessage message={errors.email.message as string} />}
                                </div>
                                <div className={styles['form-group-item']}>
                                    <Label>{t('fax')}</Label>
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
                            <div className={styles['form-group-row']} style={{ justifyContent: isAlpha ? 'flex-start' : 'flex-end' }}>
                                <ButtonFull text={t('Update')} onClick={methods.handleSubmit(onSubmit)} disabled={isLoading} />
                                <ButtonOutline text={t('Reset')} onClick={() => {
                                    reset(originalCustomerInfo);
                                }} disabled={isLoading} />
                            </div>
                        </div>
                    </form>
                </FormProvider>
            </div>
        </>
    )
}