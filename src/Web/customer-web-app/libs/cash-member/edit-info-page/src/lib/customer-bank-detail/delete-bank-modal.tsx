import { ConfirmPopup, useResultMessage } from "@customer-web-app/shared/base/components";
import { MemberBankAccountRecord, RemoveMemberBankInfoResponse } from "../edit-info-cash-member-types";
import { useTranslation } from "react-i18next";
import { RemoveMemberBankInfo } from "../edit-info-cash-member-services";
import { useCallback, useState } from "react";

export const DeleteBankModal = ({
    record,
    onDeleteSuccess,
    onCancel,
}: {
    record: MemberBankAccountRecord;
    onDeleteSuccess: () => void;
    onCancel: () => void;
}) => {
    const { t } = useTranslation();
    const { renderResultMessage, showResult } = useResultMessage();
    const [isLoading, setIsLoading] = useState(false);

    const handleConfirmDelete = useCallback(async () => {
        if (!record.memberBankAccountID) return;

        try {
            setIsLoading(true);
            const response = await RemoveMemberBankInfo<RemoveMemberBankInfoResponse>({
                custId: updateInfoCustomerPageConfig.custId,
                memberBankAccountID: record.memberBankAccountID,
            });

            if (response.isSuccessful) {
                showResult('success', t('RemoveSuccess'), 1500, onDeleteSuccess);
            }
            else if (response.code === 203) {
                showResult('error', t('BankAccountNotFound'), 1000, onDeleteSuccess);
            }
            else if (response.message === 'NoPermission') {
                showResult('error', t('YouAreNotAuthorized'));
            }
            else {
                showResult('error', t('RemoveFailed'));
            } 
        } catch (error) {
            console.error('Error deleting bank account:', error);
            showResult('error', t('RemoveFailed'));
        }
        finally {
            setIsLoading(false);
        }
    }, [record.memberBankAccountID, t]);

    return (
        <ConfirmPopup
            size="small"
            title={t('DeleteBankAccount')}
            onConfirm={handleConfirmDelete}
            onCancel={onCancel}
            loading={isLoading}
        >
            <div>
                <div style={{ marginBottom: '10px' }}>
                    {renderResultMessage()}
                </div>
                <div style={{ textAlign: 'center' }}>
                    {t('AreYouSureYouWantToDeleteThisBankAccount')}
                    <span style={{ fontSize: '13px', fontWeight: 'bold', marginLeft: '5px' }}>
                        {record.memberBankAccountNumber}
                    </span>
                </div>
                <div style={{ textAlign: 'center', fontSize: '13px' }}>
                    *{t('ThisActionIsPermanentAndCannotBeUndone')}
                </div>
            </div>
        </ConfirmPopup>
    );
};