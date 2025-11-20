import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

type Props = {
  displayName: string;
  isReadOnly: boolean;
  isSelected: boolean;
  tooltipMessage?: string;
  sportTypeId: number;
  betTypeId: number;
  handleSelectCommission: (isSelected: boolean) => void;
};

export default function CommissionLabel({ 
  isSelected, 
  handleSelectCommission, 
  displayName, 
  isReadOnly, 
  tooltipMessage,
  sportTypeId,
  betTypeId
}: Readonly<Props>) {
  const { isMultipleMode } = useGroupSettingContext();
  
  return (
    <div>
      {isMultipleMode && !isReadOnly && (
        <input type="checkbox" onChange={(e) => handleSelectCommission(e.target.checked)} checked={isSelected} />
      )}
      {tooltipMessage ? (
        <div style={{ display: 'inline-flex', alignItems: 'center', gap: '8px' }}>
          <span>{displayName}</span>
          <span
            className="ic-info"
            role="img"
            tabIndex={0}
            aria-label="Commission information"
            aria-describedby={`commission-tooltip-${sportTypeId}-${betTypeId}`}
            data-tooltip-id={`commission-tooltip-${sportTypeId}-${betTypeId}`}
            data-tooltip-place="top"
            data-tooltip-content={tooltipMessage}
          ></span>
          <WrapperTooltip type="default" id={`commission-tooltip-${sportTypeId}-${betTypeId}`} place="top" />
        </div>
      ) : (
        <span>{displayName}</span>
      )}
    </div>
  );
}
