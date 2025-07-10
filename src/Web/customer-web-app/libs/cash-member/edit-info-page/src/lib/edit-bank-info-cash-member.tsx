import { useState, useEffect, useMemo, useCallback, memo } from "react";
// import {
//     ButtonFullComponent,
//     ButtonPenComponent,
//     ButtonDeleteComponent,
// } from "../../../../components/Buttons";
import { MemberBankAccountRecord } from "./edit-info-cash-member-types";

import { useTranslation } from "react-i18next";
import { SingleFilterableDropdown, DropdownOption, TableSkeleton } from "@customer-web-app/shared/base/components";
import { IconTick, IconClose2, IconPen, NoDataIcon } from "@customer-web-app/shared/base/common-icon";
import { ButtonIcon, ButtonFull } from "@customer-web-app/shared/base/components";
import { useResultMessage } from "@customer-web-app/shared/base/components";
import { GetCustomerBankInfo, RemoveMemberBankInfo, UpdateMemberBankInfo } from './edit-info-cash-member-services'
import styles from './edit-info-cash-member-page.module.scss';
// import NoRecordFound from "../../../../components/NoRecordFoundComponent";
// import { MemberBankAccountRecord } from "../../types/UpdateCustomerType";
// import { AddBankModal } from "./AddBankModal";
// import { GetCustomerBankInfo, RemoveMemberBankInfo, UpdateMemberBankInfo } from "../../services/CustomerInfoServices";
// import { IconClose, IconInfo, IconTick } from "../../../../components/Icons";
// import { useResultMessage } from "../../../../hooks/useResultMessage";
// import ConfirmPopup from "../../../../components/ConfirmPopup";
// import Tippy from "@tippyjs/react";

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

// Memoized bank cell component
const BankCell = memo(({ accountNumber, bankCode }: { accountNumber: string, bankCode: string }) => {
    if (!accountNumber || !bankCode) return <div className="text-left">No Matching Bank Account</div>;

    return (
        <div className="flex items-center">
            <img
                src={`${updateInfoCustomerPageConfig.cdnUri}cash-member/create/bank/${bankCode}.png`}
                alt={`${bankCode} bank icon`}
                className="w-[16px] h-[16px] inline-block mr-[5px]"
                loading="lazy"
            />
            {accountNumber}
        </div>
    );
});

BankCell.displayName = 'BankCell';

// Memoized table row component
const TableRow = memo(({
    account,
    index,
    isEditing,
    editingRow,
    agentBankOptions,
    onEdit,
    onDelete,
    onSave,
    onCancel,
    onAgentBankChange,
    isLoading
}: {
    account: MemberBankAccountRecord;
    index: number;
    isEditing: boolean;
    editingRow: EditingRow | null;
    agentBankOptions: DropdownOption[];
    onEdit: (account: MemberBankAccountRecord) => void;
    onDelete: (id: number) => void;
    onSave: () => void;
    onCancel: () => void;
    onAgentBankChange: (option: DropdownOption) => void;
    isLoading: boolean;
}) => {

    const renderAgentBankCell = () => {
        if (isEditing) {
            return (
                <SingleFilterableDropdown
                    dropdownOptions={agentBankOptions}
                    onSelect={onAgentBankChange}
                    defaultOption={agentBankOptions.find(option => option.value === editingRow?.agentBankID + '|' + editingRow?.agentBankAccountNumber)}
                    className="w-full"
                />
            );
        }

        return (
            <BankCell
                accountNumber={account.agentBankAccountNumber}
                bankCode={account.agentBankCode}
            />
        );
    };

    const renderActionCell = () => {
        if (isEditing) {
            return (
                <div className="flex justify-center gap-2">
                    <button
                        onClick={onSave}
                        className="p-1 hover:bg-gray-100 rounded"
                        disabled={isLoading}
                    >
                        <IconTick className="w-[20px] h-[20px] fill-[#52A220]! dark:fill-[#52A220]" />
                    </button>
                    <button
                        onClick={onCancel}
                        className="p-1 hover:bg-gray-100 rounded"
                        disabled={isLoading}
                    >
                        <IconClose2 className="w-[20px] h-[20px] fill-[#D33333] dark:fill-[#ededed]" />
                    </button>
                </div>
            );  
        }

        return (
            <div className="flex justify-center">
                <ButtonIcon
                    onClick={() => onEdit(account)}
                    disabled={isLoading}
                    icon={<IconPen />}
                />
                <ButtonIcon
                    onClick={() => onDelete(account.memberBankAccountID)}
                    disabled={isLoading}
                    icon={<IconClose2 />}
                />
            </div>
        );
    };

    return (
        <tr key={account.memberBankAccountID}>
            <td>{index + 1}</td>
            <td>
                <BankCell
                    accountNumber={account.memberBankAccountNumber}
                    bankCode={account.memberBankCode}
                />
            </td>
            <td>{renderAgentBankCell()}</td>
            <td>{renderActionCell()}</td>
        </tr>
    );
});

