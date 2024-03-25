import classNames from 'classnames';

import { SharedBaseCharactersSelector } from '@customer-web-app/shared/base/characters-selector';
import { IconCorrect, IconWrong } from '@customer-web-app/shared/base/common-icon';

import styles from './username-field.module.scss';

interface IProps {
  newUserName: string;
  uplineUserName: string;
  onChangeUserName: (newValue: string) => void;
  error?: boolean;
}

export default function Username({ newUserName = '', uplineUserName = '', onChangeUserName, error }: IProps) {
  const buildUserName = (newChar: string, index: number) => {
    const newValue = newUserName.split('');
    newValue[index] = newChar;
    onChangeUserName(`${uplineUserName}${newValue.join('')}`);
  };

  return (
    <div id="userName" className={classNames(styles.username, { [styles.error]: error })}>
      <div className={styles.name}>{uplineUserName.toUpperCase()}</div>
      {newUserName?.split('')?.map((value, index) => (
        <SharedBaseCharactersSelector
          key={value + index.toString()}
          className={classNames({ [styles.error]: error })}
          onSelect={(e) => buildUserName(e, index)}
          value={value}
        />
      ))}
      <span>{error ? <IconWrong /> : <IconCorrect />}</span>
    </div>
  );
}
