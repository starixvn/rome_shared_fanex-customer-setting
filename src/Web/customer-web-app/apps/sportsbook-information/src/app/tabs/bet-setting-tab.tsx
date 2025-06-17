import React from 'react';
import { Trans } from 'react-i18next';
import { useEffect, useState } from 'react';
import axios from 'axios';

import styles from '../app.module.scss';

import {i7Markets, i7Sites} from "@customer-web-app/shared-constants";

interface SabaSport {
  settingName: string;
  settingFollowedName: string;
  rowSpan: number;
}

type BetSettingData = {
  sabaSport: SabaSport[];
  isSpecialPlatform: boolean;
};

const FistTable: React.FC<{ data: SabaSport[], isSpecialPlatform: boolean }> = ({ data, isSpecialPlatform }) => {
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
            <Trans i18nKey={isSpecialPlatform ? 'VS_Header' : 'SabaTable_Header1'} />
          </th>
          <th>
            <Trans i18nKey={'BetSetting'} />
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

const MBCMarketBetSetting = () => {
  const SMALevelRoles = [5, 4, 3, 2];
  const SMABetSetting = () => {
    return (
      <>
        <ul>
          <li>
            <Trans i18nKey="DoNotSetLimitBetSetting" />
          </li>
          <li>
            <Trans i18nKey="PreventMembersPlayGame" />
          </li>
          <li>
            <Trans i18nKey="UpdateUplineBetSettingNote" />
          </li>
        </ul>
      </>
    );
  };
  const MemberBetSetting = () => {
    return (
      <>
        <ul>
          <li>
            <Trans i18nKey="MemberNote" />
          </li>
        </ul>
      </>
    );
  };
  return (
    <div> {SMALevelRoles.includes(informationPageParams?.roleId || 1) ? <SMABetSetting /> : <MemberBetSetting />}</div>
  );
};

const I7MarketBetSetting = () => {
  const SMABetSetting = () => {
    return (
      <>
        <div>
          <Trans i18nKey="UpdateUplineBetSettingNote" />
        </div>
        <div>
          <Trans i18nKey="ForSportsBookOnly" />:
        </div>
        <ul>
          <li>
            <Trans i18nKey="DoNotSetLimitBetSetting" />
          </li>
          <li>
            <Trans i18nKey="PreventMembersPlayGame" />
          </li>
          <li>
            <Trans i18nKey="ESportMuayThaiBetSetting" />
          </li>
        </ul>
      </>
    );
  };
  const MemberBetSetting = () => {
    return (
      <>
        <div>
          <Trans i18nKey="ForSportsBookOnly" />:
        </div>
        <ul>
          <li>
            <Trans i18nKey="ESportMuayThaiBetSetting" />
          </li>
          <li>
            <Trans i18nKey="MemberNote" />
          </li>
        </ul>
      </>
    );
  };
  const SMALevelRoles = [5, 4, 3, 2];
  return (
    <div> {SMALevelRoles.includes(informationPageParams?.roleId || 1) ? <SMABetSetting /> : <MemberBetSetting />}</div>
  );
};

export const BetSettingTab = ({ url}: { url: string}) => {
  const SpecialMarket = i7Markets.includes(informationPageParams.site ?? "") || i7Sites.includes(informationPageParams?.siteLabel ?? "");
  const [data, setData] = useState<BetSettingData>({} as BetSettingData);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async (url: string) => {
      const response = await axios.get<BetSettingData>(url);
      return response.data;
    };
    fetchData(url).then((data) => {
      setData(data);
      setLoading(false);
    });
  }, [url]);

  if (loading) {
    return <div>{SpecialMarket ? <I7MarketBetSetting /> : <MBCMarketBetSetting />}</div>;
  }

  return (
    <div>
      {SpecialMarket ? <I7MarketBetSetting /> : <MBCMarketBetSetting />}
      <br />
      <div>
        <strong>
          <Trans i18nKey={data.isSpecialPlatform ? 'MPS_VSCaption' : 'MPS_SabaCaption'} />
        </strong>
        <div>
          <Trans i18nKey={data.isSpecialPlatform ? 'MPS_VSDetails' : 'MPS_SabaDetails'} />
        </div>
        <FistTable data={data.sabaSport} isSpecialPlatform={data.isSpecialPlatform} />
      </div>
    </div>
  );
};
