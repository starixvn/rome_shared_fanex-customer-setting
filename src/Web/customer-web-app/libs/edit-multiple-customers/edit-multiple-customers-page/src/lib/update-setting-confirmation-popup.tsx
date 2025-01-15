import { useEffect, useState } from 'react';

import { IconClose } from '@customer-web-app/shared/base/common-icon';
import { useTheme } from '@customer-web-app/shared/base/layout';
import NiceModal, { useModal } from '@ebay/nice-modal-react';

import popUpStyles from './popup.module.scss';

const UpdateSettingConfirmationPopup = NiceModal.create(
  ({ productGroups, sportChangedCount, settingChangedCount, custCount, isCopyBetSetting }: any) => {
    const modal = useModal();
    const theme = useTheme();
    const [confirmTimer, setConfirmTimer] = useState(1);

    useEffect(() => {
      if (confirmTimer === 0) {
        return;
      }

      const timer = setTimeout(() => {
        setConfirmTimer(confirmTimer - 1);
      }, 1000);

      return () => {
        clearTimeout(timer);
      };
    }, [confirmTimer]);

    return (
      <div className={popUpStyles.overlay} data-theme={theme}>
        <div className={`${popUpStyles.wrapper} ${popUpStyles.wrappermultiple}`}>
          <div className={popUpStyles.content}>
            <div className={popUpStyles.title}>
              {isCopyBetSetting ? 'Copy' : 'Edit'} {productGroups[0].name} Bet Setting Confirmation
              <span
                onClick={() => {
                  modal.resolve({
                    isCancel: true,
                  });
                  modal.remove();
                }}
                className={popUpStyles.close}
                aria-hidden
              >
                {IconClose}
              </span>
            </div>
            <p>Please review and confirm your changes in the summary below:</p>
            <ul className="ul-changes">
              <li>Sports to be updated: {sportChangedCount}</li>
              <li>Settings Values to be updated: {settingChangedCount}</li>
              <li>Supers to be updated: {custCount}</li>
            </ul>
            <p className={popUpStyles.note}>Note: Decreasing the bet setting may adjust the downline's bet setting.</p>
            <p>Proceed with these updates?</p>
            <div className={popUpStyles.button}>
              <button
                className={popUpStyles['btn-cancel']}
                style={{ display: 'block' }}
                onClick={() => {
                  modal.resolve({
                    isCancel: true,
                  });
                  modal.remove();
                }}
              >
                Cancel
              </button>
              <button
                className={popUpStyles['btn-confirm']}
                style={{ display: 'block' }}
                onClick={() => {
                  if (confirmTimer == 0) {
                    modal.resolve({
                      isCancel: false,
                    });
                    modal.remove();
                  }
                }}
              >
                Confirm({confirmTimer})
              </button>
            </div>
          </div>
        </div>
      </div>
    );
  }
);

export default UpdateSettingConfirmationPopup;
