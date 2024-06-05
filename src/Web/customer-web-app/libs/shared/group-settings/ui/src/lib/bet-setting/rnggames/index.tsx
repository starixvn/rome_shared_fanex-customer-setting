import { forwardRef, useImperativeHandle, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { ValidationRefType } from '@customer-web-app/share-models';
import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import { BetSettingsProps, RNGGamesBetSettingGroupItem } from '@customer-web-app/shared-group-settings-types';
import { scrollToBLock } from '@customer-web-app/shared-group-settings-ui';
import { Block, BlockColumn, BlockRefType, BlockRow } from '@customer-web-app/shared/base/block';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import CopyIcon from '../../copy-icon';
import { GroupSettingBlockProps } from '../../types';
import { ValidateBetSetting } from '../bet-setting-base-validator';
import BetSettingNote from '../bet-setting-note';

const RNGGameBetSettingProps: ReadonlyArray<keyof RNGGamesBetSettingGroupItem> = ['minBet', 'maxBet', 'maxPerMatch'];
const RNGGamesBetSettingSubTypeProps: { [key: number]: (keyof RNGGamesBetSettingGroupItem | '')[] } = {
  1: [...RNGGameBetSettingProps, ''],
  2: [...RNGGameBetSettingProps, 'maxPerBall'],
};

const LotteryKey = '220-1024';
const NumberGameKey = '161-81';
const RngGamesCopyLabel: ReadonlyArray<string> = [LotteryKey, NumberGameKey];

export default function RNGGamesBetSetting({
  is789y,
  groupTypeId,
  groupItems,
}: BetSettingsProps<RNGGamesBetSettingGroupItem>) {
  const { t } = useTranslation();
  const [error, submitCount] = useNewAccountStore((state) => [state.betSettingErrors, state.submitCount], shallow);
  const handleCopyRngGames = useNewAccountStore((state) => state.handleCopyRngGames);
  const [copyId, handleCopyBetSettings] = useCopyBetSetting();

  const isCopingRow = (subGroupItems: RNGGamesBetSettingGroupItem[], index: number) => {
    if (copyId === '') return false;
    const [sportTypeId, betTypeId] = copyId.split('-');
    const copyItemIndex = subGroupItems.findIndex(
      (copyItem) => copyItem.sportTypeId === Number(sportTypeId) && copyItem.betTypeId === Number(betTypeId),
    );
    return index > copyItemIndex && copyItemIndex !== -1;
  };

  const validate = (
    configs: RNGGamesBetSettingGroupItem,
    newBetSettings: RNGGamesBetSettingGroupItem,
    prop: keyof RNGGamesBetSettingGroupItem,
  ) => {
    const error = new RNGGamesValidator(is789y, configs, newBetSettings).validate(prop);

    return {
      isValid: error === '',
      message: error !== '' ? getErrorMessageTranslate(error, t) : '',
    };
  };

  const renderLabel = (groupItem: RNGGamesBetSettingGroupItem) => {
    const sportTypeKey = `${groupItem.sportTypeId}-${groupItem.betTypeId}`;
    if (RngGamesCopyLabel.includes(sportTypeKey))
      return renderCopyLabel(groupItem.displayName, copyId === sportTypeKey, () => {
        const isCopyable = handleCopyRngGames(sportTypeKey, groupItem);
        isCopyable && handleCopyBetSettings(sportTypeKey);
      });
    return groupItem.displayName;
  };

  return (
    <Block title={t('BetSetting')} isCollapse={{ collapse: error.length === 0, resetCollapse: submitCount }}>
      {Object.keys(RNGGamesBetSettingSubTypeProps).map((subType) => {
        const subTypeNumber = Number(subType);
        const properties = [...RNGGamesBetSettingSubTypeProps[subTypeNumber]];
        const subGroupItems = groupItems.filter((groupItem) => groupItem.subType === subTypeNumber);

        return (
          <React.Fragment key={subType}>
            <BetSettingHeading key={subType} subType={subType} properties={properties} />
            {subGroupItems.map((subGroupItem, index) => (
              <BetSettingRow
                key={`${subGroupItem.sportTypeId}-${subGroupItem.betTypeId}`}
                groupTypeId={groupTypeId}
                groupItem={subGroupItem}
                label={renderLabel(subGroupItem)}
                properties={[...RNGGamesBetSettingSubTypeProps[Number(subType)]]}
                validate={validate}
                isCoping={isCopingRow(subGroupItems, index)}
              />
            ))}
          </React.Fragment>
        );
      })}
    </Block>
  );
}
