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

import { CustomerSettingInformationPageUrl, TabNames } from '../customer-setting-information-page-url';
import { scrollToBLock } from '../scroll-helper';
import { GroupSettingBlockProps } from '../types';
import WinAmount from './win-amount';

const UnLimitedWinLimit = 0;

const validateWinLimitItem = (winLimit: WinLimitGroupItem) => {
  const id = `winLimit-${winLimit.productId}-${winLimit.sportTypeId}-${winLimit.betTypeId}`;
  if (winLimit.winLimitValue === undefined && winLimit.winLimit !== UnLimitedWinLimit) {
    return {
      id,
      productId: winLimit.productId,
      isValid: false,
      message: 'RequiredField',
    };
  }

  if (
    winLimit.winLimit !== UnLimitedWinLimit &&
    ((winLimit.winLimitValue < winLimit.minWinRange && winLimit.minWinRange !== UnLimitedWinLimit) ||
      (winLimit.winLimitValue > winLimit.maxWinRange && winLimit.maxWinRange !== UnLimitedWinLimit))
  ) {
    return {
      id,
      productId: winLimit.productId,
      isValid: false,
      message: 'MaximumWinValueShouldBeInRange',
      msgParams: {
        min: NumberHelper.formatWithCommas(winLimit.minWinRange),
        max: NumberHelper.formatWithCommas(winLimit.maxWinRange),
      },
    };
  }

  return { id: '', isValid: true };
};

type ValidateWinLimitResult = {
  id: string;
  productId?: number;
  isValid: boolean;
  message?: string;
  msgParams?: { [key: string]: string };
};

type MaximumWinColumnProps = {
  isReadOnly: boolean;
  data: WinLimitGroupItem;
  handleWinLimitChange: (
    winLimit: WinLimitGroupItem,
    newWinLimit: Partial<WinLimitGroupItem>
  ) => void;
  setTooltipError: (validateResult: ValidateWinLimitResult) => void;
};

function MaximumWinColumn(props: Readonly<MaximumWinColumnProps>) {
  const { isReadOnly, data, handleWinLimitChange, setTooltipError } = props;
  const { t } = useTranslation();

  const [validateWinLimit, setValidateWinLimit] = useState<ValidateWinLimitResult>({
    id: '',
    isValid: true,
  });

  useEffect(() => {
    setValidateWinLimit(validateWinLimitItem(data));
  }, [data]);

  const id = `winLimit-${data.productId}-${data.sportTypeId}-${data.betTypeId}`;
  return (
    <BlockColumn>
      <div>
        <div style={{ display: 'flex', alignItems: 'center' }}>
          {data.winLimit === UnLimitedWinLimit ? (
            <input type="text" value={t('unlimited')} disabled />
          ) : (
            <div className={cn({ error: validateWinLimit.id === id })} id={id}>
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
            {data.maxWinRange === UnLimitedWinLimit
              ? t('unlimited')
              : NumberHelper.formatWithCommas(data.maxWinRange)}
            )
          </span>
        </div>
        {data.maxWinRange === UnLimitedWinLimit && (
          <div>
            <div className="blockLeft">
              <input
                type="checkbox"
                disabled={isReadOnly}
                checked={data.winLimit === UnLimitedWinLimit}
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
  const [validateWinLimit, setValidateWinLimit] = useState<ValidateWinLimitResult>({
    id: '',
    isValid: true,
  });
  const blockRef = useRef<BlockRefType>(null);

  useEffect(() => {
    if (!productIds.includes(selectedProductId)) {
      setSelectedProductId(productIds[0]);
    }
  }, [productIds, selectedProductId]);

  const validateWinLimits = (winLimitItems: WinLimitGroupItem[]) => {
    for (const item of winLimitItems) {
      const error = validateWinLimitItem(item);
      if (!error.isValid) {
        setValidateWinLimit(error);
        return error;
      }
    }
    return { id: '', isValid: true };
  };

  useImperativeHandle(ref, () => {
    const validate = () => {
      const validateResult = validateWinLimits(winLimitItems);
      if (!validateResult.isValid) {
        if (validateResult.productId) {
          setSelectedProductId(validateResult.productId);
        }
        blockRef.current?.expand();
      }
      if (validateResult.isValid) {
        setValidateWinLimit({ id: '', isValid: true });
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

  const showCurrentWinAmount = (sportType: number, betType: number) => {
    NiceModal.show(WinAmount, { sportType, betType });
  };

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
                  setTooltipError={setValidateWinLimit}
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
                                  showCurrentWinAmount(winLimit.sportTypeId, winLimit.betTypeId)
                                }
                                style={{ cursor: 'pointer', color: 'blue', display: 'inline-block' }}
                              >
                                {t('ClickToViewCurrentWinAmount')}
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
        {!validateWinLimit.isValid && (
          <WrapperTooltip
            type="error"
            anchorSelect={`#${validateWinLimit.id}`}
            isOpen={selectedProductId === validateWinLimit.productId && !validateWinLimit.isValid}
            content={t(validateWinLimit?.message ?? '', validateWinLimit?.msgParams)}
          />
        )}
      </Block>
    </div>
  );
});

function Note() {
  const { t } = useTranslation();
  const { url } = CustomerSettingInformationPageUrl({ tabName: TabNames.WinLossLimitTab });

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
