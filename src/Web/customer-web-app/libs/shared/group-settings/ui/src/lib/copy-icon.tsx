import classNames from 'classnames';
import { useTranslation } from 'react-i18next';

import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

interface CopyIconProps {
  id: string;
  isCopied?: boolean;
  onClick: () => void;
}

export default function CopyIcon({ id, isCopied = false, onClick }: Readonly<CopyIconProps>) {
  const { t } = useTranslation();

  const iconClass = isCopied ? 'ic-copied' : 'ic-copy';
  return (
    <div>
      <span
        className={classNames('icon', iconClass)}
        aria-hidden
        onClick={onClick}
        style={{ position: 'relative', top: '2px', fontSize: '15px', right: '0', margin: '0' }}
        data-tooltip-id={id}
        data-tooltip-content={t('CopyTooltip')}
      />
      <WrapperTooltip type="default" id={id} place="top" />
    </div>
  );
}
