import { forwardRef, useImperativeHandle, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { ValidationRefType } from '@customer-web-app/share-models';
import { GroupTypeIds, ProductGroupIds, UserLevel } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import { RNGGamesBetSettingGroupItem } from '@customer-web-app/shared-group-settings-types';
import { Block, BlockColumn, BlockRefType, BlockRow } from '@customer-web-app/shared/base/block';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import CopyIcon from '../../copy-icon';
import { scrollToBLock } from '../../scroll-helper';
import { GroupSettingBlockProps } from '../../types';
import { ValidateBetSetting } from '../bet-setting-base-validator';
import BetSettingNote from '../bet-setting-note';
import RNGGamesBetSettingRow from './rnggames-bet-setting-row';
import RNGGamesBetSettingValidator from './rnggames-bet-setting-validator';
import { AppConfigHelper } from '@customer-web-app/shared-helpers';

const BetSettingBlockId = 'bet-setting';
const SportsLotteryKey = '169-3611';
const CopyableRowKeys: ReadonlyArray<string> = [SportsLotteryKey];

const RNGGamesBetSetting = forwardRef<ValidationRefType, GroupSettingBlockProps>(function RNGGamesBetSetting(
  { isReadOnly },
  ref
) {
  const { t } = useTranslation();
  const {
    productGroups,
    isMultipleMode,
    handleUpdateSettingItems,
    handleSettingItemChange,
    generalInfo,
    isEditMode,
    updateGroupSetting,
    roleId,
  } = useGroupSettingContext();
  const [sourceItemIndex, setSourceItemIndex] = useState<number | undefined>(undefined);
  const [validationState, setValidationState] = useState<ValidateBetSetting>();
  const blockRef = useRef<BlockRefType>(null);
  const timeId = useRef<NodeJS.Timeout>();
  const isSpecialSite = generalInfo?.isSpecialSite ?? false;

  useImperativeHandle(ref, () => {
    const validate = () => {
      let validateResult = true;
      if (isReadOnly) return validateResult;

      for (const element of betSettings) {
        if (element.isMultipleModeChecked) {
          const validator = new RNGGamesBetSettingValidator(element, isSpecialSite);
          const validateItemResult = validator.validateBetSettingItem();
          const isErrorField = Object.values(validateItemResult).find((item) => !item.isValid);
          if (isErrorField) {
            setValidationState(isErrorField);
            validateResult = false;
            break;
          }
        }
      }

      if (validateResult === false) {
        blockRef.current?.expand();
      }

      if (validateResult) {
        setValidationState(undefined);
      }

      return validateResult;
    };

    return {
      validate,
      scrollToBlock: () => scrollToBLock(BetSettingBlockId),
    };
  });

  const betSettings = getSettingItems<RNGGamesBetSettingGroupItem>(
    productGroups,
    ProductGroupIds.RNGGames,
    GroupTypeIds.RNGGamesBetSetting
  );

  const handleSelectAll = (isChecked: boolean) => {
    const newBetSettings = betSettings.map((item) => ({ ...item, isMultipleModeChecked: isChecked }));
    handleUpdateSettingItems?.(ProductGroupIds.RNGGames, GroupTypeIds.RNGGamesBetSetting, newBetSettings);
  };

  const handleSelectMinBet = (isChecked: boolean) => {
    const newBetSettings = betSettings.map((item) => ({ ...item, minBetMultipleMode: isChecked }));
    handleUpdateSettingItems?.(ProductGroupIds.RNGGames, GroupTypeIds.RNGGamesBetSetting, newBetSettings);
  };

  const handleSelectMaxBet = (isChecked: boolean) => {
    const newBetSettings = betSettings.map((item) => ({ ...item, maxBetMultipleMode: isChecked }));
    handleUpdateSettingItems?.(ProductGroupIds.RNGGames, GroupTypeIds.RNGGamesBetSetting, newBetSettings);
  };

  const handleSelectMaxPerMatch = (isChecked: boolean) => {
    const newBetSettings = betSettings.map((item) => ({ ...item, maxPerMatchMultipleMode: isChecked }));
    handleUpdateSettingItems?.(ProductGroupIds.RNGGames, GroupTypeIds.RNGGamesBetSetting, newBetSettings);
  };

  const handleSelectMaxPerBall = (isChecked: boolean) => {
    const newBetSettings = betSettings.map((item) => ({ ...item, maxPerBallMultipleMode: isChecked }));
    handleUpdateSettingItems?.(ProductGroupIds.RNGGames, GroupTypeIds.RNGGamesBetSetting, newBetSettings);
  };

  const handleCopyClick = (sourceBetSetting: RNGGamesBetSettingGroupItem) => {
    const validator = new RNGGamesBetSettingValidator(sourceBetSetting, isSpecialSite);
    const itemValidateResult = validator.validateBetSettingItem();
    if (Object.values(itemValidateResult).every((value) => value.isValid)) {
      const { sportTypeId, betTypeId } = sourceBetSetting;
      const sourceIndex = betSettings.findIndex((x) => x.sportTypeId === sportTypeId && x.betTypeId === betTypeId);
      const newBetSettings = betSettings.map((item, index) => {
        if (index > sourceIndex) {
          return {
            ...item,
            ...{
              minBet: sourceBetSetting.minBet,
              maxBet: sourceBetSetting.maxBet,
              maxPerMatch: sourceBetSetting.maxPerMatch,
              maxPerBall: sourceBetSetting.maxPerBall === null ? item.maxPerBall : sourceBetSetting.maxPerBall,
              isMultipleModeChecked: sourceBetSetting.isMultipleModeChecked,
            },
          };
        }
        return item;
      });
      handleUpdateSettingItems?.(ProductGroupIds.RNGGames, GroupTypeIds.RNGGamesBetSetting, newBetSettings);
      if (sourceIndex !== sourceItemIndex) {
        if (timeId) clearTimeout(timeId.current);
        setSourceItemIndex(sourceIndex);
        timeId.current = setTimeout(() => {
          setSourceItemIndex(undefined);
        }, 3000);
      }
    } else {
      const isErrorField = Object.values(itemValidateResult).find((item) => !item.isValid);
      if (isErrorField) {
        setValidationState(isErrorField);
      }
    }
  };

  const renderLabel = (
    betSetting: RNGGamesBetSettingGroupItem,
    index: number,
    onChange: (isChecked: boolean) => void
  ) => {
    const key = `${betSetting.sportTypeId}-${betSetting.betTypeId}`;
    const renderCheckBox = () => {
      return (
        <input
          type="checkbox"
          onChange={(e) => onChange(e.target.checked)}
          checked={betSetting.isMultipleModeChecked}
        />
      );
    };

    if (CopyableRowKeys.includes(key)) {
      return (
        <>
          {isMultipleMode && !isReadOnly && renderCheckBox()}
          <div style={{ display: 'flex', justifyContent: 'space-between', width: '100%' }}>
            <span>{betSetting.displayName}</span>
            {!isReadOnly && (
              <CopyIcon
                id="copy-bet-setting"
                isCopied={sourceItemIndex === index}
                onClick={() => {
                  if (betSetting.isMultipleModeChecked) {
                    handleCopyClick(betSetting);
                  }
                }}
              />
            )}
          </div>
        </>
      );
    }

    return (
      <div>
        {isMultipleMode && !isReadOnly && renderCheckBox()}
        <span>{betSetting.displayName}</span>
      </div>
    );
  };

  return (
    <Block
      id={BetSettingBlockId}
      title={
        <>
          {t('BetSetting')}
          <BetSettingNote />
        </>
      }
      isPartialUpdate={isEditMode === true && !isReadOnly}
      ref={blockRef}
      onSaveClick={() => updateGroupSetting?.(GroupTypeIds.RNGGamesBetSetting)}
    >
      <BlockRow
        isHeaderRow
        label={
          isMultipleMode &&
          !isReadOnly && (
            <div style={{ display: 'flex', marginLeft: '2px' }}>
              <input
                type="checkbox"
                style={{ top: '0' }}
                onChange={(e) => handleSelectAll(e.target.checked)}
                checked={betSettings.every((item) => item.isMultipleModeChecked === true)}
              />
              <div>{t('SelectAllSports')}</div>
            </div>
          )
        }
      >
        <BlockColumn>
          {
            <>
              {!isReadOnly && roleId === UserLevel.Super && !AppConfigHelper.isEditBetSettingOnly() && (
                <input
                    type="checkbox"
                    defaultChecked={true}
                    style={{ top: '0', margin: '0 4px' }}
                    onChange={(e) => handleSelectMinBet(e.target.checked)}
                    checked={betSettings.every((item) => item.minBetMultipleMode === true)}
                  />
              )}
              {t('minBet')}
            </>
          }
        </BlockColumn>
        <BlockColumn>
          {
            <>
              {!isReadOnly && roleId === UserLevel.Super && !AppConfigHelper.isEditBetSettingOnly() && (
                <input
                    type="checkbox"
                    defaultChecked={true}
                    style={{ top: '0', margin: '0 4px' }}
                    onChange={(e) => handleSelectMaxBet(e.target.checked)}
                    checked={betSettings.every((item) => item.maxBetMultipleMode === true)}
                  />
              )}
              {t('maxBet')}
            </>
          }
        </BlockColumn>
        <BlockColumn>
          {
            <>
              {!isReadOnly && roleId === UserLevel.Super && !AppConfigHelper.isEditBetSettingOnly() && (
                <input
                    type="checkbox"
                    defaultChecked={true}
                    style={{ top: '0', margin: '0 4px' }}
                    onChange={(e) => handleSelectMaxPerMatch(e.target.checked)}
                    checked={betSettings.every((item) => item.maxPerMatchMultipleMode === true)}
                  />
              )}
              {t('maxPerMatch')}
              </>
          }
        </BlockColumn>
        <BlockColumn>
          {
            <>
              {!isReadOnly && roleId === UserLevel.Super && !AppConfigHelper.isEditBetSettingOnly() && (
                <input
                    type="checkbox"
                    defaultChecked={true}
                    style={{ top: '0', margin: '0 4px' }}
                    onChange={(e) => handleSelectMaxPerBall(e.target.checked)}
                    checked={betSettings.every((item) => item.maxPerBallMultipleMode === true)}
                  />
              )}
              {t('maxPerBall')}
            </>
          }
        </BlockColumn>
      </BlockRow>
      {betSettings?.map((betSetting, index) => {
        const key = `${betSetting.sportTypeId}-${betSetting.betTypeId}`;
        return (
          <RNGGamesBetSettingRow
            key={key}
            isReadOnly={isReadOnly}
            settingItem={betSetting}
            label={renderLabel(betSetting, index, (isChecked) =>
              handleSettingItemChange?.(
                ProductGroupIds.RNGGames,
                GroupTypeIds.RNGGamesBetSetting,
                betSetting.sportTypeId,
                betSetting.betTypeId,
                {
                  isMultipleModeChecked: isChecked,
                }
              )
            )}
            isCopied={sourceItemIndex !== undefined && index > sourceItemIndex}
            validationState={validationState}
            setErrorTooltip={(validationState: ValidateBetSetting) => setValidationState(validationState)}
          />
        );
      })}

      {validationState && !validationState.isValid && (
        <WrapperTooltip
          type="error"
          anchorSelect={`#${validationState.fieldName}`}
          isOpen={!validationState.isValid}
          content={validationState?.message ? t(validationState.message, validationState.msgParams) : ''}
        />
      )}
    </Block>
  );
});

export default RNGGamesBetSetting;
