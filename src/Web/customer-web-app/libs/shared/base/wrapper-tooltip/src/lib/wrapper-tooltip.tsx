import classNames from 'classnames';
import { ITooltip, Tooltip } from 'react-tooltip';

import styles from './wrapper-tooltip.module.scss';

type Type = 'error' | 'warning' | 'success' | 'default';

export interface WrapperTooltipProps extends ITooltip {
  type?: Type;
}

export function WrapperTooltip({ children, type, ...rest }: WrapperTooltipProps) {
  const isOpening = type === 'error' && rest.isOpen;
  return (
    <Tooltip className={classNames(styles.tooltip, styles[`${type}`], { errTooltipOpen: isOpening })} {...rest}>
      {children}
    </Tooltip>
  );
}

export default WrapperTooltip;
