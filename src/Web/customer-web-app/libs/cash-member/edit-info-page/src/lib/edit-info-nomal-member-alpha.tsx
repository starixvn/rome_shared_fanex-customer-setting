import { useTranslation } from "react-i18next";
import { ButtonFull, ButtonOutline, Label, TableSkeleton, Title, useResultMessage } from "@customer-web-app/shared/base/components";
import { Controller, FormProvider, useForm } from "react-hook-form";
import { PageTitle } from "./page-title";
import { FIRST_NAME_MAX_LENGTH, LAST_NAME_MAX_LENGTH, PHONE_NUMBER_MAX_LENGTH, NUMBER_REGEX, EMAIL_MAX_LENGTH, EMAIL_REGEX } from "@customer-web-app/shared-constants";
import styles from "./edit-info-nomal-member-alpha.module.scss";
import { useEffect, useState } from "react";
import { GetCustomerInfo, UpdateCustomerInfo } from "./edit-info-cash-member-services";
import { UpdateCustomerInfoResponse } from "./edit-info-cash-member-types";

export const EditInfoNomalMemberAlpha = () => {
    const NumberCols = 4;
    const { t } = useTranslation();
    const [originalCustomerInfo, setOriginalCustomerInfo] = useState<any>({
        firstName: '',
        lastName: '',
        mobilePhone: '',
        mobileCode: '',
        mobileNumber: '',
        phone: '',
        email: '',
        fax: ''
    });
    const [loading, setLoading] = useState(false);
    const { showResult, renderResultMessage } = useResultMessage();

    const methods = useForm<any>({
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
            const data = await GetCustomerInfo<any>({
                custId: updateInfoCustomerPageConfig.custId
            });
            reset(data);
            setOriginalCustomerInfo(data);
        }
        fetchCustomerInfo();
    }, [reset]);

    const onSubmit = async (formData: any) => {
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
                FirstName: formData.firstName,
                LastName: formData.lastName,
                Phone: formData.phone,
                Mobile: formData.mobilePhone,
                Email: formData.email,
                Fax: formData.fax
            });

            if (response.isSuccessful) {
                setOriginalCustomerInfo(formData);
                reset(formData);
                showResult('success', t('UpdateSuccess'));
            } else {
                showResult('error', response.errorMessage || response.message);
            }

        } catch (error) {
            showResult('error', t('UpdateFailed'));
        } finally {
            setLoading(false);
        }
    };

    const isValidEmail = (formData: any) => {
        if (formData.email && !EMAIL_REGEX.test(formData.email)) {
            showResult('error', t('EmailNotValid'));
            return false;
        }
        return true;
    }

    return (
        <>
            <PageTitle />
            <div className={styles.editInfoContainer}>
                <div className={styles.resultMessageContainer}>
                    {renderResultMessage()}
                </div>
                <FormProvider {...methods}>
                    <form onSubmit={handleSubmit(onSubmit)} className={styles.form}>
                        <TableSkeleton>
                            <thead className={styles.tableHeader}>
                                <tr>
                                    <th colSpan={NumberCols} scope="col">{t('CustomerInformation')}</th>
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
                                        <Label>{t('email')}</Label>
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
                            </tbody>
                        </TableSkeleton>
                        <div className={styles.buttonContainer}>
                            <ButtonFull text={t('Update')} onClick={handleSubmit(onSubmit)} disabled={loading} />
                            <ButtonOutline text={t('Reset')} onClick={() => {reset(originalCustomerInfo);}} disabled={loading} />
                        </div>
                    </form>
                </FormProvider>
            </div>
        </>
    );
};