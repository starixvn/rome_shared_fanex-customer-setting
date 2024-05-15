import { useTranslation } from 'react-i18next';

import { GroupTypeIds } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { getSettingItems } from '@customer-web-app/shared-group-settings-state';
import { ProductStatusGroupItem } from '@customer-web-app/shared-group-settings-types';
import { Block, BlockItemGroup } from '@customer-web-app/shared/base/block';

import { GroupSettingBlockProps } from '../types';
import { ProductStatusItem } from './product-status-item';

export default function ProductStatus({ isReadOnly }: Readonly<GroupSettingBlockProps>) {
  const { t } = useTranslation();
  const { productGroups, productGroupId, handleUpdateSettingItems, updateGroupSetting, isEditMode } =
    useGroupSettingContext();
  const groupId = Number(productGroupId);
  const productStatusItems = getSettingItems<ProductStatusGroupItem>(
    productGroups,
    groupId,
    GroupTypeIds.ProductStatus
  );

  const onChange = (sportTypeId: number, betTypeId: number, isDisabled: boolean) => {
    const newProductStatusItems = productStatusItems.map((item) => {
      if (item.sportTypeId === sportTypeId && item.betTypeId === betTypeId) {
        return { ...item, isDisabled };
      }

      if (item.parentSportType === sportTypeId && item.parentBetType === betTypeId) {
        if (isDisabled === true) {
          return { ...item, readonly: true, isDisabled };
        } else {
          return { ...item, readonly: false };
        }
      }

      return item;
    });

    handleUpdateSettingItems?.(groupId, GroupTypeIds.ProductStatus, newProductStatusItems);
  };

  const itemCounts = productStatusItems.length;
  const numberOfRow = itemCounts % 2 === 0 ? itemCounts / 2 : Math.floor(itemCounts / 2) + 1;
  const renderItems = () => {
    const rows = [];
    for (let i = 0; i < numberOfRow; i++) {
      rows.push(
        <BlockItemGroup key={i}>
          <ProductStatusItem isReadonly={isReadOnly} data={productStatusItems[2 * i]} onChange={onChange} />
          <ProductStatusItem isReadonly={isReadOnly} data={productStatusItems[2 * i + 1]} onChange={onChange} />
        </BlockItemGroup>
      );
    }

    return rows;
  };

  return (
    <Block
      isPartialUpdate={isEditMode === true && !isReadOnly}
      title={t('ProductStatus')}
      onSaveClick={() => updateGroupSetting?.(GroupTypeIds.ProductStatus)}
    >
      {renderItems()}
    </Block>
  );
}
