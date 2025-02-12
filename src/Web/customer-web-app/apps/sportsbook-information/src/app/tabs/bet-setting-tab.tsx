import { T } from '@tolgee/react';

import {i7Markets, i7Sites} from "@customer-web-app/shared-constants";

const MBCMarketBetSetting = () => {
  const SMALevelRoles = [5, 4, 3, 2];
  const SMABetSetting = () => {
    return (
      <>
        <ul>
          <li>
            <T keyName="DoNotSetLimitBetSetting" />
          </li>
          <li>
            <T keyName="PreventMembersPlayGame" />
          </li>
          <li>
            <T keyName="UpdateUplineBetSettingNote" />
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
            <T keyName="MemberNote" />
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
          <T keyName="UpdateUplineBetSettingNote" />
        </div>
        <div>
          <T keyName="ForSportsBookOnly" />:
        </div>
        <ul>
          <li>
            <T keyName="DoNotSetLimitBetSetting" />
          </li>
          <li>
            <T keyName="PreventMembersPlayGame" />
          </li>
          <li>
            <T keyName="ESportMuayThaiBetSetting" />
          </li>
        </ul>
      </>
    );
  };
  const MemberBetSetting = () => {
    return (
      <>
        <div>
          <T keyName="ForSportsBookOnly" />:
        </div>
        <ul>
          <li>
            <T keyName="ESportMuayThaiBetSetting" />
          </li>
          <li>
            <T keyName="MemberNote" />
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

export const BetSettingTab = () => {
  const SpecialMarket = i7Markets.includes(informationPageParams.site ?? "") || i7Sites.includes(informationPageParams?.siteLabel ?? "");

  return <div>{SpecialMarket ? <I7MarketBetSetting /> : <MBCMarketBetSetting />}</div>;
};
