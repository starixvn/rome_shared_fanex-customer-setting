import { useState, useEffect, useMemo, useCallback } from "react";
import { MemberBankAccountRecord, UpdateMemberBankInfoResponse } from "../edit-info-cash-member-types";
import { useTranslation } from "react-i18next";
import { DropdownOption, TableSkeleton } from "@customer-web-app/shared/base/components";
import { IconExclamationPoint, IconInfo, NoDataIcon } from "@customer-web-app/shared/base/common-icon";
import { ButtonFull } from "@customer-web-app/shared/base/components";
import { useResultMessage } from "@customer-web-app/shared/base/components";
import { GetCustomerBankInfo, UpdateMemberBankInfo } from '../edit-info-cash-member-services'
import { BankAccountRow } from './grid-components';
import styles from "./customer-bank-detail.module.scss";
import { AddBankModal } from "./add-new-bank-modal";
import { DeleteBankModal } from "./delete-bank-modal";
import { WrapperTooltip } from "@customer-web-app/wrapper-tooltip";

interface Bank {
    bankId: number;
    bankName: string;
    bankCode: string;
    bankAccountNumber: string;
}

interface EditingRow {
    id: number;
    memberBankID: number;
    memberBankAccountID: number;
    memberBankAccountNumber: string;
    agentBankID: number;
    agentBankAccountNumber: string;
    agentBankCode: string;
}

