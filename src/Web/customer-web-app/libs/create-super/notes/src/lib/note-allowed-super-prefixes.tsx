import { Fragment } from 'react';

import styles from './create-super-note.module.scss';
import { SuperPrefixes } from './types';

interface NotAllowedSuperPrefixes {
  data: SuperPrefixes[];
}

export default function NotAllowedSuperPrefixes({ data }: NotAllowedSuperPrefixes) {
  if (data && data.length) {
    return (
      <div className={styles['alert-info']}>
        {'For 789y site, the following username prefixes are not allowed for new accounts anymore'}
        {data.map(({ groupName, prefixes }) => (
          <Fragment key={groupName}>
            <br />
            {`- ${groupName}: ${prefixes}`}
          </Fragment>
        ))}
      </div>
    );
  }

  return null;
}
