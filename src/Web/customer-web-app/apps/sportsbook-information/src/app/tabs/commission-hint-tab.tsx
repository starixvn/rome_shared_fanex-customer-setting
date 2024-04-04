import axios from 'axios';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { BlockV2 } from '@customer-web-app/shared/base/block';

import styles from '../app.module.scss';
import { HighlightFrame } from '../highlight-frame';

// naming by api response
type CommissionHintData = {
  commissionTypeList: { commissionName: string; commissionId: number; bettypes: { name: string; id: number }[] }[];
  myanmarOddsBetTypesName: string;
};

export const CommissionHintTab = ({ alphaUrl, napUrl, exNapUrl }: { alphaUrl: string; napUrl: string, exNapUrl: string }) => {
  const { t } = useTranslation();
  const [data, setData] = useState<CommissionHintData>({} as CommissionHintData);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async (url: string) => {
      const response = await axios.get<CommissionHintData>(url);
      return response.data;
    };
    fetchData(alphaUrl || napUrl || exNapUrl).then((data) => {
      setData(data);
      setLoading(false);
    });
  }, [alphaUrl, napUrl]);

  if (loading) {
    return <div />;
  }

  return (
    <div>
      <HighlightFrame header={t('Note')} content={t('NoteContent')} />
      <div className={styles.info}>

        <div>
          <b>{t('OtherProductsCommissionInformation')}</b>
        </div>
        <div>{t('BBINBetSetting2')}</div>
        <div>{t('FGGNote')}</div>

        <div>
          <b>{t('SportsBookCommissionInformation')}</b>
        </div>
        <div>{t('CommissionMap')}</div>
      </div>

      <div className="table">
        {data.commissionTypeList?.map((item, index: number) => (
          <BlockV2 title={item.commissionName} isCollapse={index !== 0} key={`${item.commissionName}-${index}`}>
            <table className={styles.tableaccordion}>
              <thead>
                <tr>
                  <th>{t('CommissionGroup')}</th>
                  <th>{t('BetType')}</th>
                </tr>
              </thead>
              <tbody>
                {item.bettypes.map((betType, index: number) => (
                  <tr key={`${item.commissionName}-${index}`}>
                    {index === 0 && (
                      <td rowSpan={item.bettypes.length} className="text-center bgGrey vertical-middle">
                        {item.commissionName}
                      </td>
                    )}
                    <td className="text-left">{betType.name}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </BlockV2>
        ))}
        <BlockV2 title={t('Commission3')} />
      </div>
    </div>
  );
};