TableRow.displayName = 'TableRow';

export const EditBankInfoCashMember = () => {
    const { t } = useTranslation('customerSites');
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
        const agentBanks = await GetCustomerBankInfo<Bank[]>({
            roleId: 2
        });
        setAgentBanks(agentBanks);

        const agentBankDropdownOptions: DropdownOption[] = agentBanks.map((bank) => ({
            key: bank.bankId + '|' + bank.bankAccountNumber,
            value: bank.bankId + '|' + bank.bankAccountNumber,
            label: bank.bankAccountNumber,
            icon: <img
                src={`${updateInfoCustomerPageConfig.cdnUri}cash-member/create/bank/${bank.bankCode?.toUpperCase()}.png`}
                alt={bank.bankName}
                className="w-[16px] h-[16px] inline-block mr-[5px]"
                loading="lazy"
            />
        }));
        setAgentBankOptions(agentBankDropdownOptions);
    }, []);

    const handleDeleteBankAccount = useCallback(async (id: number) => {
        setEditingRow(null);
        setDeleteAccountId(id);
        setShowDeleteConfirm(true);
    }, []);

    const handleConfirmDelete = useCallback(async () => {
        if (!deleteAccountId) return;

        try {
            setIsLoading(true);
            const response = await RemoveMemberBankInfo<any>({
                custId: updateInfoCustomerPageConfig.custId,
                memberBankAccountID: deleteAccountId,
            });
            setBankAccounts(prev => prev.filter(account => account.memberBankAccountID !== deleteAccountId));
            if (response.isSuccessful) {
                showResult('success', t('global:removeSuccess') || 'Bank account removed successfully');
            } else {
                showResult('error', response.message);
            }
        } catch (error) {
            console.error('Error deleting bank account:', error);
            showResult('error', t('global:removeError') || 'Failed to remove bank account');
            loadBankAccounts();
        } finally {
            setIsLoading(false);
            setShowDeleteConfirm(false);
            setDeleteAccountId(null);
        }
    }, [deleteAccountId, t]);

    const handleCancelDelete = useCallback(() => {
        setShowDeleteConfirm(false);
        setDeleteAccountId(null);
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

    const handleCancelEdit = useCallback(() => {
        setEditingRow(null);
    }, []);

    const handleSaveEdit = useCallback(async () => {
        if (!editingRow) return;

        try {
            setIsLoading(true);
            const response = await UpdateMemberBankInfo<any>({
                custId: updateInfoCustomerPageConfig.custId,
                memberBankID: editingRow.memberBankID,
                memberBankAccountID: editingRow.memberBankAccountID,
                memberBankAccountNumber: editingRow.memberBankAccountNumber,
                agentBankID: editingRow.agentBankID,
                agentBankAccountNumber: editingRow.agentBankAccountNumber,
                agentBankCode: editingRow.agentBankCode,
            });
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
            if (response.isSuccessful) {
                showResult('success', t('global:updateSuccess'));
            } else {
                showResult('error', response.message);
            }

        } catch (error) {
            console.error('Error updating bank account:', error);
            showResult('error', t('updateInfoCashMember.updateError') || 'Failed to update bank account');
        } finally {
            setIsLoading(false);
            setEditingRow(null);
        }
    }, [editingRow, t]);

    const handleAgentBankChange = useCallback((option: DropdownOption) => {
        if (!editingRow) return;
        const selectedBank = agentBanks.find(bank => (bank.bankId + '|' + bank.bankAccountNumber) === option.value);
        if (selectedBank) {
            setEditingRow({
                ...editingRow,
                agentBankID: selectedBank.bankId,
                agentBankAccountNumber: selectedBank.bankAccountNumber,
                agentBankCode: selectedBank.bankCode,
            });
        }
    }, [editingRow, agentBanks]);

    const handleAddBankSuccess = useCallback(() => {
        setShowAddBankAccountPopup(false);
        loadBankAccounts();
    }, [loadBankAccounts]);

    const handleCloseModal = useCallback(() => {
        setShowAddBankAccountPopup(false);
    }, []);

    const handleAddBankClick = useCallback(() => {
        setEditingRow(null);
        setShowAddBankAccountPopup(true);
    }, []);

    // Memoized table rows
    const tableRows = useMemo(() => {
        return bankAccounts.map((account, index) => {
            const isEditing = editingRowId === account.memberBankAccountID;

            return (
                <TableRow
                    key={account.memberBankAccountID}
                    account={account}
                    index={index}
                    isEditing={isEditing}
                    editingRow={editingRow}
                    agentBankOptions={agentBankOptions}
                    onEdit={handleEditBankAccount}
                    onDelete={handleDeleteBankAccount}
                    onSave={handleSaveEdit}
                    onCancel={handleCancelEdit}
                    onAgentBankChange={handleAgentBankChange}
                    isLoading={isLoading}
                />
            );
        });
    }, [
        bankAccounts,
        editingRowId,
        editingRow,
        agentBankOptions,
        handleEditBankAccount,
        handleDeleteBankAccount,
        handleSaveEdit,
        handleCancelEdit,
        handleAgentBankChange,
        isLoading
    ]);

    return (
        <>
            <div className="flex gap-[8px] mb-[10px]">
                <ButtonFull
                    className="w-[150px] [&>*]:w-[100%]"
                    onClick={handleAddBankClick}
                    disabled={isLoading}
                    text={t('AddBankAccount')}
                />
            </div>
            <TableSkeleton>
                <thead>
                    <tr>
                        <th scope="col">#</th>
                        <th scope="col">{t('MemberBankAccount')}</th>
                        <th scope="col">{t('AgentBankAccount')}</th>
                        <th scope="col">{t('Action')}</th>
                    </tr>
                </thead>
                <tbody> 
                    <tr>
                        <td colSpan={4} className="p-3 text-center">
                            <div className="flex justify-center items-center flex-col py-[20px]" role="alert">
                                <NoDataIcon />
                                <span>{t('noData') || 'No data'}</span>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </TableSkeleton>
            {/* {showAddBankAccountPopup && (
                <AddBankModal
                    closeModal={handleCloseModal}
                    onAddSuccess={handleAddBankSuccess}
                />
            )}
            {showDeleteConfirm && (
                <ConfirmPopup
                    title={t('updateInfoCashMember.deleteBankAccount')}
                    message={
                        <>
                            <div className="text-center">
                                {t('updateInfoCashMember.areYouSureYouWantToDeleteThisBankAccount', { bankNumber: bankAccounts.find(account => account.memberBankAccountID === deleteAccountId)?.memberBankAccountNumber })}
                            </div>
                            <div className="text-center text-[12px]">
                                *{t('updateInfoCashMember.thisActionIsPermanentAndCannotBeUndone')}
                            </div>
                        </>
                    }
                    onCancel={handleCancelDelete}
                    onConfirm={handleConfirmDelete}
                />
            )} */}
        </>
    );
};


