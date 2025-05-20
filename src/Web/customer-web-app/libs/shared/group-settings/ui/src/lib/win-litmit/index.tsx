import cn from 'classnames';
import { forwardRef, MouseEvent, useEffect, useImperativeHandle, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { NumericFormat } from 'react-number-format';

import { ValidationRefType } from '@customer-web-app/share-models';
import { GroupTypeIds, MaxNumberInputLength, UserLevel } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import { WinLimitGroupItem } from '@customer-web-app/shared-group-settings-types';
import { NumberHelper } from '@customer-web-app/shared-helpers';
import { Block, BlockColumn, BlockRefType, BlockRow } from '@customer-web-app/shared/base/block';
import { IconInfo } from '@customer-web-app/shared/base/common-icon';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';
import NiceModal from '@ebay/nice-modal-react';

import { CustomerSettingInformationPageUrl } from '../customer-setting-information-page-url';
import { scrollToBLock } from '../scroll-helper';
import { GroupSettingBlockProps } from '../types';
import WinLossAmount from './win-amount';

const UnLimitedWinLossLimit = 0;

const validateWinLimitItem = (winLossLimit: WinLimitGroupItem) => {
  const id = `winLimit-${winLossLimit.productId}-${winLossLimit.sportTypeId}-${winLossLimit.betTypeId}`;
  if (winLossLimit.winLimitValue === undefined && winLossLimit.winLimit !== UnLimitedWinLossLimit) {
    return {
      id,
      productId: winLossLimit.productId,
      isValid: false,
      message: 'RequiredField',
    };
  }


  if (
    winLossLimit.winLimit !== UnLimitedWinLossLimit &&
    ((winLossLimit.winLimitValue < winLossLimit.minWinRange && winLossLimit.minWinRange !== UnLimitedWinLossLimit) ||
      (winLossLimit.winLimitValue > winLossLimit.maxWinRange && winLossLimit.maxWinRange !== UnLimitedWinLossLimit))
  ) {
    return {
      id,
      productId: winLossLimit.productId,
      isValid: false,
      message: 'MaximumWinValueShouldBeInRange',
      msgParams: {
        min: NumberHelper.formatWithCommas(winLossLimit.minWinRange),
        max: NumberHelper.formatWithCommas(winLossLimit.maxWinRange),
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
  data: WinLimitGroupItem;
  handleWinLimitChange: (
    winLossLimit: WinLimitGroupItem,
    newWinLossLimit: Partial<WinLimitGroupItem>
  ) => void;
  setTooltipError: (validateResult: ValidateWinLossLimitResult) => void;
};

function MaximumWinColumn(props: Readonly<MaximumLossColumnProps>) {
  const { isReadOnly, data, handleWinLimitChange, setTooltipError } = props;
  const { t } = useTranslation();

  const [validateWinLossLimit, setValidateWinLossLimit] = useState<ValidateWinLossLimitResult>({
    id: '',
    isValid: true,
  });

  useEffect(() => {
    setValidateWinLossLimit(validateWinLimitItem(data));
  }, [data]);

  const id = `winLimit-${data.productId}-${data.sportTypeId}-${data.betTypeId}`;
  return (
    <BlockColumn>
      <div>
        <div style={{ display: 'flex', alignItems: 'center' }}>
          {data.winLimit === UnLimitedWinLossLimit ? (
            <input type="text" value={t('unlimited')} disabled />
          ) : (
            <div className={cn({ error: validateWinLossLimit.id === id })} id={id}>
              <div className="blockLeft">
                <NumericFormat
                  type="text"
                  thousandSeparator=","
                  allowNegative={false}
                  value={data.winLimitValue}
                  isAllowed={(values) => {
                    const { floatValue } = values;
                    if (floatValue === undefined) return true;

                    return floatValue > 0;
                  }}
                  maxLength={MaxNumberInputLength}
                  disabled={isReadOnly}
                  onFocus={() => setTooltipError(validateWinLimitItem(data))}
                  onBlur={() => setTooltipError({ id: '', isValid: true })}
                  onChange={(e) => {
                    const newWinLimitValue = NumberHelper.parseNumber(e.target.value);
                    const newWinLimit = {
                      ...data,
                      winLimit: newWinLimitValue ?? data.winLimit,
                      winLimitValue: e.target.value !== '' ? newWinLimitValue : undefined,
                    };
                    setTooltipError(validateWinLimitItem(newWinLimit as WinLimitGroupItem));
                    handleWinLimitChange(data, newWinLimit);
                  }}
                />
              </div>
            </div>
          )}
          <span>
            ({NumberHelper.formatWithCommas(data.minWinRange)} -{' '}
            {data.maxWinRange === UnLimitedWinLossLimit
              ? t('unlimited')
              : NumberHelper.formatWithCommas(data.maxWinRange)}
            )
          </span>
        </div>
        {data.maxWinRange === UnLimitedWinLossLimit && (
          <div>
            <div className="blockLeft">
              <input
                type="checkbox"
                disabled={isReadOnly}
                checked={data.winLimit === UnLimitedWinLossLimit}
                onChange={(e) =>
                  handleWinLimitChange(data, {
                    winLimit: e.target.checked ? 0 : data.winLimitValue,
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

const winLimitBlockId = 'WinLimitBlockId';

const WinLimit = forwardRef<ValidationRefType, GroupSettingBlockProps>(function WinLimit(props, ref) {
  const { t } = useTranslation();
  const { isReadOnly } = props;
  const { productGroups, productGroupId, isEditMode, handleSettingItemChange, updateGroupSetting } = useGroupSettingContext();
  const groupId = Number(productGroupId);
  const winLimitItems = getSettingItems<WinLimitGroupItem>(productGroups, groupId, GroupTypeIds.WinLimit);
  const productIds = Array.from(new Set(winLimitItems.map((item) => item.productId)));

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

  const validateWinLossLimits = (winLossLimitItems: WinLimitGroupItem[]) => {
    for (const item of winLossLimitItems) {
      const error = validateWinLimitItem(item);
      if (!error.isValid) {
        setValidateWinLossLimit(error);
        return error;
      }
    }

    return { id: '', isValid: true };
  };

  useImperativeHandle(ref, () => {
    const validate = () => {
      const validateResult = validateWinLossLimits(winLimitItems);

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
      scrollToBlock: () => scrollToBLock(winLimitBlockId),
    };
  });

  const handleWinLimitChange = (
    winLimit: WinLimitGroupItem,
    newWinLimit: Partial<WinLimitGroupItem>
  ) => {
    handleSettingItemChange?.(
      groupId,
      GroupTypeIds.WinLimit,
      winLimit.sportTypeId,
      winLimit.betTypeId,
      newWinLimit
    );
  };

  const showCurrentWinLossAmount = (sportType: number, betType: number) => {
    NiceModal.show(WinLossAmount, { sportType, betType });
  };
  console.log('winLimitItems', winLimitItems)
  return (
    <div id={winLimitBlockId} style={{ width: '880px', height: '100%' }}>
      <Block
        ref={blockRef}
        title={
          <>
            {t('WinLimit')}
            <Note />
          </>
        }
        isPartialUpdate={isEditMode === true && !isReadOnly}
        onSaveClick={() => updateGroupSetting?.(GroupTypeIds.WinLimit)}
      >
        {productIds.length > 1 && (
          <div className="nav-tab">
            {productIds.map((item) => (
              <button
                className={cn('item', { active: item === selectedProductId })}
                key={item}
                onClick={() => setSelectedProductId(item)}
              >
                {winLimitItems.find((i) => i.productId === item)?.productName}
              </button>
            ))}
          </div>
        )}
        <BlockRow isHeaderRow>
          <BlockColumn>{t('MaximumWin')}</BlockColumn>
          <div style={{ width: '400px', height: '100%' }}>
            <BlockColumn>{t('')}</BlockColumn>
          </div>
        </BlockRow>
        {winLimitItems
          .filter((item) => item.productId === selectedProductId)
          .map((winLimit) => {
            return (
              <BlockRow key={winLimit.sportTypeId} label={winLimit.sportTypeName}>
                <MaximumWinColumn
                  isReadOnly={isReadOnly}
                  data={winLimit}
                  handleWinLimitChange={handleWinLimitChange}
                  setTooltipError={setValidateWinLossLimit}
                />
                <div style={{ width: '400px', height: '100%' }}>
                  <BlockColumn>
                    <div>
                      <div className="blockLeft" style={{ display: 'flex', flexDirection: 'column' }}>
                        <div style={{ display: 'flex', alignItems: 'center' }}>
                          <input
                            type="checkbox"
                            checked={winLimit.isAutoReset}
                            disabled={!winLimit.isAutoResetUpline || isReadOnly}
                            onChange={(e) => handleWinLimitChange(winLimit, { isAutoReset: e.target.checked })}
                          />
                          <div>
                            <span>{t('DailyAutoResetLimitAt')} </span>
                            {params.roleId === UserLevel.Member && (
                              <span
                                aria-hidden
                                onClick={() =>
                                  showCurrentWinLossAmount(winLimit.sportTypeId, winLimit.betTypeId)
                                }
                                style={{ cursor: 'pointer', color: 'blue', display: 'inline-block' }}
                              >
                                {t('ClickToViewCurrentWinLossAmount')}
                              </span>
                            )}
                          </div>
                        </div>
                      </div>
                    </div>
                  </BlockColumn>
                </div>
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

  const tooltipContent = t('ViewWinLimitInfo');
  return (
    <span onClick={handleClick} aria-hidden data-tooltip-id="global-tooltip" data-tooltip-content={tooltipContent}>
      {IconInfo}
    </span>
  );
}

export default WinLimit;
