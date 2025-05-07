import { GeneralInfo, UserInformation } from "@customer-web-app/shared-general-information-state";
import { useTranslation } from "react-i18next";
import styles from "./general-information.module.scss";
import classNames from "classnames";
import { BlockItem } from "@customer-web-app/shared/base/block";
import { WrapperTooltip } from "@customer-web-app/wrapper-tooltip";
import { Mobifone } from './mobifone';

type GeneralInfoProps = keyof GeneralInfo;
interface UserInfoProps {
  readonly generalInfo: GeneralInfo;
  readonly handleGeneralInfoChange: (fieldName: GeneralInfoProps, value: string) => void;
  readonly setCollapse: (collapse: boolean) => void;
  readonly collapse: boolean;
  readonly isReadOnly?: boolean;
  readonly userInformationFields: ReadonlyArray<FieldConfig>;
}

interface FieldConfig {
  readonly field: keyof UserInformation;
  readonly component?: string;
}


function CashUserInfo({ generalInfo, handleGeneralInfoChange, setCollapse, collapse, isReadOnly = false, userInformationFields }: UserInfoProps) {
  const { t } = useTranslation();
  const tooltipContent = collapse ? t('ClickToExpand') : t('ClickToCollapse');

  const renderField = (config: FieldConfig) => {
    const { field, component } = config;

    if (isReadOnly) {
      return <span className={styles['readOny-user-info']}>{generalInfo[field] || ''}</span>;
    }

    switch (component) {
      case 'phoneWithCountryCode':
        return (
          <div className={styles['mobile-phone-container']}>
            <Mobifone
              phoneNumber={generalInfo.phone || ''}
              onChangePhoneNumber={(phoneNumber) => handleGeneralInfoChange('phone', phoneNumber)}
            />
          </div>
        );
      default:
        return (
          <input
            type="text"
            maxLength={50}
            value={generalInfo[field] || ''}
            onChange={(e) => handleGeneralInfoChange(field, e.target.value)}
          />
        );
    }
  };

  return (
    <div className={styles['user-info']}>
      <div className={styles['subheader']} onClick={() => setCollapse(!collapse)}>
        <span
          className={classNames('ic-arrow-down', { toggle: collapse })}
          data-tooltip-id="userInfo"
          data-tooltip-content={tooltipContent}
        ></span>
        {t('userInformation')}
      </div>
      <WrapperTooltip className="global-tooltip" id="userInfo" />
      <div className={classNames(styles['row'], { toggle: collapse })}>
        {!collapse && userInformationFields.map((config) => (
          <BlockItem key={config.field} label={t(config.field)}>
            {renderField(config)}
          </BlockItem>
        ))}
      </div>
    </div>
  );
}
export default CashUserInfo;
