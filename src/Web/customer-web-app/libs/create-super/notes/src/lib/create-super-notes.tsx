import { Block } from '@customer-web-app/shared/base/block';

import styles from './create-super-note.module.scss';
import HeadlineBelongSites from './headline-belong-sites';
import NotAllowedSuperPrefixes from './note-allowed-super-prefixes';
import { HeadlineBelongSite, SuperPrefixes } from './types';

/* eslint-disable-next-line */
export interface CreateSuperNoteProps {
  belongSites: HeadlineBelongSite[];
  notAllowedPrefixes: SuperPrefixes[];
}

export function CreateSuperNotes({ belongSites, notAllowedPrefixes }: CreateSuperNoteProps) {
  return (
    <Block title="Notes" isPartialUpdate={false} onSaveClick={() => {}}>
      <div className={styles['block-note']}>
        <div className={styles['block-alert']}>
          <HeadlineBelongSites data={belongSites} />
          <NotAllowedSuperPrefixes data={notAllowedPrefixes} />
        </div>
      </div>
    </Block>
  );
}

export default CreateSuperNotes;
