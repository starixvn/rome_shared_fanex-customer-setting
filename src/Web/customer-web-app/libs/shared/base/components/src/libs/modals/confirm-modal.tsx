import { useTranslation } from "react-i18next";
import styles from './confirm-modal.module.scss';
import { Label } from "../labels/label";
import { ButtonIcon, ButtonCancel } from "../buttons";
import { IconClose2 } from "@customer-web-app/shared/base/common-icon";
import { ButtonFull } from "../buttons";

type ModalSize = 'small' | 'medium' | 'large' | 'fullscreen';

export const ConfirmPopup: React.FC<{
    title: string;
    onCancel: () => void;
    onConfirm: () => void;
    children: React.ReactNode;
    loading?: boolean;
    size?: ModalSize;
}> = ({
    title,
    onCancel,
    onConfirm,
    children,
    loading,
    size = 'medium',
  }) => {
    const { t } = useTranslation();
    
    const getModalSizeClass = () => {
      switch (size) {
        case 'small':
          return styles.modalSmall;
        case 'large':
          return styles.modalLarge;
        case 'fullscreen':
          return styles.modalFullscreen;
        case 'medium':
        default:
          return styles.modalMedium;
      }
    };

    return (
      <div className={styles.overlay}>
        <div className={`${styles.modal} ${getModalSizeClass()}`}>
          <div className={styles.header}>
            <Label className={styles.title}>{title}</Label>
            <ButtonIcon
                onClick={onCancel}
                icon={<IconClose2 />}
                className={styles.closeModal}
            />
          </div>
          <div className={styles.content}>{children}</div>
          <div className={styles.footer}>
            <ButtonCancel onClick={onCancel} text={t('Cancel')} />
            <ButtonFull onClick={onConfirm} text={t('Confirm')} disabled={loading} />
          </div>
        </div>
      </div>
    );
  };