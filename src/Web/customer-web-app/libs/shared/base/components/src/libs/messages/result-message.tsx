import { IconCheck, IconError2, IconWarning } from "@customer-web-app/shared/base/common-icon";
import styles from './result-message.module.scss';
import classNames from "classnames";

type MessageType = 'success' | 'error' | 'warning' | 'info';

interface ResultMessageProps {
    type: MessageType;
    message: string;
    className?: string; 
    style?: React.CSSProperties;
}

const MESSAGE_CONFIG = {
    error: {
        icon: IconError2,
        styleClass: styles.error,
    },
    warning: {
        icon: IconWarning,
        styleClass: styles.warning,
    },
    success: {
        icon: IconCheck,
        styleClass: styles.success,
    },
} as const;

export const ResultMessage: React.FC<ResultMessageProps> = ({
    type,
    message,
    className = '',
    style,
}) => {
    const config = MESSAGE_CONFIG[type as keyof typeof MESSAGE_CONFIG];
    
    if (!config) {
        return null;
    }

    const IconComponent = config.icon;

    return (
        <div className={classNames(styles.resultMessage, className)} role="alert" style={style}>
            <div className={`${styles.message} ${config.styleClass}`}>
                <IconComponent className={styles.icon} />
                <div>{message}</div>
            </div>
        </div>
    );
}; 