import axios from 'axios';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { BlockV2 } from '@customer-web-app/shared/base/block';

import styles from '../app.module.scss';
import { HighlightFrame } from '../highlight-frame';

// naming by api response
type PtSettingData = {
  bettypeName: string;
  bettypes: { typeId: number; displayTypeName: string }[];
}[];

export const PtSettingTab = ({ alphaUrl, napUrl, exNapUrl }: { alphaUrl: string; napUrl: string, exNapUrl: string }) => {
  const { t } = useTranslation();
  const [data, setData] = useState<PtSettingData>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async (url: string) => {
      const response = await axios.get<PtSettingData>(url);
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
          <b>{t('LiteModePT')}</b> {/*chưa dịch*/}
          <div>{t('CommonPTSettingNote1')}</div>
          <div>{t('PTSettingNote2')}</div>
          <b>{t('PTSettingInformation')}</b>
          <div>{t('PTSettingInformationContent')}</div>
      </div>

      <div className="table">
        {data?.map((item, index: number) => (
          <BlockV2 title={item.bettypeName} isCollapse={index !== 0} key={`${item.bettypeName}-${index}`}>
            <table className={styles.tableaccordion}>
              <thead>
                <tr>
                  <th>{t('BetType')}</th>
                  <th>{t('FollowedBy')}</th>
                </tr>
              </thead>
              <tbody>
                {item.bettypes.map((betType, index: number) => (
                  <tr key={`${item.bettypeName}-${index}`}>
                    {index === 0 && (
                      <td rowSpan={item.bettypes.length} className="text-center bgGrey vertical-middle">
                        {item.bettypeName}
                      </td>
                    )}
                    <td className="text-left">{betType.displayTypeName}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </BlockV2>
        ))}
      </div>
    </div>
  );
};
