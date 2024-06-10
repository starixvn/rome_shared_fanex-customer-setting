import { forwardRef, useImperativeHandle, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { ValidationRefType } from '@customer-web-app/share-models';
import { GroupTypeIds, ProductGroupIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import { SportsBookBetSettingGroupItem } from '@customer-web-app/shared-group-settings-types';
import { Block, BlockColumn, BlockRefType, BlockRow } from '@customer-web-app/shared/base/block';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import CopyIcon from '../../copy-icon';
import { scrollToBLock } from '../../scroll-helper';
import { GroupSettingBlockProps } from '../../types';
import { ValidateBetSetting } from '../bet-setting-base-validator';
import BetSettingNote from '../bet-setting-note';
import SportsBookBetSettingMaxPayoutNote from './sportsbook-bet-setting-max-payout-note';
import SportsBookBetSettingMixSportParlayNote from './sportsbook-bet-setting-mix-sports-parlay-note';
import { SportsBookBetSettingRow } from './sportsbook-bet-setting-row';
import SportsBookBetSettingValidator from './sportsbook-bet-setting-validator';

const SoccerKey = '1-1';
const BasketballKey = '2-1';
const MixSportParlayKey = '99-9';
const CopyableRowKeys: ReadonlyArray<string> = [SoccerKey, BasketballKey];

const BetSettingBlockId = 'sportsbook-bet-setting';

const SportsBookBetSetting = forwardRef<ValidationRefType, GroupSettingBlockProps>(function SportsBookBetSetting(
  props,
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
  } = useGroupSettingContext();
  const [validationState, setValidationState] = useState<ValidateBetSetting>();
  const blockRef = useRef<BlockRefType>(null);

  const isSpecialSite = generalInfo?.isSpecialSite ?? false;
  const betSettings = getSettingItems<SportsBookBetSettingGroupItem>(
    productGroups,
    ProductGroupIds.SportsBook,
    GroupTypeIds.SportsBookBetSetting
  );
  const [sourceItemIndex, setSourceItemIndex] = useState<number | undefined>(undefined);
  const timeId = useRef<NodeJS.Timeout>();
  const { isReadOnly } = props;

  useImperativeHandle(ref, () => {
    const validate = () => {
      let validateResult = true;
      if (isReadOnly) return validateResult;

      for (const element of betSettings) {
        if (element.isMultipleModeChecked) {
          const validator = new SportsBookBetSettingValidator(element, isSpecialSite);
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

  const handleCopyClick = (sourceBetSetting: SportsBookBetSettingGroupItem) => {
    const validator = new SportsBookBetSettingValidator(sourceBetSetting, isSpecialSite);
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
              maxPayoutPerMatch: item.maxPayoutPerMatch === null ? null : sourceBetSetting.maxPayoutPerMatch,
              isMultipleModeChecked: sourceBetSetting.isMultipleModeChecked,
            },
          };
        }
        return item;
      });
      handleUpdateSettingItems?.(ProductGroupIds.SportsBook, GroupTypeIds.SportsBookBetSetting, newBetSettings);
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

  const handleSelectAll = (isChecked: boolean) => {
    const newBetSettings = betSettings.map((item) => ({ ...item, isMultipleModeChecked: isChecked }));
    handleUpdateSettingItems?.(ProductGroupIds.SportsBook, GroupTypeIds.SportsBookBetSetting, newBetSettings);
  };

  const renderLabel = (
    betSetting: SportsBookBetSettingGroupItem,
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

    if (key === MixSportParlayKey) {
      return (
        <>
          {isMultipleMode && !isReadOnly && renderCheckBox()}
          <span>{betSetting.displayName}</span> <SportsBookBetSettingMixSportParlayNote />
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
      onSaveClick={() => updateGroupSetting?.(GroupTypeIds.SportsBookBetSetting)}
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
              <div>{t('SelectAll')}</div>
            </div>
          )
        }
      >
        <BlockColumn>{t('minBet')}</BlockColumn>
        <BlockColumn>{t('maxBet')}</BlockColumn>
        <BlockColumn>{t('maxPerMatch')}</BlockColumn>
        <BlockColumn>
          {
            <>
              {t('maxPayoutPerMatch')}
              <SportsBookBetSettingMaxPayoutNote />
            </>
          }
        </BlockColumn>
      </BlockRow>
      {betSettings?.map((betSetting, index) => {
        const key = `${betSetting.sportTypeId}-${betSetting.betTypeId}`;
        return (
          <SportsBookBetSettingRow
            key={key}
            isReadOnly={props.isReadOnly}
            settingItem={betSetting}
            label={renderLabel(betSetting, index, (isChecked) =>
              handleSettingItemChange?.(
                ProductGroupIds.SportsBook,
                GroupTypeIds.SportsBookBetSetting,
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

      {validationState && (
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

export default SportsBookBetSetting;
