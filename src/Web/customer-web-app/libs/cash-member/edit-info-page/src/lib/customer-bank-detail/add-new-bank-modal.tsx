import { DropdownOption, SingleFilterableDropdown } from "@customer-web-app/shared/base/components";
import { useTranslation } from "react-i18next";
import { useState, useEffect } from "react";
import { useResultMessage } from "@customer-web-app/shared/base/components";
import { AddMemberBankInfo, GetBanks, GetCustomerBankInfo } from "../edit-info-cash-member-services";
import { FormProvider, useForm, Controller } from 'react-hook-form';
import { ButtonIcon, ButtonFull, ButtonCancel } from "@customer-web-app/shared/base/components";
import { IconClose2 } from "@customer-web-app/shared/base/common-icon";
import styles from "./add-new-bank-modal.module.scss";
import { Label, ErrorFieldMessage } from "@customer-web-app/shared/base/components";
import { NUMBER_REGEX } from '@customer-web-app/shared-constants';
import classNames from "classnames";


interface AddBankModalProps {
    onCancel: () => void;
    onAddSuccess: () => void;
}

interface Bank {
    bankId: number;
    bankName: string;
    bankCode: string;
    bankAccountNumber: string;
}

interface AddMemberBankFormData {
    memberId: number;
    memberBankID: number;
    agentBankID: number;
    memberBankAccountNumber: string;
    agentBankAccountNumber: string;
    memberBankDropdownOptions: DropdownOption[];
    agentBankDropdownOptions: DropdownOption[];
    agentBanks: any[];
}
export const AddBankModal = ({ onCancel, onAddSuccess }: AddBankModalProps) => {
    const { t } = useTranslation();
    const [isLoading, setIsLoading] = useState(false);
    const [isBlockAction, setIsBlockAction] = useState(false);
    const { renderResultMessage, showResult, clearResult } = useResultMessage();

    useEffect(() => {
        loadModalData();
    }, []);

    const loadModalData = async () => {
        const memberBanks = await GetBanks<Bank[]>({});
        const memberBankDropdownOptions: DropdownOption[] = memberBanks.map((bank) => ({
            key: bank.bankId,
            value: bank.bankId,
            label: bank.bankName,
            icon: <img src={`${updateInfoCustomerPageConfig.cdnUri}cash-member/create/bank/${bank.bankCode?.toUpperCase()}.png`} alt={bank.bankName} className={styles.bankIcon} />
        }));
        const noMachingBank = {
            bankId: 0,
            bankName: '',
            bankCode: '',
            bankAccountNumber: '',
        };
        const agentBanks = await GetCustomerBankInfo<Bank[]>({
            roleId: 2
        });
        const agentBanksWithNoMachingBank = [noMachingBank, ...agentBanks];

        const agentBankDropdownOptions: DropdownOption[] = [{
            key: '0|0',
            value: '0|0',
            label: t('NoMatchingBankAccount'),
            icon: null
        }, ...agentBanks.map((bank) => ({
            key: bank.bankId + '|' + bank.bankAccountNumber,
            value: bank.bankId + '|' + bank.bankAccountNumber,
            label: bank.bankAccountNumber,
            icon: <img src={`${updateInfoCustomerPageConfig.cdnUri}cash-member/create/bank/${bank.bankCode?.toUpperCase()}.png`} alt={bank.bankName} className={styles.bankIcon} />
        }))];

        reset({
            memberId: updateInfoCustomerPageConfig.custId,
            memberBankID: memberBanks[0].bankId,
            memberBankDropdownOptions: memberBankDropdownOptions,
            agentBankID: agentBanksWithNoMachingBank[0].bankId,
            agentBankDropdownOptions: agentBankDropdownOptions,
            agentBanks: agentBanksWithNoMachingBank,
        });
    }

    const methods = useForm<AddMemberBankFormData>({
        mode: 'onChange',
        criteriaMode: 'all',
    });

    const {
        handleSubmit,
        control,
        watch,
        formState: { errors },
        reset,
        setValue,
    } = methods;

    const onSubmit = async (formData: AddMemberBankFormData) => {
        try {
            setIsLoading(true);
            setIsBlockAction(true);
            const response = await AddMemberBankInfo<any>({
                CustId: formData.memberId,
                MemberBankId: formData.memberBankID,
                MemberBankAccountNumber: formData.memberBankAccountNumber,
                AgentBankId: formData.agentBankID,
                AgentBankAccountNumber: formData.agentBankAccountNumber,
            });

            if (response.isSuccessful) {
                showResult('success', t('AddSuccess'), 1000, onAddSuccess);
            } else {
                showResult('error', response.message);
            }

        } catch (error) {
            console.error('Error adding bank account:', error);
            showResult('error', t('AddFailed'));
        } finally {
            setIsLoading(false);
            setIsBlockAction(false);
        }
    };

    return (
        <div className={styles.modalOverlay}>
            <div className={styles.modalContainer}>
                <div className={styles.modalHeader}>
                    <Label>{t('AddBankAccount')}</Label>
                    <ButtonIcon
                        onClick={onCancel}
                        icon={<IconClose2 />}
                        className={styles.closeModal}
                    />
                </div>
                <div>
                    <div className={styles.resultMessage}>
                        {renderResultMessage()}
                    </div>
                    <FormProvider {...methods}>
                        <form onSubmit={handleSubmit(onSubmit)} className={styles.form}>
                            <div className={styles.formField}>
                                <Label>{t('MemberBankAccount')}</Label>
                                <SingleFilterableDropdown
                                    dropdownOptions={watch('memberBankDropdownOptions') ?? []}
                                    onSelect={(option) => {
                                        setValue('memberBankID', Number(option.value));
                                    }}
                                    defaultOption={watch('memberBankDropdownOptions')?.[0] as DropdownOption}
                                />
                                {errors.memberBankID && (
                                    <div className={styles.errorMessage}>{errors.memberBankID.message}</div>
                                )}
                            </div>

                            <div className={styles.formField}>
                                <Label required>{t('MemberBankAccountNumber')}</Label>
                                <Controller
                                    name="memberBankAccountNumber"
                                    control={control}
                                    rules={{ required: t('AccountNumberRequired') || 'Account number is required' }}
                                    render={({ field }) => (
                                        <input
                                            {...field}
                                            maxLength={50}
                                            type="text"
                                            className={classNames(styles.inputMemberBankAccountNumber, {
                                                [styles.error]: errors.memberBankAccountNumber
                                            })}
                                            onChange={(e) => {
                                                e.preventDefault();
                                                const value = e.target.value.replace(NUMBER_REGEX, '').slice(0, 50);
                                                field.onChange(value);
                                            }}
                                        />
                                    )}
                                />
                                {errors.memberBankAccountNumber && <ErrorFieldMessage message={errors.memberBankAccountNumber.message as string} />}
                            </div>
                            <div className={styles.formField}>
                                <Label>{t('AgentBankAccount')}</Label>
                                <SingleFilterableDropdown
                                    dropdownOptions={watch('agentBankDropdownOptions') ?? []}
                                    onSelect={(option) => {
                                        const selectedBank = watch('agentBanks')?.find((opt) => (opt.bankId + '|' + opt.bankAccountNumber) === option.value);
                                        setValue('agentBankID', selectedBank?.bankId ?? 0);
                                        setValue('agentBankAccountNumber', selectedBank?.bankAccountNumber ?? '');
                                    }}
                                    defaultOption={watch('agentBankDropdownOptions')?.[0] as DropdownOption}
                                />
                            </div>

                            <div className={styles.buttonContainer}>
                                <ButtonCancel
                                    text={t('Cancel')}
                                    onClick={() => {
                                        onCancel();
                                        clearResult();
                                    }}
                                    disabled={isLoading || isBlockAction}
                                />
                                <ButtonFull
                                    text={isLoading ? t('Loading') : t('Save')}
                                    onClick={handleSubmit(onSubmit)}
                                    disabled={isLoading || isBlockAction}
                                />
                            </div>
                        </form>
                    </FormProvider>
                </div>
            </div>
        </div>
    )
}
