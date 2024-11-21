import { useTranslation } from 'react-i18next';

import { isSportsBookProduct, UserLevel } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import { CommissionGroupItem } from '@customer-web-app/shared-group-settings-types';
import { Block } from '@customer-web-app/shared/base/block';

import { GroupSettingBlockProps } from '../types';
import DefaultCommission from './default';
import { MemberSportsBookCommission } from './sportsbook';
import DirectMemberCommission from './sportsbook/direct-member';

type CommissionBlockProps = {
  groupTypeId: number;
} & GroupSettingBlockProps;

export default function Commission({ isReadOnly, groupTypeId }: Readonly<CommissionBlockProps>) {
  const { t } = useTranslation();
  const {
    productGroups,
    handleSettingItemChange,
    handleUpdateSettingItems,
    productGroupId,
    isEditMode,
    updateGroupSetting,
    handleSelectCommGroup,
  } = useGroupSettingContext();
  const groupId = Number(productGroupId);
  const commissions = getSettingItems<CommissionGroupItem>(productGroups, groupId, groupTypeId);
  const isIndirectUpdate = params.roleId === UserLevel.Member && params.uplineRoleId !== UserLevel.Agent;
  const isMemberSportsBookComm = params.uplineRoleId === UserLevel.Agent && isSportsBookProduct(groupId);

  const handleCommissionChange = (sportTypeId: number, betTypeId: number, data: Partial<CommissionGroupItem>) => {
    handleSettingItemChange?.(groupId, groupTypeId, sportTypeId, betTypeId, data);
  };

  const handleSelectAll = (isChecked: boolean) => {
    const newCommissions = commissions.map((item) => ({ ...item, isMultipleModeChecked: isChecked }));
    handleUpdateSettingItems?.(groupId, groupTypeId, newCommissions);
    if (isMemberSportsBookComm && !isChecked) {
      handleSelectCommGroup?.(false);
    }
  };

  const props = { commissions, handleCommissionChange, isReadOnly, handleSelectAll };


  const renderContent = () => {
    if (isIndirectUpdate || isMemberSportsBookComm) {
      return <MemberSportsBookCommission {...props} />;
    }

    if (params.isDirectMember) {
      return <DirectMemberCommission {...props} />;
    }

    return <DefaultCommission {...props} />;
  };
  return (
    <Block
      title={t('Commission')}
      isPartialUpdate={isEditMode === true && !isReadOnly}
      onSaveClick={() => updateGroupSetting?.(groupTypeId)}
    >
      {renderContent()}
    </Block>
  );
}
