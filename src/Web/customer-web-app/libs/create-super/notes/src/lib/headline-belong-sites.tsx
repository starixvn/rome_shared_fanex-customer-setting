import { Fragment } from 'react';

import styles from './create-super-note.module.scss';
import { HeadlineBelongSite } from './types';

interface HeadlineBelongSitesProps {
  data: HeadlineBelongSite[];
}

export default function HeadlineBelongSites({ data }: HeadlineBelongSitesProps) {
  if (data && data.length) {
    return (
      <div className={styles['alert-info']}>
        {'For 789y site group, the following headlines belong to site:'}
        {data.map(({ site, headline }) => (
          <Fragment key={site}>
            <br />
            {`- ${site}: ${headline}`}
          </Fragment>
        ))}
      </div>
    );
  }

  return null;
}
