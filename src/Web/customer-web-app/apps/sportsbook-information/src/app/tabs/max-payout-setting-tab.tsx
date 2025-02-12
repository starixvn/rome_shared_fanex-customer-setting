import axios from 'axios';
import { useEffect, useState } from 'react';
import { T } from '@tolgee/react';

import styles from '../app.module.scss';

interface MaxPayoutSport {
  bettypeName: string[];
  sportName: string;
}
interface SabaSport {
  settingName: string;
  settingFollowedName: string;
  rowSpan: number;
}

type MaxPayoutSettingData = {
  maxPayoutSport: MaxPayoutSport;
  sabaSport: SabaSport[];
  isSpecialPlatform: boolean;
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
            <T keyName={'MPSInformation'} />{' '}
          </strong>
        </div>
        <div>
          <T keyName={'MPS1'} />
        </div>
        <div>
          <T keyName={'MPS2'} />
        </div>
        <div>
          <div>
            <T keyName={'MPS3'} />
          </div>
          <div>
            <T keyName={'MPS4'} />
            <div>
              <T keyName={'MPS5'} />
            </div>
            <div>
              <T keyName={'MPS6'} />
            </div>
          </div>
          <div>
            <T keyName={'MPS7'} />
            <div>
              <T keyName={'MPS8'} />
            </div>
          </div>
        </div>
        <br />
        <div>
          <T keyName={'MPS9'} />
        </div>
        <br />
        <div>
          <strong>
            <T keyName={data.isSpecialPlatform ? 'MPS_VSCaption' : 'MPS_SabaCaption'} />
          </strong>
          <div>
            <T keyName={data.isSpecialPlatform ? 'MPS_VSDetails' : 'MPS_SabaDetails'} />
          </div>
          <FistTable data={data.sabaSport} isSpecialPlatform={data.isSpecialPlatform}  />
        </div>
        <br />
        <div>
          <T keyName={'MPS10'} />
          <SecondTable data={data.maxPayoutSport} />
        </div>
        <br />
        <div>
          <strong>
            <T keyName={'MPS18'} />
          </strong>
        </div>
        <div>
          <T keyName={'MPS19'} />
        </div>
        <div>
          <T keyName={'MPS2'} />
        </div>
        <div>
          <T keyName={'MPS20'} />
        </div>
        <div>
          <T keyName={'MPS21'} />
        </div>

        <div>
          <T keyName={'MPS22'} />
        </div>
        <div>
          <T keyName={'MPS23'} />
        </div>
        <div>
          <T keyName={'MPS24'} />
        </div>
        <br />

        <div>
          <T keyName={'MPS25'} />
        </div>
        <br />
        <div>
          <T keyName={'MPS26'} />
        </div>
        <br />
        <div>
          <T keyName={'MPS27'} />
        </div>
        <br />
        <div>
          <T keyName={'MPS17'} />
        </div>
        <div>
          <strong>
            <T keyName={'MPS28'} />
          </strong>
        </div>
      </div>
    </div>
  );
};

const FistTable = ({ data, isSpecialPlatform }: { data: SabaSport[], isSpecialPlatform: boolean }) => {
  const removeDuplicateSettingFollowedName = data?.map((e, index) => {
    if (e.rowSpan === undefined) {
      e.rowSpan = 1;
    }
    for (let i = index + 1; i < data.length; i++) {
      if (e.settingFollowedName !== '' && e.settingFollowedName.toLowerCase() === data[i].settingFollowedName.toLowerCase()) {
        e.rowSpan = e.rowSpan + 1;
        data[i].settingFollowedName = '';
      }
    }
    return e;
  });

  return (
    <table className={styles.tableaccordion}>
      <thead>
        <tr>
          <th>
            <T keyName={'BetSettingsFollowed'} />
          </th>
          <th>
            <T keyName={isSpecialPlatform ? 'VS_Header' : 'SabaTable_Header1'} />
          </th>
        </tr>
      </thead>
      <tbody>
        {removeDuplicateSettingFollowedName.map((e, index) => (
          <tr key={`${e.settingName}-${index}`}>
            <td className="text-left">{e.settingName}</td>
            {e.settingFollowedName && (
              <td rowSpan={e.rowSpan} className="bgGrey text-left">
                {e.settingFollowedName}
              </td>
            )}
          </tr>
        ))}
      </tbody>
    </table>
  );
};
const SecondTable = ({ data }: { data: MaxPayoutSport }) => {
  return (
    <table className={styles.tableaccordion}>
      <thead>
        <tr>
          <th>
            <T keyName={'MPS11'} />
          </th>
          <th>
            <T keyName={'MPS12'} />
          </th>
        </tr>
      </thead>
      <tbody>
        {data?.bettypeName.map((betType: any, index: number) => {
          return (
            <tr key={`${betType}-${index}`}>
              {index === 0 && (
                <td rowSpan={data?.bettypeName.length} className="text-left bgGrey">
                  <T keyName={`${data?.sportName}`} />
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
