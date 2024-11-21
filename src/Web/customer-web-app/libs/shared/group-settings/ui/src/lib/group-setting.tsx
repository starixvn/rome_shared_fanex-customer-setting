import React, { forwardRef, useImperativeHandle, useRef } from 'react';

import { ValidationRefType } from '@customer-web-app/share-models';
import { GroupTypeIds, UserLevel } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { GroupType } from '@customer-web-app/shared-group-settings-types';

import BetLimit from './bet-limit';
import RNGGamesBetSetting from './bet-setting/rnggames';
import SportsBookBetSetting from './bet-setting/sportsbook';
import Commission from './commission';
import ParlayMaxPayout from './parlay-max-payout';
import PositionTaking from './position-taking';
import DirectMemberPositionTaking from './position-taking/direct-member';
import ProductStatus from './product-status';
import WinLossLimit from './win-loss-limit';
import { AppConfigHelper } from '@customer-web-app/shared-helpers';

export const GroupSetting = forwardRef<ValidationRefType>(function GroupSetting(_, ref) {
  const { productGroups, productGroupId, roleId } = useGroupSettingContext();
  const productGroup = productGroups?.find((x) => x.id === productGroupId);

  const betSettingRef = useRef<ValidationRefType>(null);
  const positionTakingRef = useRef<ValidationRefType>(null);
  const parlayMaxPayoutRef = useRef<ValidationRefType>(null);
  const winLossLimitRef = useRef<ValidationRefType>(null);
  const betLimitRef = useRef<ValidationRefType>(null);

  const refDetails = [
    { ref: betSettingRef, id: GroupTypeIds.SportsBookBetSetting },
    { ref: betSettingRef, id: GroupTypeIds.RNGGamesBetSetting },
    { ref: parlayMaxPayoutRef, id: GroupTypeIds.ParlayMaxPayout },
    { ref: positionTakingRef, id: GroupTypeIds.PositionTaking },
    { ref: winLossLimitRef, id: GroupTypeIds.WinLossLimit },
    { ref: betLimitRef, id: GroupTypeIds.BetLimit },
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

  const renderGroupTypeSetting = (groupType: GroupType) => {
    const { id, isReadOnly, isAllowUncheckAll, groupItems } = groupType;

    if(roleId === UserLevel.Super && AppConfigHelper.isNapSystem()) {
      switch (id) {
        case GroupTypeIds.SportsBookBetSetting:
          return <SportsBookBetSetting isReadOnly={isReadOnly} ref={betSettingRef} />;
        case GroupTypeIds.RNGGamesBetSetting:
          return <RNGGamesBetSetting isReadOnly={isReadOnly} ref={betSettingRef} />;
        case GroupTypeIds.ParlayMaxPayout:
          return <ParlayMaxPayout isReadOnly={isReadOnly} ref={parlayMaxPayoutRef} />;
        case GroupTypeIds.BetLimit:
          return (
            groupItems.length > 0 && (
              <BetLimit isReadOnly={isReadOnly} ref={betLimitRef} isAllowUncheckAll={isAllowUncheckAll} />
            )
          );
        default:
          return null;
      }
    }
    
    switch (id) {
      case GroupTypeIds.ProductStatus:
        return <ProductStatus isReadOnly={isReadOnly} />;
      case GroupTypeIds.SportsBookCommission:
      case GroupTypeIds.Commission:
        return <Commission groupTypeId={id} isReadOnly={isReadOnly} />;
      case GroupTypeIds.SportsBookBetSetting:
        return <SportsBookBetSetting isReadOnly={isReadOnly} ref={betSettingRef} />;
      case GroupTypeIds.RNGGamesBetSetting:
        return <RNGGamesBetSetting isReadOnly={isReadOnly} ref={betSettingRef} />;
      case GroupTypeIds.ParlayMaxPayout:
        return <ParlayMaxPayout isReadOnly={isReadOnly} ref={parlayMaxPayoutRef} />;
      case GroupTypeIds.PositionTaking:
        return params.isDirectMember ? (
          <DirectMemberPositionTaking isReadOnly={isReadOnly} ref={positionTakingRef} />
        ) : (
          <PositionTaking isReadOnly={isReadOnly} ref={positionTakingRef} />
        );
      case GroupTypeIds.BetLimit:
        return (
          groupItems.length > 0 && (
            <BetLimit isReadOnly={isReadOnly} ref={betLimitRef} isAllowUncheckAll={isAllowUncheckAll} />
          )
        );
      case GroupTypeIds.WinLossLimit:
        return <WinLossLimit isReadOnly={isReadOnly} ref={winLossLimitRef} />;
      default:
        return null;
    }
  };

  return productGroup?.groupTypes?.map((groupType) => (
    <React.Fragment key={groupType.id}>{renderGroupTypeSetting(groupType)}</React.Fragment>
  ));
});

export default GroupSetting;
