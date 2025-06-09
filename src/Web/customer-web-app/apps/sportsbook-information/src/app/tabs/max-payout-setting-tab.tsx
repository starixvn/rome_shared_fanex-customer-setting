import axios from 'axios';
import { useEffect, useState } from 'react';
import { Trans } from 'react-i18next';

import styles from '../app.module.scss';

interface MaxPayoutSport {
  bettypeName: string[];
  sportName: string;
}

type MaxPayoutSettingData = {
  maxPayoutSport: MaxPayoutSport;
};

export const MaxPayoutSettingTab = ({ url}: { url: string}) => {
  const [data, setData] = useState<MaxPayoutSettingData>({} as MaxPayoutSettingData);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async (url: string) => {
      const response = await axios.get<MaxPayoutSettingData>(url);
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

  return (
    <div>
      <div>
        <div>
          <strong>
            <Trans i18nKey={'MPSInformation'} />{' '}
          </strong>
        </div>
        <div>
          <Trans i18nKey={'MPS1'} />
        </div>
        <div>
          <Trans i18nKey={'MPS2'} />
        </div>
        <div>
          <div>
            <Trans i18nKey={'MPS3'} />
          </div>
          <div>
            <Trans i18nKey={'MPS4'} />
            <div>
              <Trans i18nKey={'MPS5'} />
            </div>
            <div>
              <Trans i18nKey={'MPS6'} />
            </div>
          </div>
          <div>
            <Trans i18nKey={'MPS7'} />
            <div>
              <Trans i18nKey={'MPS8'} />
            </div>
          </div>
        </div>
        <br />
        <div>
          <Trans i18nKey={'MPS9'} />
        </div>
        <br />
        <div>
          <Trans i18nKey={'MPS10'} />
          <SecondTable data={data.maxPayoutSport} />
        </div>
        <br />
        <div>
          <strong>
            <Trans i18nKey={'MPS18'} />
          </strong>
        </div>
        <div>
          <Trans i18nKey={'MPS19'} />
        </div>
        <div>
          <Trans i18nKey={'MPS2'} />
        </div>
        <div>
          <Trans i18nKey={'MPS20'} />
        </div>
        <div>
          <Trans i18nKey={'MPS21'} />
        </div>

        <div>
          <Trans i18nKey={'MPS22'} />
        </div>
        <div>
          <Trans i18nKey={'MPS23'} />
        </div>
        <div>
          <Trans i18nKey={'MPS24'} />
        </div>
        <br />

        <div>
          <Trans i18nKey={'MPS25'} />
        </div>
        <br />
        <div>
          <Trans i18nKey={'MPS26'} />
        </div>
        <br />
        <div>
          <Trans i18nKey={'MPS27'} />
        </div>
        <br />
        <div>
          <Trans i18nKey={'MPS17'} />
        </div>
        <div>
          <strong>
            <Trans i18nKey={'MPS28'} />
          </strong>
        </div>
      </div>
    </div>
  );
};

const SecondTable = ({ data }: { data: MaxPayoutSport }) => {
  return (
    <table className={styles.tableaccordion}>
      <thead>
        <tr>
          <th>
            <Trans i18nKey={'MPS11'} />
          </th>
          <th>
            <Trans i18nKey={'MPS12'} />
          </th>
        </tr>
      </thead>
      <tbody>
        {data?.bettypeName.map((betType: any, index: number) => {
          return (
            <tr key={`${betType}-${index}`}>
              {index === 0 && (
                <td rowSpan={data?.bettypeName.length} className="text-left bgGrey">
                  <Trans i18nKey={`${data?.sportName}`} />
                </td>
              )}
              <td className="text-left">{betType}</td>
            </tr>
          );
        })}
      </tbody>
    </table>
  );
};
