import {GeneralInfo, UserInformation} from "@customer-web-app/shared-general-information-state";
import {useTranslation} from "react-i18next";
import styles from "./general-information.module.scss";
import classNames from "classnames";
import {BlockItem} from "@customer-web-app/shared/base/block";
import React from "react";
import {WrapperTooltip} from "@customer-web-app/wrapper-tooltip";

type GeneralInfoProps = keyof GeneralInfo;
interface UserInfoProps {
  generalInfo: GeneralInfo;
  handleGeneralInfoChange: (fieldName: GeneralInfoProps, value: string) => void;
  setCollapse: (collapse: boolean) => void;
  collapse: boolean;
  isReadOnly?: boolean;
}

const UserInformationFields: ReadonlyArray<keyof UserInformation> = [
  'firstName',
  'lastName',
  'phone',
  'mobilePhone',
  'fax',
];

function UserInfo({generalInfo, handleGeneralInfoChange, setCollapse, collapse, isReadOnly=false}: UserInfoProps) {
  const {t} = useTranslation();
  const tooltipContent = collapse ? t('ClickToExpand') : t('ClickToCollapse');

  return (
    <div className={styles['user-info']}>
      <div className={styles['subheader']} onClick={() => setCollapse(!collapse)}>
        <span
          className={classNames('ic-arrow-down', {toggle: collapse})}
          data-tooltip-id="userInfo"
          data-tooltip-content={tooltipContent}
        ></span>
        {t('userInformation')}
      </div>
      <WrapperTooltip className="global-tooltip" id="userInfo" />
      <div className={classNames(styles['row'], { toggle: collapse })}>
        {!collapse && UserInformationFields.map((field) => (
          <BlockItem key={field} label={t(field)}>
            {isReadOnly ?
              <span className={styles['readOny-user-info']}>{generalInfo[field] || ''}</span>
              : <input
                key={field}
                type="text"
                maxLength={50}
                value={generalInfo[field] || ''}
                onChange={(e) => handleGeneralInfoChange(field, e.target.value)}
              />
            }
          </BlockItem>
        ))}
      </div>
    </div>
  );
}
export default UserInfo;
