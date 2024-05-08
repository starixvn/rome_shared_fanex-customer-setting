import cn from 'classnames';
import { forwardRef, useEffect, useImperativeHandle, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { ValidationRefType } from '@customer-web-app/share-models';
import { GroupTypeIds, ProductIds, UserLevel } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import { BetLimitGroupItem } from '@customer-web-app/shared-group-settings-types';
import { scrollToBLock } from '@customer-web-app/shared-group-settings-ui';
import { Block, BlockColumn, BlockRefType, BlockRow } from '@customer-web-app/shared/base/block';
import { WrapperTooltip } from '@customer-web-app/wrapper-tooltip';

import { renderNoneBlockColumn } from '../position-taking/utils';
import { GroupSettingBlockProps } from '../types';

const MaxBetLimitColumns = 6;
const StringToPreventAutoSortReduce = 'PreventAutoSort';

const groupBetLimitByMultipleChoice = (betLimits: BetLimitGroupItem[]) => {
  return betLimits.reduce((acc: { [key: string]: BetLimitGroupItem[] }, cur) => {
    acc[cur.isMultipleChoice.toString()] = acc[cur.isMultipleChoice.toString()] ?? [];
    acc[cur.isMultipleChoice.toString()].push(cur);
    return acc;
  }, {});
};

export const groupBetLimitBySportTypeId = (betLimits: BetLimitGroupItem[]) => {
  return betLimits.reduce((acc: { [key: string]: BetLimitGroupItem[] }, cur) => {
    const key = `${StringToPreventAutoSortReduce}-${cur.sportTypeId}`;
    acc[key] = acc[key] ?? [];
    acc[key].push(cur);
    return acc;
  }, {});
};

const validateBetLimits = (betLimits: BetLimitGroupItem[], isAllowUncheckAll: boolean) => {
  const betLimitBySportType = groupBetLimitBySportTypeId(betLimits);
  for (const item of Object.values(betLimitBySportType)) {
    if (item.every((i) => !i.isChecked) && !isAllowUncheckAll) {
      return {
        id: `name-${item[0].productId}-${item[0].sportTypeId}-${item[0].stakeLevelId}`,
        productId: item[0].productId,
        isValid: false,
        message: 'StakeLevelRequired',
      };
    }
  }

  return { id: '', isValid: true };
};

type BetLimitRowProps = {
  item: BetLimitGroupItem[];
  isAllowUncheckAll: boolean;
  isReadOnly: boolean;
  handleOnBetLimitChange: (item: BetLimitGroupItem, isChecked: boolean, isCheckNone?: boolean) => void;
  handleCheckAllItem: (item: BetLimitGroupItem, isChecked: boolean) => void;
};

function BetLimitRow({
  item,
  isAllowUncheckAll,
  isReadOnly,
  handleOnBetLimitChange,
  handleCheckAllItem,
}: Readonly<BetLimitRowProps>) {
  const { t } = useTranslation();

  if (item.length < 1) return null;

  const firstItem = item[0];
  const isShowNoneInput =
    params.roleId === UserLevel.Member &&
    isAllowUncheckAll &&
    !firstItem.isMultipleChoice &&
    firstItem.productId === ProductIds.LiveCasino;
  const numberEmptyBlockColumn = MaxBetLimitColumns - item.length - (isShowNoneInput ? 1 : 0);
  return (
    <BlockRow
      key={firstItem.sportTypeId}
      label={
        params.roleId !== UserLevel.Member ? (
          <div style={{ display: 'flex', justifyContent: 'space-between', width: '100%', alignItems: 'center' }}>
            <div>{firstItem.sportTypeName}</div>
            <input
              type="checkbox"
              disabled={isReadOnly}
              checked={!item.some((item) => !item.isChecked)}
              onChange={(e) => handleCheckAllItem(firstItem, e.target.checked)}
            />
          </div>
        ) : (
          <div style={{ display: 'flex', justifyContent: 'space-between', width: '100%' }}>
            <div>{firstItem.sportTypeName}</div>
            {item.length > firstItem.maxChoice && firstItem.maxChoice !== 0 && (
              <div>
                <span
                  className="ic-warning"
                  id="bet-limit-warning"
                  data-tooltip-content={t('MemberMaxStakeLevelCanBeSelect', {
                    name: firstItem.sportTypeName,
                    value: firstItem.maxChoice,
                  })}
                ></span>
              </div>
            )}
          </div>
        )
      }
    >
      {isShowNoneInput && (
        <BlockColumn>
          <input
            type="radio"
            checked={item.every((item) => !item.isChecked)}
            name={firstItem.sportTypeId.toString()}
            disabled={isReadOnly}
            onChange={(e) => handleOnBetLimitChange(firstItem, false, true)}
          ></input>
          <div>{t('None')}</div>
        </BlockColumn>
      )}
      {item.map((stakeLevel) => (
        <BlockColumn key={stakeLevel.stakeLevelId}>
          <div style={{ display: 'flex', alignItems: 'center', paddingLeft: '4px' }}>
            <div
              id={`name-${stakeLevel.productId}-${stakeLevel.sportTypeId}-${stakeLevel.stakeLevelId}`}
              className="blockLeft"
            >
              {params.roleId === UserLevel.Member && !stakeLevel.isMultipleChoice ? (
                <input
                  type="radio"
                  checked={stakeLevel.isChecked}
                  disabled={isReadOnly}
                  name={stakeLevel.sportTypeId.toString()}
                  onChange={(e) => handleOnBetLimitChange(stakeLevel, e.target.checked)}
                />
              ) : (
                <input
                  type="checkbox"
                  checked={stakeLevel.isChecked}
                  onChange={(e) => handleOnBetLimitChange(stakeLevel, e.target.checked)}
                  disabled={
                    (!stakeLevel.isChecked &&
                      params.roleId === UserLevel.Member &&
                      item.filter((i) => i.isChecked).length === stakeLevel.maxChoice &&
                      stakeLevel.maxChoice !== 0) ||
                    isReadOnly
                  }
                />
              )}

              <div>{stakeLevel.stakeLevelName}</div>
            </div>
          </div>
        </BlockColumn>
      ))}
      {renderNoneBlockColumn('betLimitColumn', numberEmptyBlockColumn)}
    </BlockRow>
  );
}

const BetLimitBlockId = 'BetLimitBlockId';

const BetLimit = forwardRef<ValidationRefType, GroupSettingBlockProps>(function BetLimit(props, ref) {
  const { t } = useTranslation();
  const { isReadOnly, isAllowUncheckAll } = props;
  const { productGroups, productGroupId, isEditMode, handleUpdateSettingItems, updateGroupSetting } =
    useGroupSettingContext();
  const groupId = Number(productGroupId);
  const betLimitItems = getSettingItems<BetLimitGroupItem>(productGroups, groupId, GroupTypeIds.BetLimit);
  const productIds = Array.from(new Set(betLimitItems.map((item) => item.productId)));

  const [selectedProductId, setSelectedProductId] = useState(productIds[0]);
  const selectedProductBetLimitItems = betLimitItems.filter((item) => item.productId === selectedProductId);
  const betLimitGroup = groupBetLimitByMultipleChoice(selectedProductBetLimitItems);
  const [validateBetLimitResult, setValidateBetLimitResult] = useState<{
    id: string;
    productId?: number;
    isValid: boolean;
    message?: string;
  }>({ id: '', isValid: true });
  const blockRef = useRef<BlockRefType>(null);

  useEffect(() => {
    if (!productIds.includes(selectedProductId)) {
      setSelectedProductId(productIds[0]);
    }
  }, [productIds, selectedProductId]);

  useEffect(() => {
    const validateResult = validateBetLimits(
      betLimitItems.filter((item) => item.productId === selectedProductId),
      !!isAllowUncheckAll
    );
    setValidateBetLimitResult(validateResult);
  }, [betLimitItems, selectedProductId, isAllowUncheckAll]);

  useImperativeHandle(ref, () => {
    const validate = () => {
      const validateResult = validateBetLimits(betLimitItems, !!isAllowUncheckAll);
      if (!validateResult.isValid) {
        if (validateResult.productId) setSelectedProductId(validateResult.productId);
        blockRef.current?.expand();
      }
      setValidateBetLimitResult(validateResult);
      return validateResult.isValid;
    };

    return {
      validate,
      scrollToBlock: () => scrollToBLock(BetLimitBlockId),
    };
  });

  const handleOnBetLimitChange = (item: BetLimitGroupItem, isChecked: boolean, isCheckNone = false) => {
    const newBetLimits = betLimitItems.map((i) => {
      if (i.sportTypeId === item.sportTypeId) {
        if (isCheckNone) {
          return { ...i, isChecked: false };
        }

        if (i.stakeLevelId === item.stakeLevelId) {
          return { ...i, isChecked };
        }

        if (params.roleId === UserLevel.Member && !item.isMultipleChoice && i.stakeLevelId !== item.stakeLevelId) {
          return { ...i, isChecked: false };
        }
      }

      return { ...i };
    });
    handleUpdateSettingItems?.(groupId, GroupTypeIds.BetLimit, newBetLimits);
  };

  const handleCheckAllItem = (item: BetLimitGroupItem, isChecked: boolean) => {
    const newBetLimits = betLimitItems.map((i) => {
      if (i.sportTypeId === item.sportTypeId) {
        return { ...i, isChecked };
      }
      return { ...i };
    });
    handleUpdateSettingItems?.(groupId, GroupTypeIds.BetLimit, newBetLimits);
  };

  const handleCheckAllRows = (isChecked: boolean) => {
    const newBetLimits = betLimitItems.map((i) => {
      if (i.productId === selectedProductId) {
        return { ...i, isChecked };
      }
      return { ...i };
    });
    handleUpdateSettingItems?.(groupId, GroupTypeIds.BetLimit, newBetLimits);
  };

  return (
    <div id={BetLimitBlockId}>
      <Block
        ref={blockRef}
        title={t('BetLimit')}
        isPartialUpdate={isEditMode === true && !isReadOnly}
        onSaveClick={() => updateGroupSetting?.(GroupTypeIds.BetLimit)}
      >
        <div className="nav-tab">
          {productIds.map((item) => (
            <button
              className={cn('item', { active: item === selectedProductId })}
              key={item}
              onClick={() => setSelectedProductId(item)}
            >
              {betLimitItems.find((i) => i.productId === item)?.productName}
            </button>
          ))}
        </div>
        {params.roleId !== UserLevel.Member && (
          <BlockRow
            label={
              <div style={{ display: 'flex', justifyContent: 'space-between', width: '100%', alignItems: 'center' }}>
                <div>{t('SelectAll')}</div>
                <input
                  type="checkbox"
                  disabled={isReadOnly}
                  checked={selectedProductBetLimitItems.every((item) => item.isChecked)}
                  onChange={(e) => handleCheckAllRows(e.target.checked)}
                />
              </div>
            }
          >
            {renderNoneBlockColumn('select-all-id', MaxBetLimitColumns)}
          </BlockRow>
        )}
        {Object.values(betLimitGroup).map((betLimitGroup, index) => {
          const groupBetLimitStyle = index === 0 ? { borderBottom: '1px solid #d1d1d1' } : { marginTop: '7px' };
          return (
            <div key={betLimitGroup[0].isMultipleChoice} style={groupBetLimitStyle}>
              {Object.values(groupBetLimitBySportTypeId(betLimitGroup)).map((item) => (
                <BetLimitRow
                  key={item[0].sportTypeId}
                  item={item}
                  isReadOnly={isReadOnly}
                  isAllowUncheckAll={!!isAllowUncheckAll}
                  handleOnBetLimitChange={handleOnBetLimitChange}
                  handleCheckAllItem={handleCheckAllItem}
                />
              ))}
            </div>
          );
        })}
        {!validateBetLimitResult.isValid && (
          <WrapperTooltip
            type="error"
            anchorSelect={`#${validateBetLimitResult.id}`}
            isOpen={selectedProductId === validateBetLimitResult.productId && !validateBetLimitResult.isValid}
            content={t(validateBetLimitResult?.message ?? '')}
          />
        )}
      </Block>
      <WrapperTooltip type="warning" anchorSelect="#bet-limit-warning" />
    </div>
  );
});

export default BetLimit;
