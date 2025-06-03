import { useTranslation } from 'react-i18next';
import { components, InputActionMeta, InputProps, OptionProps, SingleValue, StylesConfig } from 'react-select';
import AsyncSelect from 'react-select/async';

import { UsernameSuggestion } from '@customer-web-app/share-models';
import { SettingService } from '@customer-web-app/shared-group-settings-services';

import styles from './username-suggestion-select.module.scss';

export interface UsernameSuggestionOption extends UsernameSuggestion {
  value: string;
}

export const getDirectMemberPromiseOptions = async (inputValue: string): Promise<UsernameSuggestionOption[]> => {
  if (inputValue.length >= 3) {
    const usernames = await SettingService.GetUserNameSuggestion(inputValue);
    return usernames.map((x) => ({ ...x, value: x.UserName }));
  }

  return [];
};

export const getCashMemberPromiseOptions = async (inputValue: string): Promise<UsernameSuggestionOption[]> => {
  if (inputValue.length >= 3) {
    const usernames = await SettingService.GetCashMemberUserNameSuggestion(inputValue);
    return usernames.map((x) => ({ ...x, value: x.UserName }));
  }

  return [];
};


const Option = (props: OptionProps<UsernameSuggestionOption, false>) => {
  return (
    <components.Option {...props}>
      {props.data.UserName ? (
        <div className={styles['line']}>
          <div className="username">
            {props.data.UserName} {props.data.UserName2 && `(${props.data.UserName2})`}
          </div>
          <div className="fullname">
            {props.data.FirstName} {props.data.LastName}
          </div>
        </div>
      ) : (
        <span>{props.data.value}</span>
      )}
    </components.Option>
  );
};

const Input = (props: InputProps<UsernameSuggestionOption, false>) => (
  <components.Input
    {...props}
    isHidden={false}
    placeholder={props.selectProps.placeholder as string}
    style={{ minWidth: '250px', height: '26px', marginBottom: '2px', border: 'none', paddingLeft: '5px' }}
  />
);

const customStyles: StylesConfig<UsernameSuggestionOption> = {
  valueContainer: (provided) => ({
    ...provided,
    height: '28px',
    padding: 0,
    minWidth: '100%',
  }),
  container: (provided) => ({
    ...provided,
    display: 'inline-block',
    width: '250px',
    height: '28px',
    fontSize: 'var(--font-size)',
    fontWeight: '400',
    lineHeight: '22px',
  }),

  control: (baseStyles) => ({
    ...baseStyles,
    borderRadius: '3px',
    minHeight: '28px',
    height: '28px',
    width: '100%',
    display: 'inline-block',
    border: '1px solid var(--border-input)',
    boxShadow: 'none',
    background: 'none',
    '&:hover': {
      outline: 'none',
    },
  }),

  menu: (provided) => ({
    ...provided,
    zIndex: 9999,
    borderRadius: 'none',
    marginTop: '1px',
    background: 'var(--suggetion-bg)',
    border: 'var(--suggetion-border)',
    boxShadow: 'var(--suggetion-shadown)',
    color: 'var(--suggetion-color)',
  }),
  option: () => ({
    background: 'none',
  }),

  input: (provided) => ({
    ...provided,
    padding: 0,
    margin: 0,
  }),

  singleValue: (provided) => ({
    ...provided,
    padding: 0,
    paddingLeft: '5px',
    margin: 0,
  }),

  placeholder: (provided) => ({
    ...provided,
    padding: 0,
    paddingLeft: '5px',
    margin: 0,
  }),

  indicatorSeparator: () => ({
    display: 'none',
  }),

  indicatorsContainer: (provided) => ({
    ...provided,
    display: 'none',
  }),
};

interface Props {
  readonly id: string;
  readonly value: string;
  readonly promiseOptions: (inputValue: string) => Promise<UsernameSuggestionOption[]>;
  readonly onChange: (newValue: SingleValue<UsernameSuggestionOption>) => void;
  readonly onInputChange: (newValue: string, actionMeta: InputActionMeta) => void;
}

export default function UsernameSuggestionSelect({ id, value, onChange, onInputChange, promiseOptions }: Props) {
  const { t } = useTranslation();
  return (
    <AsyncSelect
      id={id}
      inputValue={value}
      loadOptions={promiseOptions}
      components={{ Option, Input, SingleValue: () => null, Placeholder: () => null }}
      styles={customStyles}
      placeholder={t('UsernamePlaceHolder')}
      noOptionsMessage={() => null}
      controlShouldRenderValue={false}
      onChange={onChange}
      onInputChange={onInputChange}
    />
  );
}
