import cn from 'classnames';
import { forwardRef, MouseEvent, useEffect, useImperativeHandle, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { NumericFormat } from 'react-number-format';

import { ValidationRefType } from '@customer-web-app/share-models';
import { GroupTypeIds, MaxNumberInputLength, UserLevel } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import { WinLossLimitGroupItem } from '@customer-web-app/shared-group-settings-types';
import { NumberHelper } from '@customer-web-app/shared-helpers';
import { Block, BlockColumn, BlockRefType, BlockRow } from '@customer-web-app/shared/base/block';
import { IconInfo } from '@customer-web-app/shared/base/common-icon';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';
import NiceModal from '@ebay/nice-modal-react';

import { CustomerSettingInformationPageUrl } from '../customer-setting-information-page-url';
import scrollToBLock from '../scroll-helper';
import { GroupSettingBlockProps } from '../types';
import WinLossAmount from './win-loss-amount';

const MaximumWinOptions = [
  { value: 0, displayName: 'unlimited' },
  { value: 3, displayName: '3X' },
  { value: 5, displayName: '5X' },
  { value: 10, displayName: '10X' },
];

const UnLimitedWinLossLimit = 0;

const validateWinLossLimitItem = (winLossLimit: WinLossLimitGroupItem) => {
  const id = `lossLimit-${winLossLimit.productId}-${winLossLimit.sportTypeId}-${winLossLimit.betTypeId}`;
  if (winLossLimit.lossLimitValue === undefined && winLossLimit.lossLimit !== UnLimitedWinLossLimit) {
    return {
      id,
      productId: winLossLimit.productId,
      isValid: false,
      message: 'RequiredField',
    };
  }

  if (
    winLossLimit.lossLimit !== UnLimitedWinLossLimit &&
    ((winLossLimit.lossLimitValue < winLossLimit.minLossRange && winLossLimit.minLossRange !== UnLimitedWinLossLimit) ||
      (winLossLimit.lossLimitValue > winLossLimit.maxLossRange && winLossLimit.maxLossRange !== UnLimitedWinLossLimit))
  ) {
    return {
      id,
      productId: winLossLimit.productId,
      isValid: false,
      message: 'MaximumLossValueShouldBeInRange',
      msgParams: {
        min: NumberHelper.formatWithCommas(winLossLimit.minLossRange),
        max: NumberHelper.formatWithCommas(winLossLimit.maxLossRange),
      },
    };
  }

  return { id: '', isValid: true };
};

type ValidateWinLossLimitResult = {
  id: string;
  productId?: number;
  isValid: boolean;
  message?: string;
  msgParams?: { [key: string]: string };
};

type MaximumLossColumnProps = {
  isReadOnly: boolean;
  winLossLimit: WinLossLimitGroupItem;
  handleWinLossLimitChange: (
    winLossLimit: WinLossLimitGroupItem,
    newWinLossLimit: Partial<WinLossLimitGroupItem>
  ) => void;
  setTooltipError: (validateResult: ValidateWinLossLimitResult) => void;
};

function MaximumLossColumn({
  isReadOnly,
  winLossLimit,
  handleWinLossLimitChange,
  setTooltipError,
}: Readonly<MaximumLossColumnProps>) {
  const { t } = useTranslation();

  const [validateWinLossLimit, setValidateWinLossLimit] = useState<ValidateWinLossLimitResult>({
    id: '',
    isValid: true,
  });

  useEffect(() => {
    setValidateWinLossLimit(validateWinLossLimitItem(winLossLimit));
  }, [winLossLimit]);

  const id = `lossLimit-${winLossLimit.productId}-${winLossLimit.sportTypeId}-${winLossLimit.betTypeId}`;
  return (
    <BlockColumn>
      <div>
        <div style={{ display: 'flex', alignItems: 'center' }}>
          {winLossLimit.lossLimit === UnLimitedWinLossLimit ? (
            <input type="text" value={t('unlimited')} disabled />
          ) : (
            <div className={cn({ error: validateWinLossLimit.id === id })} id={id}>
              <div className="blockLeft">
                <NumericFormat
                  type="text"
                  thousandSeparator=","
                  allowNegative={false}
                  value={winLossLimit.lossLimitValue}
                  isAllowed={(values) => {
                    const { floatValue } = values;
                    if (floatValue === undefined) return true;

                    return floatValue > 0;
                  }}
                  maxLength={MaxNumberInputLength}
                  disabled={isReadOnly}
                  onFocus={() => setTooltipError(validateWinLossLimitItem(winLossLimit))}
                  onBlur={() => setTooltipError({ id: '', isValid: true })}
                  onChange={(e) => {
                    const newLossLimitValue = NumberHelper.parseNumber(e.target.value);
                    const newWinLossLimit = {
                      ...winLossLimit,
                      lossLimit: newLossLimitValue ?? winLossLimit.lossLimit,
                      lossLimitValue: e.target.value !== '' ? newLossLimitValue : undefined,
                    };
                    setTooltipError(validateWinLossLimitItem(newWinLossLimit as WinLossLimitGroupItem));
                    handleWinLossLimitChange(winLossLimit, newWinLossLimit);
                  }}
                />
              </div>
            </div>
          )}
          <span>
            ({NumberHelper.formatWithCommas(winLossLimit.minLossRange)} -{' '}
            {winLossLimit.maxLossRange === UnLimitedWinLossLimit
              ? t('unlimited')
              : NumberHelper.formatWithCommas(winLossLimit.maxLossRange)}
            )
          </span>
        </div>
        {winLossLimit.maxLossRange === UnLimitedWinLossLimit && (
          <div>
            <div className="blockLeft">
              <input
                type="checkbox"
                disabled={isReadOnly}
                checked={winLossLimit.lossLimit === UnLimitedWinLossLimit}
                onChange={(e) =>
                  handleWinLossLimitChange(winLossLimit, {
                    lossLimit: e.target.checked ? 0 : winLossLimit.lossLimitValue,
                  })
                }
              />
              <span>{t('unlimited')}</span>
            </div>
          </div>
        )}
      </div>
    </BlockColumn>
  );
}

const getValidMaximumWinOptions = (winLossLimit: WinLossLimitGroupItem) => {
  return MaximumWinOptions.filter((opt) => {
    return (
      (opt.value <= winLossLimit.maxWinRange && opt.value !== UnLimitedWinLossLimit) ||
      winLossLimit.maxWinRange === UnLimitedWinLossLimit
    );
  });
};

const winLossLimitBlockId = 'WinLossLimitBlockId';

const WinLossLimit = forwardRef<ValidationRefType, GroupSettingBlockProps>(function WinLossLimit(props, ref) {
  const { t } = useTranslation();
  const { isReadOnly } = props;
  const { productGroups, productGroupId, isEditMode, handleSettingItemChange, updateGroupSetting } =
    useGroupSettingContext();
  const groupId = Number(productGroupId);
  const winLossLimitItems = getSettingItems<WinLossLimitGroupItem>(productGroups, groupId, GroupTypeIds.WinLossLimit);
  const productIds = Array.from(new Set(winLossLimitItems.map((item) => item.productId)));

  const [selectedProductId, setSelectedProductId] = useState(productIds[0]);
  const [validateWinLossLimit, setValidateWinLossLimit] = useState<ValidateWinLossLimitResult>({
    id: '',
    isValid: true,
  });
  const blockRef = useRef<BlockRefType>(null);

  useEffect(() => {
    if (!productIds.includes(selectedProductId)) {
      setSelectedProductId(productIds[0]);
    }
  }, [productIds, selectedProductId]);

  const validateWinLossLimits = (winLossLimitItems: WinLossLimitGroupItem[]) => {
    for (const item of winLossLimitItems) {
      const error = validateWinLossLimitItem(item);
      if (!error.isValid) {
        setValidateWinLossLimit(error);
        return error;
      }
    }

    return { id: '', isValid: true };
  };

  useImperativeHandle(ref, () => {
    const validate = () => {
      const validateResult = validateWinLossLimits(winLossLimitItems);

      if (!validateResult.isValid) {
        if (validateResult.productId) {
          setSelectedProductId(validateResult.productId);
        }
        blockRef.current?.expand();
      }

      if (validateResult.isValid) {
        setValidateWinLossLimit({ id: '', isValid: true });
      }

      return validateResult.isValid;
    };

    return {
      validate,
      scrollToBlock: () => scrollToBLock(winLossLimitBlockId),
    };
  });

  const handleWinLossLimitChange = (
    winLossLimit: WinLossLimitGroupItem,
    newWinLossLimit: Partial<WinLossLimitGroupItem>
  ) => {
    handleSettingItemChange?.(
      groupId,
      GroupTypeIds.WinLossLimit,
      winLossLimit.sportTypeId,
      winLossLimit.betTypeId,
      newWinLossLimit
    );
  };

  const renderMaximumWisOptions = (winLossLimit: WinLossLimitGroupItem) => {
    const maximumWinOptions = getValidMaximumWinOptions(winLossLimit);

    if (maximumWinOptions.length < 1) return null;

    return (
      <select className='long-select'
        key={winLossLimit.sportTypeId}
        value={winLossLimit.winLimit}
        onChange={(e) => handleWinLossLimitChange(winLossLimit, { winLimit: Number(e.target.value) })}
        disabled={isReadOnly}
      >
        {maximumWinOptions.map((opt) => {
          return (
            <option key={opt.value} value={opt.value}>
              {t(opt.displayName)}
            </option>
          );
        })}
      </select>
    );
  };

  const showCurrentWinLossAmount = (sportType: number, betType: number) => {
    NiceModal.show(WinLossAmount, { sportType, betType });
  };

  return (
    <div id={winLossLimitBlockId}>
      <Block
        ref={blockRef}
        title={
          <>
            {t('WinLossLimit')}
            <Note />
          </>
        }
        isPartialUpdate={isEditMode === true && !isReadOnly}
        onSaveClick={() => updateGroupSetting?.(GroupTypeIds.WinLossLimit)}
      >
        <div className="nav-tab">
          {productIds.map((item) => (
            <button
              className={cn('item', { active: item === selectedProductId })}
              key={item}
              onClick={() => setSelectedProductId(item)}
            >
              {winLossLimitItems.find((i) => i.productId === item)?.productName}
            </button>
          ))}
        </div>
        <BlockRow isHeaderRow>
          <BlockColumn>{t('MaximumWin')}</BlockColumn>
          <BlockColumn>{t('MaximumLoss')}</BlockColumn>
          <BlockColumn>{t('')}</BlockColumn>
        </BlockRow>
        {winLossLimitItems
          .filter((item) => item.productId === selectedProductId)
          .map((winLossLimit) => {
            return (
              <BlockRow key={winLossLimit.sportTypeId} label={winLossLimit.sportTypeName}>
                <BlockColumn>{renderMaximumWisOptions(winLossLimit)}</BlockColumn>
                <MaximumLossColumn
                  isReadOnly={isReadOnly}
                  winLossLimit={winLossLimit}
                  handleWinLossLimitChange={handleWinLossLimitChange}
                  setTooltipError={setValidateWinLossLimit}
                />
                <BlockColumn>
                  <div>
                    <div className="blockLeft" style={{ display: 'flex', flexDirection: 'column' }}>
                      <div style={{ display: 'flex', alignItems: 'center' }}>
                        <input
                          type="checkbox"
                          checked={winLossLimit.isAutoReset}
                          disabled={!winLossLimit.isAutoResetUpline || isReadOnly}
                          onChange={(e) => handleWinLossLimitChange(winLossLimit, { isAutoReset: e.target.checked })}
                        />
                        <span>{t('DailyAutoResetLimitAt')}</span>
                      </div>
                      <div style={{ widows: '100%' }}>
                        {params.roleId === UserLevel.Member && (
                          <div
                            aria-hidden
                            onClick={() => showCurrentWinLossAmount(winLossLimit.sportTypeId, winLossLimit.betTypeId)}
                            style={{ cursor: 'pointer', color: 'blue' }}
                          >
                            {t('ClickToViewCurrentWinLossAmount')}
                          </div>
                        )}
                      </div>
                    </div>
                  </div>
                </BlockColumn>
              </BlockRow>
            );
          })}
        {!validateWinLossLimit.isValid && (
          <WrapperTooltip
            type="error"
            anchorSelect={`#${validateWinLossLimit.id}`}
            isOpen={selectedProductId === validateWinLossLimit.productId && !validateWinLossLimit.isValid}
            content={t(validateWinLossLimit?.message ?? '', validateWinLossLimit?.msgParams)}
          />
        )}
      </Block>
    </div>
  );
});

function Note() {
  const { t } = useTranslation();
  const { url } = CustomerSettingInformationPageUrl({ tab: 5 });

  const handleClick = (e: MouseEvent<HTMLDivElement>) => {
    e.preventDefault();
    window.open(url, '_blank');
    e.stopPropagation();
  };

  const tooltipContent = t('ViewWinLossLimitInfo');
  return (
    <span onClick={handleClick} aria-hidden data-tooltip-id="global-tooltip" data-tooltip-content={tooltipContent}>
      {IconInfo}
    </span>
  );
}

export default WinLossLimit;
