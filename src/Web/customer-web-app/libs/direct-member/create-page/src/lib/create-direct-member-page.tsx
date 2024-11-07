import { useState } from 'react';

import { BlockUI } from '@customer-web-app/shared-block-ui';
import { Layout, useTheme } from '@customer-web-app/shared/base/layout';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import styles from './create-direct-member-page.module.scss';

/* eslint-disable-next-line */
export interface CreateDirectMemberPageProps {}

export function CreateDirectMemberPage(props: CreateDirectMemberPageProps) {
  const [isLoading, setIsLoading] = useState(true);
  const theme = useTheme();

  return (
    <>
      {isLoading && <BlockUI />}
      <WrapperTooltip id="global-tooltip" className={styles['global-tooltip']} />
      <Layout theme={theme}>
        <div>Direct Member</div>
      </Layout>
    </>
  );
}

export default CreateDirectMemberPage;
