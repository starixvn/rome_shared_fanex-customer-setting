import { forwardRef } from 'react';
import { useTranslation } from 'react-i18next';

import { ValidationRefType } from '@customer-web-app/share-models';
import { ProductGroupIds, UserLevel } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';

import { GroupSettingBlockProps } from '../types';
import AgentPositionTaking from './agent-position-taking';
import MasterEditMemberPositionTaking from './master-edit-member-position-taking';
import MasterPositionTaking from './master-position-taking';
import MemberPositionTaking from './member-position-taking';
import PositionTakingNote from './position-taking-note';
import SuperEditMemberPositionTaking from './super-edit-member-position-taking';
import SuperPositionTaking from './super-position-taking';

const PositionTaking = forwardRef<ValidationRefType, GroupSettingBlockProps>(function PositionTaking(
  { isReadOnly },
  ref
) {
  const { generalInfo } = useGroupSettingContext();

  if (params.uplineRoleId === UserLevel.Super && params.roleId === UserLevel.Member)
    return <SuperEditMemberPositionTaking ref={ref} isReadOnly={isReadOnly} title={<Title />} />;
  if (params.uplineRoleId === UserLevel.Master && params.roleId === UserLevel.Member)
    return (
      <MasterEditMemberPositionTaking
        ref={ref}
        isReadOnly={isReadOnly}
        title={<Title />}
        isDisableMasterAutoPT={generalInfo?.disableAutoMPT}
      />
    );
  if (params.uplineRoleId === UserLevel.Agent)
    return <MemberPositionTaking ref={ref} isReadOnly={isReadOnly} title={<Title />} />;
  if (params.uplineRoleId === UserLevel.Master)
    return (
      <AgentPositionTaking
        ref={ref}
        isReadOnly={isReadOnly}
        isDisableMasterAutoPT={generalInfo?.disableAutoMPT}
        title={<Title />}
      />
    );
  if (params.uplineRoleId === UserLevel.Super)
    return <MasterPositionTaking ref={ref} isReadOnly={isReadOnly} title={<Title />} />;
  return <SuperPositionTaking isReadOnly={isReadOnly} title={<Title />} />;
});

const Title = () => {
  const { t } = useTranslation();
  const { productGroupId } = useGroupSettingContext();
  return (
    <>
      {t('PositionTaking')}
      {productGroupId === ProductGroupIds.SportsBook && <PositionTakingNote />}
    </>
  );
};

export default PositionTaking;
