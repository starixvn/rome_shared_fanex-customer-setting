import { useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { GroupItem } from '@customer-web-app/shared-group-settings-types';
import { BlockColumn } from '@customer-web-app/shared/base/block';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import CopyIcon from '../copy-icon';

export type PTRowProps<T> = {
  item: T;
  label?: string | React.ReactNode;
  isCopy: boolean | undefined;
  isDisableMasterAutoPT?: boolean;
};

export const renderNoneBlockColumn = (key: string, numbers: number) => {
  return [...Array(numbers)].map((i, index) => {
    return <BlockColumn key={`${key}-${index}`}>{''}</BlockColumn>;
  });
};

function CopyPTICon({
  id,
  isCopy,
  isCopySuccess,
  content,
  handleCopyClick,
}: Readonly<{
  id: number;
  isCopy: boolean;
  isCopySuccess: boolean;
  content: string;
  handleCopyClick?: () => void;
}>) {
  const { t } = useTranslation();

  return (
    <span>
      <span data-tooltip-id={id.toString()} data-tooltip-content={t(content)}>
        <CopyIcon
          id="copy-position-taking"
          isCopied={isCopy}
          onClick={() => {
            handleCopyClick?.();
          }}
        />
        {content !== '' && (
          <WrapperTooltip type={isCopySuccess ? 'success' : 'error'} id={id.toString()} isOpen={true} place="bottom" />
        )}
      </span>
    </span>
  );
}

export const renderPTLabel = (
  labelName: string | undefined,
  isMultipleMode: boolean,
  isMultipleModeChecked: boolean,
  id: number,
  isCopy: boolean,
  isCopySuccess: boolean,
  handleCheckMultiple: (isChecked: boolean) => void,
  handleCopyClick?: () => void
) => {
  const getContent = () => {
    if (isCopy === false) return '';

    return isCopySuccess ? 'CopiedSuccessfully' : 'CopyingFailed';
  };

  return (
    <div style={{ display: 'flex', justifyContent: 'space-between', width: '100%', alignItems: 'normal' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        {isMultipleMode && (
          <input
            type="checkbox"
            onChange={(e) => handleCheckMultiple(e.target.checked)}
            checked={isMultipleModeChecked}
          />
        )}
        <span>{labelName}</span>
      </div>
      <CopyPTICon
        id={id}
        isCopy={isCopy}
        isCopySuccess={isCopySuccess}
        content={getContent()}
        handleCopyClick={handleCopyClick}
      />
    </div>
  );
};

export const renderBlockRowTitle = (
  name: string | undefined,
  isMultipleMode: boolean,
  isMultipleModeChecked: boolean,
  onChange: (isChecked: boolean) => void
) => {
  return (
    <div>
      {isMultipleMode && (
        <input type="checkbox" onChange={(e) => onChange(e.target.checked)} checked={isMultipleModeChecked} />
      )}
      <span>{name}</span>
    </div>
  );
};

export type CopiedPTItem = {
  betTypeId: number;
  sportTypeId: number;
  isCopied: boolean | undefined;
};

export const usePTCopy = () => {
  const [stateCopiedGroup, setStateCopiedGroup] = useState<CopiedPTItem[]>([]);
  const [copyIconId, setCopyIconId] = useState<number | undefined>();
  const timeId = useRef<NodeJS.Timeout>();

  return {
    stateCopiedGroup,
    setStateCopiedGroup,
    timeId,
    copyIconId,
    setCopyIconId,
  };
};

export const isCopyingPTItem = (stateCopiedGroup: CopiedPTItem[], item: GroupItem) => {
  return stateCopiedGroup.find((i) => i.betTypeId === item.betTypeId && i.sportTypeId === item.sportTypeId)?.isCopied;
};
