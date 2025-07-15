import { useState, useCallback } from 'react';
import { useTranslation } from 'react-i18next';
import { MemberBankAccountRecord } from '../edit-info-cash-member-types';
import { RemoveMemberBankInfo } from '../edit-info-cash-member-services';
import { useResultMessage } from '@customer-web-app/shared/base/components';

interface UseDeleteBankAccountProps {
    custId: number;
    bankAccounts: MemberBankAccountRecord[];
    setBankAccounts: React.Dispatch<React.SetStateAction<MemberBankAccountRecord[]>>;
    loadBankAccounts: () => Promise<void>;
    setIsLoading: (loading: boolean) => void;
}

export const useDeleteBankAccount = ({
    custId,
    bankAccounts,
    setBankAccounts,
    loadBankAccounts,
    setIsLoading
}: UseDeleteBankAccountProps) => {
    const { t } = useTranslation();
    const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
    const [deleteAccountId, setDeleteAccountId] = useState<number | null>(null);
    const { showResult } = useResultMessage();

    const handleConfirmDelete = useCallback(async () => {
        if (!deleteAccountId) return;

        try {
            setIsLoading(true);
            const response = await RemoveMemberBankInfo<any>({
                custId,
                memberBankAccountID: deleteAccountId,
            });
            
            setBankAccounts(prev => prev.filter(account => account.memberBankAccountID !== deleteAccountId));
            
            if (response.isSuccessful) {
                showResult('success', t('RemoveSuccess'));
            } else {
                showResult('error', response.message);
            }
        } catch (error) {
            console.error('Error deleting bank account:', error);
            showResult('error', t('RemoveFailed'));
            loadBankAccounts();
        } finally {
            setIsLoading(false);
            setShowDeleteConfirm(false);
            setDeleteAccountId(null);
        }
    }, [deleteAccountId, custId, setBankAccounts, showResult, t, setIsLoading, loadBankAccounts]);

    const handleCancelDelete = useCallback(() => {
        setShowDeleteConfirm(false);
        setDeleteAccountId(null);
    }, []);

    const handleDeleteBankAccount = useCallback((id: number) => {
        setDeleteAccountId(id);
        setShowDeleteConfirm(true);
    }, []);

    const getAccountToDelete = useCallback(() => {
        return bankAccounts.find((record: MemberBankAccountRecord) => 
            record.memberBankAccountID === deleteAccountId
        );
    }, [bankAccounts, deleteAccountId]);

    return {
        showDeleteConfirm,
        deleteAccountId,
        handleConfirmDelete,
        handleCancelDelete,
        handleDeleteBankAccount,
        getAccountToDelete
    };
}; 