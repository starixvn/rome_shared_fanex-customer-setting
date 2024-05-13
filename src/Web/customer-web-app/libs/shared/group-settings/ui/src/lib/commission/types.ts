import { CommissionGroupItem } from '@customer-web-app/shared-group-settings-types';

export interface Props {
  isReadOnly: boolean;
  commissions: CommissionGroupItem[];
  handleCommissionChange: (sportTypeId: number, betTypeId: number, data: Partial<CommissionGroupItem>) => void;
  handleSelectAll: (isChecked: boolean) => void;
}
