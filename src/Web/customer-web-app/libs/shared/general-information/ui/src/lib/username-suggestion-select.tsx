import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { components, InputActionMeta, InputProps, OptionProps, SingleValue, StylesConfig } from 'react-select';
import AsyncSelect from 'react-select/async';

import { UsernameSuggestion } from '@customer-web-app/share-models';
import { SettingService } from '@customer-web-app/shared-group-settings-services';
import { IconCorrect, IconWrong } from '@customer-web-app/shared/base/common-icon';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import styles from './username-suggestion-select.module.scss';

interface UsernameSuggestionOption extends UsernameSuggestion {
  value: string;
}

const promiseOptions = async (inputValue: string): Promise<UsernameSuggestionOption[]> => {
  if (inputValue.length > 3) {
    const usernames = await SettingService.GetUserNameSuggestion(inputValue);
    return usernames.map((x) => ({ ...x, value: x.UserName }));
  }

  return [];
};

const Option = (props: OptionProps<UsernameSuggestionOption, false>) => {
  return (
    <components.Option {...props}>
      {props.data.UserName ? (
        <div>
          <div>
            {props.data.UserName} {props.data.UserName2 && `(${props.data.UserName2})`}
          </div>
          <div>
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
    style={{ minWidth: '250px', height: '26px', marginBottom: '2px', border: 'none' }}
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
    '&:hover': {
      outline: 'none',
    },
  }),

  menu: (provided) => ({
    ...provided,
    zIndex: 2,
    borderRadius: 'none',
    marginTop: '1px',
  }),

  input: (provided) => ({
    ...provided,
    padding: 0,
    margin: 0,
    paddingLeft: '5px',
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
  onSelectDirectMember: (custId: number) => void;
}

export default function UsernameSuggestionSelect({ onSelectDirectMember }: Props) {
  const { t } = useTranslation();
  const [value, setValue] = useState('');
  const [validateResult, setValidateResult] = useState<{ [key: string]: boolean }>({});

  const handleCheckIsDirectMember = async (username: string) => {
    const custId = await SettingService.GetDirectMember(username);
    setValidateResult({ username: custId > 0 });
    if (custId) {
      onSelectDirectMember(custId);
    }
  };

  const handlerChange = async (newValue: SingleValue<UsernameSuggestionOption>) => {
    if (newValue) {
      setValue(newValue.value);
      newValue.value.length > 3 && (await handleCheckIsDirectMember(newValue.value));
    } else {
      setValidateResult({});
    }
  };

  const handleInputChange = async (newValue: string, { action }: InputActionMeta) => {
    if (action === 'input-change') {
      setValue(newValue);
      newValue.length > 3 && (await handleCheckIsDirectMember(newValue));
    }
  };

  const handleBlur = async () => {
    if (value) {
      await handleCheckIsDirectMember(value);
    } else {
      setValidateResult({});
    }
  };

  return (
    <div className={styles.search}>
      <AsyncSelect
        id="sourceUsername"
        inputValue={value}
        loadOptions={promiseOptions}
        components={{ Option, Input, SingleValue: () => null, Placeholder: () => null }}
        styles={customStyles}
        placeholder={t('UsernamePlaceHolder')}
        noOptionsMessage={() => null}
        controlShouldRenderValue={false}
        onChange={handlerChange}
        onInputChange={handleInputChange}
        onBlur={handleBlur}
      />
      {validateResult['username'] !== undefined && (
        <div>{validateResult['username'] ? <IconCorrect /> : <IconWrong />}</div>
      )}
      <WrapperTooltip
        type="error"
        anchorSelect={`#sourceUsername`}
        isOpen={validateResult['username'] !== undefined && validateResult['username'] === false}
        content={t('UserNotExist')}
      />
    </div>
  );
}
