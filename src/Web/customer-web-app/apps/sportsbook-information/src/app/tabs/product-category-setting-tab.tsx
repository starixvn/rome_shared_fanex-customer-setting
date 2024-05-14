import axios from 'axios';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { BlockV2 } from '@customer-web-app/shared/base/block';

interface ProductSetting {
  id: number;
  name: string;
  products: ProductSetting[];
}


type ProductSettingInfoResponse = {
  productSettingInfo: ProductSetting[];
  roleId: number;
  is789y: boolean;
  showBBin: boolean;
  showRNGKenoNumberGame: boolean;
};

export function ProductCategorySettingTab({ url }: { url: string }) {
  const [data, setData] = useState<ProductSettingInfoResponse>({} as ProductSettingInfoResponse);
  const { t } = useTranslation();
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async (url: string) => {
      const response = await axios.get<ProductSettingInfoResponse>(url);
      return response.data;
    };
    fetchData(url).then((data) => {
      setData(data);
      setLoading(false);
    });
  }, [url]);

  if (loading) {
    return <div />;
  }

  const GetProductName = (product: ProductSetting): string => {
    if (product?.products?.length > 0) {
      return product?.products.map((p) => GetProductName(p)).join(', ');
    }

    return product.name;
  };

  const CheckProductsNotNull = (product: ProductSetting) => product?.products?.length > 1;

  const sortAZ = (a: string, b: string) => a.localeCompare(b, undefined, { sensitivity: 'base' });

  const renderMultiple = (products: ProductSetting[], groupName: string, numberOfGroup: number) =>
    products.map((p) => (
      <tr key={p.name}>
        {(groupName !== p.name || numberOfGroup !== 1) && p.name && <td>{p.name}</td>}
        <td>
          {p.products
            .map((product: ProductSetting) => GetProductName(product))
            .sort(sortAZ)
            .join(', ')}
        </td>
      </tr>
    ));

  return (
    <div>
      <div>
        <b>{t('ProductSettingInformation')}</b>
      </div>
      <div>{t('ProductGroupTableIntro')}</div>
      <br />
      {data?.productSettingInfo.map((e, index) => (
        <BlockV2 key={e.name + index.toString()} isCollapse={false} title={e.name} colorPrimary>
          <table>
            <tbody>
              {e.products?.map((p) => CheckProductsNotNull(p)).includes(true) ? (
                renderMultiple(e.products, e.name, e.products.length)
              ) : (
                <tr>
                  <td>
                    {e.products
                      .map((product: ProductSetting) => GetProductName(product))
                      .sort(sortAZ)
                      .join(', ')}
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </BlockV2>
      ))}
    </div>
  );
}
