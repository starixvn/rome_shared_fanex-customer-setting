import React, { forwardRef, useImperativeHandle, useRef } from 'react';
import { useTranslation } from 'react-i18next';

import { ValidationRefType } from '@customer-web-app/share-models';
import { GroupTypeIds, ProductGroupIds, UserLevel } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { Constants, GroupType } from '@customer-web-app/shared-group-settings-types';
import { IconSaveSingle } from '@customer-web-app/shared/base/common-icon';

import BetSettingNote from './bet-setting/bet-setting-note';
import SportsBookBetSetting from './bet-setting/sportsbook';
import SportsBookCommission from './commission/sportsbook';
import ParlayMaxPayout from './parlay-max-payout';
import AgentPositionTaking from './position-taking/agent-position-taking';
import MasterEditMemberPositionTaking from './position-taking/master-edit-member-position-taking';
import MasterPositionTaking from './position-taking/master-position-taking';
import MemberPositionTaking from './position-taking/member-position-taking';
import PositionTakingNote from './position-taking/position-taking-note';
import SuperEditMemberPositionTaking from './position-taking/super-edit-member-position-taking';
import SuperPositionTaking from './position-taking/super-position-taking';

type Props = {
  isEditMode?: boolean;
  updateGroupSetting?: (groupTypeId: number) => void;
};

