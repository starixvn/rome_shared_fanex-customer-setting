import { memo } from "react";
import { MemberBankAccountRecord } from "../edit-info-cash-member-types";
import { SingleFilterableDropdown, DropdownOption } from "@customer-web-app/shared/base/components";
import { IconTick, IconClose2, IconPen, IconTrashBin } from "@customer-web-app/shared/base/common-icon";
import { ButtonIcon } from "@customer-web-app/shared/base/components";
import styles from "./grid-components.module.scss";
import { useTranslation } from "react-i18next";

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

const BankCell = memo(({ bankAccountNumber, bankCode }: Partial<Bank>) => {
    const { t } = useTranslation();
    if (!bankAccountNumber || !bankCode) return <div className={styles.noMatchingBankAccount}>{t('NoMatchingBankAccount')}</div>;

    return (
        <div className={styles.bankCell}>
            <img
                src={`${updateInfoCustomerPageConfig.cdnUri}cash-member/create/bank/${bankCode.toUpperCase()}.png`}
                alt={`${bankCode} bank icon`}
                className={styles.bankIcon}
                loading="lazy"
            />
            <span className={styles.bankAccountNumber} title={bankAccountNumber}>{bankAccountNumber}</span>
        </div>
    );
});

BankCell.displayName = 'BankCell';

export const BankAccountRow = memo(({
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
                    className={styles.dropdownFullWidth}
                />
            );
        }

        return (
            <BankCell
                bankAccountNumber={account.agentBankAccountNumber}
                bankCode={account.agentBankCode}
            />
        );
    };

    const handleEditClick = () => onEdit(account);
    const handleDeleteClick = () => onDelete(account.memberBankAccountID);

    const renderActionCell = () => {
        if (isEditing) {
            return (
                <div className={styles.actionButtonsContainer}>
                    <ButtonIcon
                        onClick={onSave}
                        className={styles.actionButton}
                        disabled={isLoading}
                        icon={<IconTick className={styles.saveIcon} />}
                    />
                    <ButtonIcon
                        onClick={onCancel}
                        className={styles.actionButton}
                        disabled={isLoading}
                        icon={<IconClose2 className={styles.cancelIcon} />}
                    />
                </div>
            );
        }

        return (
            <div className={styles.actionButtonsContainer}>
                <ButtonIcon
                    onClick={handleEditClick}
                    disabled={isLoading}
                    icon={<IconPen />}
                />
                <ButtonIcon
                    onClick={handleDeleteClick}
                    disabled={isLoading}
                    icon={<IconTrashBin className={styles.trashIcon} />}
                />
            </div>
        );
    };

    return (
        <tr key={account.memberBankAccountID}>
            <td style={{ maxWidth: '50px' }}>{index + 1}</td>
            <td style={{ maxWidth: '300px' }}>
                <BankCell
                    bankAccountNumber={account.memberBankAccountNumber}
                    bankCode={account.memberBankCode}
                />
            </td>
            <td style={{ maxWidth: '300px' }}>{renderAgentBankCell()}</td>
            <td style={{ maxWidth: '150px' }}>{renderActionCell()}</td>
        </tr>
    );
});

BankAccountRow.displayName = 'BankAccountRow';
