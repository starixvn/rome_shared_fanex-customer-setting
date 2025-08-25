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
  readonly label?: string;
  readonly description?: string;
  readonly component?: string;
  readonly required?: boolean;
  readonly maxInputLength?: number;
  readonly onFocus: () => void;
  readonly onBlur: () => void;
  readonly isError?: boolean;
}


function CashUserInfo({ generalInfo, handleGeneralInfoChange, setCollapse, collapse, isReadOnly = false, userInformationFields }: UserInfoProps) {
  const { t } = useTranslation();
  const tooltipContent = collapse ? t('ClickToExpand') : t('ClickToCollapse');

  const renderField = (config: FieldConfig) => {
    const { field, component, isError, onFocus, onBlur, maxInputLength, description, label } = config;

    if (isReadOnly) {
      if (field === 'remark') {
        return (
          <span className={styles['readOny-user-info']}>
            {generalInfo[field] ? (
              <div className={styles['remark-readonly']}>{generalInfo[field]}</div>
            ) : (
              ''
            )}
          </span>
        );
      }
      return <span className={styles['readOny-user-info']}>{generalInfo[field] ?? ''}</span>;
    }

    if (field === 'remark') {
      return (
        <textarea
          className={styles['remark-textarea']}
          maxLength={500}
          rows={3}
          value={generalInfo[field] || ''}
          onChange={(e) => handleGeneralInfoChange(field, e.target.value)}
          placeholder={t('enterRemark')}
          onFocus={onFocus}
          onBlur={onBlur}
        />
      );
    }

    switch (component) {
      case 'phoneWithCountryCode':
        return (
          <div className={styles['mobile-phone-container']}>
            <Mobifone
              id={field}
              isError={isError}
              phoneNumber={generalInfo[field] ?? ''}
              onChangePhoneNumber={(phoneNumber) => handleGeneralInfoChange(field, phoneNumber)}
              onFocus={onFocus}
              onBlur={onBlur}
              isLoadDefaultCountryCode={true}
            />
            {description && <span className={styles['description']}>{description}</span>}
          </div>
        );
      case 'phoneWithCountryCodeModeEdit':
        return (
          <div className={styles['mobile-phone-container']}>
            <Mobifone
              id={field}
              isError={isError}
              phoneNumber={generalInfo[field] ?? ''}
              onChangePhoneNumber={(phoneNumber) => handleGeneralInfoChange(field, phoneNumber)}
              onFocus={onFocus}
              onBlur={onBlur}
              isLoadDefaultCountryCode={false}
            />
            {description && <span className={styles['description']}>{description}</span>}
          </div>
        );
      default:
        return (
          <>
           <input
            onFocus={onFocus}
            onBlur={onBlur}
            className={classNames({ error: isError })}
            id={field}
            type="text"
            maxLength={maxInputLength}
            value={generalInfo[field] ?? ''}
            onChange={(e) => handleGeneralInfoChange(field, e.target.value)}
          />
          {description && <span className={styles['description']}>{description}</span>}
          </>
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
          <BlockItem key={config.field} label={
            <div>
              {config.label ?? t(config.field)}
              {config.required && <span className="require">*</span>}
            </div>
          }>
            {renderField(config)}
          </BlockItem>
        ))}
      </div>
      
      <div className={classNames(styles['row'], { toggle: collapse })}>
        {!collapse && (
          <div className={styles['remark-item']}>
            <BlockItem label={t('remark')}>
              {isReadOnly ? (
                <span className={styles['readOny-user-info']}>
                  {generalInfo.remark ? (
                    <div className={styles['remark-readonly']}>{generalInfo.remark}</div>
                  ) : (
                    ''
                  )}
                </span>
              ) : (
                <textarea
                  className={styles['remark-textarea']}
                  maxLength={500}
                  rows={3}
                  value={generalInfo.remark || ''}
                  onChange={(e) => handleGeneralInfoChange('remark', e.target.value)}
                  placeholder={t('enterRemark')}
                />
              )}
            </BlockItem>
          </div>
        )}
      </div>
    </div>
  );
}
export default CashUserInfo;
