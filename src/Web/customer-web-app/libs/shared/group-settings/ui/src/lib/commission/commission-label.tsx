import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';

type Props = {
  displayName: string;
  isReadOnly: boolean;
  isSelected: boolean;
  handleSelectCommission: (isSelected: boolean) => void;
};

export default function CommissionLabel({ isSelected, handleSelectCommission, displayName, isReadOnly }: Props) {
  const { isMultipleMode } = useGroupSettingContext();
  return (
    <div>
      {isMultipleMode && !isReadOnly && (
        <input type="checkbox" onChange={(e) => handleSelectCommission(e.target.checked)} checked={isSelected} />
      )}
      <span>{displayName}</span>
    </div>
  );
}