export const GroupSetting = forwardRef<ValidationRefType, Props>(function GroupSetting(props, ref) {
  const { t } = useTranslation();
  const { productGroups, generalInfo } = useGroupSettingContext();
  const productGroup = productGroups?.find((x) => x.id === ProductGroupIds.SportsBook);

  const betSettingRef = useRef<ValidationRefType>(null);
  const positionTakingRef = useRef<ValidationRefType>(null);
  const parlayMaxPayoutRef = useRef<ValidationRefType>(null);

  const refDetails = [
    { ref: betSettingRef, id: GroupTypeIds.SportsBookBetSetting },
    { ref: parlayMaxPayoutRef, id: GroupTypeIds.ParlayMaxPayout },
    { ref: positionTakingRef, id: GroupTypeIds.PositionTaking },
  ];

  const scrollToBlock = (id?: number) => {
    for (const blockRef of refDetails) {
      if (blockRef.ref?.current?.scrollToBlock && id === blockRef.id) {
        blockRef.ref.current?.scrollToBlock();
        break;
      }
    }
  };

  const validateGroupSetting = (groupTypeId?: number) => {
    return refDetails.map((blockRef) => {
      if (groupTypeId === undefined || groupTypeId === blockRef.id) {
        const isValidGroup = blockRef.ref?.current?.validate ? blockRef.ref.current.validate() : true;
        return {
          isValid: isValidGroup,
          groupTypeId: isValidGroup ? undefined : blockRef.id,
        };
      }

      return {
        isValid: true,
      };
    });
  };

  const validate = (groupTypeId?: number) => {
    const groupSettingValidateResult = validateGroupSetting(groupTypeId);
    if (groupTypeId) {
      const validateGroup = groupSettingValidateResult.find((item) => item.groupTypeId === groupTypeId);
      if (validateGroup) {
        return {
          isValid: !!validateGroup?.isValid,
          groupTypeId: groupTypeId,
        };
      }
    }

    const firstInValidItem = groupSettingValidateResult.find((item) => item.groupTypeId !== undefined);
    return {
      isValid: !firstInValidItem,
      groupTypeId: firstInValidItem && firstInValidItem.groupTypeId,
    };
  };

  useImperativeHandle(ref, () => ({
    validate,
    scrollToBlock,
  }));

  const renderBlockTitle = (title: React.ReactNode, groupTypeId: number, isEditMode: boolean) => {
    return (
      <div style={{ display: 'flex', justifyContent: 'space-between', width: '100%', alignItems: 'center' }}>
        <div>{title}</div>
        {isEditMode && (
          <div
            onClick={(e) => {
              e.stopPropagation();
              props.updateGroupSetting?.(groupTypeId);
            }}
            aria-hidden="true"
          >
            {IconSaveSingle}
          </div>
        )}
      </div>
    );
  };

  const renderPTBlockTitle = () => {
    return renderBlockTitle(
      <>
        {t('PositionTaking')}
        <PositionTakingNote generalInfo={generalInfo} />
      </>,
      Constants.GroupTypeIds.PositionTaking,
      !!props.isEditMode
    );
  };

  const renderBetSettingTitle = (isReadOnly: boolean) => {
    if (isReadOnly) {
      return (
        <>
          {t('BetSetting')}
          <BetSettingNote generalInfo={generalInfo} />
        </>
      );
    }

    return renderBlockTitle(
      <>
        {t('BetSetting')}
        <BetSettingNote generalInfo={generalInfo} />
      </>,
      Constants.GroupTypeIds.SportsBookBetSetting,
      !!props.isEditMode
    );
  };

  const renderGroupTypeSetting = (groupType: GroupType) => {
    const { id, isReadOnly } = groupType;
    switch (id) {
      case Constants.GroupTypeIds.SportsBookCommission:
        return (
          <SportsBookCommission
            isReadOnly={isReadOnly}
            title={
              isReadOnly
                ? t('Commission')
                : renderBlockTitle(t('Commission'), Constants.GroupTypeIds.SportsBookCommission, !!props.isEditMode)
            }
          />
        );
      case Constants.GroupTypeIds.SportsBookBetSetting:
        return (
          <SportsBookBetSetting isReadOnly={isReadOnly} ref={betSettingRef} title={renderBetSettingTitle(isReadOnly)} />
        );
      case Constants.GroupTypeIds.ParlayMaxPayout:
        return (
          <ParlayMaxPayout
            isReadOnly={isReadOnly}
            ref={parlayMaxPayoutRef}
            title={
              isReadOnly
                ? t('ParlayMaxPayout')
                : renderBlockTitle(t('ParlayMaxPayout'), Constants.GroupTypeIds.ParlayMaxPayout, !!props.isEditMode)
            }
          />
        );
      case Constants.GroupTypeIds.PositionTaking:
        if (params.uplineRoleId === UserLevel.Super && params.roleId === UserLevel.Member)
          return (
            <SuperEditMemberPositionTaking
              ref={positionTakingRef}
              isReadOnly={isReadOnly}
              title={renderPTBlockTitle()}
            />
          );
        if (params.uplineRoleId === UserLevel.Master && params.roleId === UserLevel.Member)
          return (
            <MasterEditMemberPositionTaking
              ref={positionTakingRef}
              isReadOnly={isReadOnly}
              title={renderPTBlockTitle()}
              isDisableMasterAutoPT={generalInfo?.disableAutoMPT}
            />
          );
        if (params.uplineRoleId === UserLevel.Agent)
          return <MemberPositionTaking ref={positionTakingRef} isReadOnly={isReadOnly} title={renderPTBlockTitle()} />;
        if (params.uplineRoleId === UserLevel.Master)
          return (
            <AgentPositionTaking
              ref={positionTakingRef}
              isReadOnly={isReadOnly}
              isDisableMasterAutoPT={generalInfo?.disableAutoMPT}
              title={renderPTBlockTitle()}
            />
          );
        if (params.uplineRoleId === UserLevel.Super)
          return <MasterPositionTaking ref={positionTakingRef} isReadOnly={isReadOnly} title={renderPTBlockTitle()} />;
        return <SuperPositionTaking isReadOnly={isReadOnly} title={renderPTBlockTitle()} />;
      default:
        return null;
    }
  };

  return productGroup?.groupTypes?.map((groupType) => (
    <React.Fragment key={groupType.id}>{renderGroupTypeSetting(groupType)}</React.Fragment>
  ));
});

export default GroupSetting;
