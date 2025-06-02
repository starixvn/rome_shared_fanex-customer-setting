import axios from 'axios';
import { useEffect, useState } from 'react';
import { Trans } from 'react-i18next';


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

const MaxWinSetting = ({ isCashMember }: { isCashMember: boolean | undefined }) => {
  if (isCashMember) {
    return (
      <div>
        <div><Trans i18nKey="MaxWinCashMember" /></div>
        <div><Trans i18nKey="MaxWinCashMemberAllowedRange" /></div>
        <div><Trans i18nKey="MaxWinCashMemberExample" /></div>
      </div>
    )
  }
  else {
    return (
      <div>
        <div><Trans i18nKey="MaxWin" /></div>
        <div><Trans i18nKey="MaxWinExample" /></div>
      </div>
    )
  }
}

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
    }).finally(() => {
      setLoading(false);
    });
  }, [url]);

  function getRNGProductNames() {
    if (!data.productSettingInfo || data.productSettingInfo.length === 0) {
      return '';
    }

    const virtualSportsId = 5;
    var ps = data.productSettingInfo[0].products[0].products.filter((p: any) => p.id !== virtualSportsId);
    var names = ps.map((p: any) => p.name).join(", ");
    return ` (${names})`;
  }

  if (loading) {
    return <div />;
  }

  return (
    <ul>
      <li><Trans i18nKey="MaxWinLoss" /></li>
      <li><MaxWinSetting isCashMember={informationPageParams.isCashMember} /></li>
      <li><Trans i18nKey="MaxLoss"/></li>
      <li><Trans i18nKey="CommonWinLossLimitNote"/></li>
      <li><Trans i18nKey="RNGGames"/>{getRNGProductNames()}: <Trans i18nKey="RNGKenoNumberGameWinLossLimitNote"/></li>
      <li><Trans i18nKey="WinLossLimitNote1"/></li>
      <li><Trans i18nKey="WinLossLimitNote2"/></li>
    </ul>
  );
};