export const CustomerBankDetail = () => {
    const { t } = useTranslation();
    const [showAddBankAccountPopup, setShowAddBankAccountPopup] = useState(false);
    const [bankAccounts, setBankAccounts] = useState<MemberBankAccountRecord[]>([]);
    const [isLoading, setIsLoading] = useState(false);
    const [editingRow, setEditingRow] = useState<EditingRow | null>(null);
    const [agentBankOptions, setAgentBankOptions] = useState<DropdownOption[]>([]);
    const [agentBanks, setAgentBanks] = useState<Bank[]>([]);
    const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
    const [deleteAccountId, setDeleteAccountId] = useState<number | null>(null);
    const { renderResultMessage, showResult } = useResultMessage();

    // Memoized values
    const editingRowId = useMemo(() => editingRow?.id, [editingRow?.id]);

    useEffect(() => {
        setIsLoading(true);
        const initializeData = async () => {
            await Promise.all([
                loadBankAccounts(),
                loadAgentBankOptions()
            ]);
        };
        initializeData().finally(() => {
            setIsLoading(false);
        });
    }, []);

    const loadBankAccounts = useCallback(async () => {

        const memberBanks = await GetCustomerBankInfo<MemberBankAccountRecord[]>({
            custId: updateInfoCustomerPageConfig.custId,
            roleId: 1
        });
        setBankAccounts(memberBanks);
    }, []);

    const loadAgentBankOptions = useCallback(async () => {
        const noMachingBank = {
            bankId: 0,
            bankName: '',
            bankCode: '',
            bankAccountNumber: '',
        }
        const agentBanks = await GetCustomerBankInfo<Bank[]>({
            custId: updateInfoCustomerPageConfig.agentId,
            roleId: 2
        });
        const agentBanksWithNoMachingBank = [noMachingBank, ...agentBanks];
        setAgentBanks(agentBanksWithNoMachingBank);

        const agentBankDropdownOptions: DropdownOption[] = [{
            key: noMachingBank.bankId + '|' + noMachingBank.bankAccountNumber,
            value: noMachingBank.bankId + '|' + noMachingBank.bankAccountNumber,
            label: t('NoMatchingBankAccount'),
        }, ...agentBanks.map((bank) => ({
            key: bank.bankId + '|' + bank.bankAccountNumber,
            value: bank.bankId + '|' + bank.bankAccountNumber,
            label: bank.bankAccountNumber,
            icon: <img
                src={`${updateInfoCustomerPageConfig.cdnUri}cash-member/create/bank/${bank.bankCode?.toUpperCase()}.png`}
                alt={bank.bankName}
                className={styles.bankIcon}
                loading="lazy"
            />
        }))];
        setAgentBankOptions(agentBankDropdownOptions);
    }, []);

    const handleEditBankAccount = useCallback((account: MemberBankAccountRecord) => {
        setEditingRow({
            id: account.memberBankAccountID,
            memberBankID: account.memberBankID,
            memberBankAccountID: account.memberBankAccountID,
            memberBankAccountNumber: account.memberBankAccountNumber,
            agentBankID: account.agentBankID,
            agentBankAccountNumber: account.agentBankAccountNumber,
            agentBankCode: account.agentBankCode,
        });
    }, []);

    const handleDeleteBankAccount = useCallback((id: number) => {
        setEditingRow(null);
        setDeleteAccountId(id);
        setShowDeleteConfirm(true);
    }, []);

    const handleCancelEdit = useCallback(() => {
        setEditingRow(null);
    }, []);

    const handleSaveEdit = async () => {
        if (!editingRow) return;
        try {
            setIsLoading(true);
            const response = await UpdateMemberBankInfo<UpdateMemberBankInfoResponse>({
                custId: updateInfoCustomerPageConfig.custId,
                memberBankID: editingRow.memberBankID,
                memberBankAccountID: editingRow.memberBankAccountID,
                memberBankAccountNumber: editingRow.memberBankAccountNumber,
                agentBankID: editingRow.agentBankID,
                agentBankAccountNumber: editingRow.agentBankAccountNumber,
                agentBankCode: editingRow.agentBankCode,
            });

            if (response.isSuccessful) {
                // Update local state only after successful API call
                setBankAccounts(prev => prev.map(account =>
                    account.memberBankAccountID === editingRow.id
                        ? {
                            ...account,
                            agentBankID: editingRow.agentBankID,
                            agentBankAccountNumber: editingRow.agentBankAccountNumber,
                            agentBankCode: editingRow.agentBankCode,
                        }
                        : account
                ));
                showResult('success', t('UpdateSuccess'));
            } else {
                showResult('error', response.message || response.errorMessage);
            }

        } catch (error) {
            console.error('Error updating bank account:', error);
            showResult('error', t('updateError') || 'Failed to update bank account');
        } finally {
            setIsLoading(false);
            setEditingRow(null);
        }
    };

    const handleAgentBankChange = (option: DropdownOption) => {
        if (!editingRow) return;
        const selectedBank = agentBanks.find(bank => (bank.bankId + '|' + bank.bankAccountNumber) === option.value);
        if (selectedBank) {
            setEditingRow(prev => prev ? {
                ...prev,
                agentBankID: selectedBank.bankId,
                agentBankAccountNumber: selectedBank.bankAccountNumber,
                agentBankCode: selectedBank.bankCode,
            } : null);
        }
    };

    const handleAddBankClick = useCallback(() => {
        setEditingRow(null);
        setShowAddBankAccountPopup(true);
    }, []);

    return (
        <>
            <div className={styles.buttonContainer}>
                <ButtonFull
                    className={styles.addButton}
                    onClick={handleAddBankClick}
                    disabled={isLoading}
                    text={t('AddBankAccount')}
                />
            </div>
            <div className={styles.gridTable}>
                <div style={{ marginBottom: '5px' }}>
                    {renderResultMessage()}
                </div>
                <TableSkeleton>
                    <thead>
                        <tr>
                            <th scope="col" style={{ maxWidth: '50px' }}>#</th>
                            <th scope="col" style={{ maxWidth: '300px' }}>{t('MemberBankAccount')}</th>
                            <th scope="col" style={{ maxWidth: '300px' }}>
                                    {t('AgentBankAccount')}
                                    <span data-tooltip-id="agent-bank-tooltip">
                                        <IconExclamationPoint />
                                    </span>
                                    <WrapperTooltip
                                        id="agent-bank-tooltip"
                                        content={t('AgentBankAccountTooltip')}
                                        place="top"
                                    />
                            </th>
                            <th scope="col" style={{ maxWidth: '150px' }}>{t('Action')}</th>
                        </tr>
                    </thead>
                    {bankAccounts.length === 0 && (
                        <tbody>
                            <NoRecordFound colSpan={4} />
                        </tbody>
                    )}
                    <tbody>
                        {bankAccounts.length > 0 && !isLoading && (
                            bankAccounts.map((account, index) => (
                                <BankAccountRow
                                    key={account.memberBankAccountID}
                                    account={account}
                                    index={index}
                                    isEditing={editingRowId === account.memberBankAccountID}
                                    editingRow={editingRow}
                                    agentBankOptions={agentBankOptions}
                                    onEdit={handleEditBankAccount}
                                    onDelete={handleDeleteBankAccount}
                                    onSave={handleSaveEdit}
                                    onCancel={handleCancelEdit}
                                    onAgentBankChange={handleAgentBankChange}
                                    isLoading={isLoading}
                                />
                            ))
                        )}
                    </tbody>
                </TableSkeleton>
            </div>
            {showAddBankAccountPopup && (
                <AddBankModal
                    onCancel={() => {
                        setShowAddBankAccountPopup(false);
                    }}
                    onAddSuccess={() => {
                        setShowAddBankAccountPopup(false);
                        loadBankAccounts();
                    }}
                />
            )}
            {showDeleteConfirm && (
                <DeleteBankModal
                    record={bankAccounts.find((record: MemberBankAccountRecord) => record.memberBankAccountID === deleteAccountId) as MemberBankAccountRecord}
                    onDeleteSuccess={() => {
                        setShowDeleteConfirm(false);
                        setDeleteAccountId(null);
                        loadBankAccounts();
                    }}
                    onCancel={() => {
                        setShowDeleteConfirm(false);
                        setDeleteAccountId(null);
                    }}
                />
            )}
        </>
    );
};

const NoRecordFound = ({ colSpan }: { colSpan: number }) => {
    const { t } = useTranslation();
    return (
        <tr>
            <td colSpan={colSpan} className={styles.noRecordCell}>
                <div className={styles.noRecordContainer} role="alert">
                    <NoDataIcon />
                    <span>{t('NoData') || 'No data'}</span>
                </div>
            </td>
        </tr>
    );
}