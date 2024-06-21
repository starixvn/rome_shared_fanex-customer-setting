import { useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { GroupItem } from '@customer-web-app/shared-group-settings-types';
import { BlockColumn } from '@customer-web-app/shared/base/block';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import CopyIcon from '../copy-icon';
import { CopyStatus } from './types';

export const renderNoneBlockColumn = (key: string, numbers: number) => {
  return [...Array(numbers)].map((i, index) => {
    return <BlockColumn key={`${key}-${index}`}>{''}</BlockColumn>;
  });
};

type CopyPtIconProp = {
  id: number;
  isCopy: boolean;
  isCopySuccess: boolean;
  handleCopyClick?: () => void;
};

export function CopyPTICon({ id, isCopy, isCopySuccess, handleCopyClick }: Readonly<CopyPtIconProp>) {
  const { t } = useTranslation();

  return (
    <span>
      <span
        data-tooltip-id={id.toString()}
        data-tooltip-content={isCopySuccess ? t('CopiedSuccessfully') : t('CopyingFailed')}
      >
        <CopyIcon
          id="copy-position-taking"
          isCopied={isCopy}
          onClick={() => {
            handleCopyClick?.();
          }}
        />
        {isCopy && (
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
  copyIcon: React.ReactNode,
  handleCheckMultiple: (isChecked: boolean) => void
) => {
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
      {copyIcon}
    </div>
  );
};

export type CopyStatusPT = {
  betTypeId: number;
  sportTypeId: number;
  copyStatus: CopyStatus;
};

export const usePTCopy = () => {
  const [stateCopiedGroup, setStateCopiedGroup] = useState<CopyStatusPT[]>([]);
  const [copyIconId, setCopyIconId] = useState<number | undefined>();
  const timeId = useRef<NodeJS.Timeout>();

  const handleCopy = (newStateCopiedGroup: CopyStatusPT[], iconCopyId: number | undefined) => {
    if (JSON.stringify(newStateCopiedGroup) !== JSON.stringify(stateCopiedGroup) || iconCopyId !== copyIconId) {
      if (timeId) clearTimeout(timeId.current);
      setStateCopiedGroup(newStateCopiedGroup);
      setCopyIconId(iconCopyId);
      timeId.current = setTimeout(() => {
        setStateCopiedGroup([]);
        setCopyIconId(undefined);
      }, 3000);
    }
  };

  return {
    timeId,
    copyIconId,
    stateCopiedGroup,
    setCopyIconId,
    setStateCopiedGroup,
    handleCopy,
  };
};

export const isCopyingPTItem = (stateCopiedGroup: CopyStatusPT[], item: GroupItem) => {
  return (
    stateCopiedGroup.find((i) => i.betTypeId === item.betTypeId && i.sportTypeId === item.sportTypeId)?.copyStatus ??
    CopyStatus.NotCopy
  );
};

export function isCopySuccess(stateCopiedGroup: CopyStatusPT[], sportTypeId?: number) {
  if (stateCopiedGroup.length <= 0) return false;

  if (sportTypeId === undefined) return !stateCopiedGroup.some((i) => i.copyStatus === CopyStatus.Fail);

  return stateCopiedGroup.every((i) =>
    i.sportTypeId === sportTypeId ? i.copyStatus === CopyStatus.Success || i.copyStatus === CopyStatus.NotCopy : true
  );
}

const StringToPreventAutoSortReduce = 'PreventAutoSort';
export const groupSportsBookPTBySportTypeId = (items: GroupItem[]) => {
  return items.reduce((acc: { [key: string]: GroupItem[] }, cur) => {
    const key = `${StringToPreventAutoSortReduce}-${cur.sportTypeId}`;
    acc[key] = acc[key] ?? [];
    acc[key].push(cur);
    return acc;
  }, {});
};
