import axios from 'axios';
import { useEffect, useState } from 'react';
import { T } from '@tolgee/react';


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

export const WinLossLimitTab = ({ url }: { url: string }) => {
  const [data, setData] = useState<ProductSettingInfoResponse>({} as ProductSettingInfoResponse);
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

  function getRNGProductNames() {
    if(!data.productSettingInfo || data.productSettingInfo.length === 0) {
      return '';
    }

    const virtualSportsId = 5;
    var ps = data.productSettingInfo[0].products[0].products.filter((p: any) => p.id !== virtualSportsId);
    var names = ps.map((p:any) => p.name).join(", ");
    return ` (${names})`;
  }

  return (
    <ul>
      <li><T keyName="MaxWinLoss"/></li>
      <li><T keyName="MaxWin"/></li>
      <li><T keyName="MaxLoss"/></li>
      <li><T keyName="CommonWinLossLimitNote"/></li>
      <li><T keyName="RNGGames"/>{getRNGProductNames()}: <T keyName="RNGKenoNumberGameWinLossLimitNote"/></li>
      <li><T keyName="WinLossLimitNote1"/></li>
      <li><T keyName="WinLossLimitNote2"/></li>
    </ul>
  );
};
