import { Trans } from 'react-i18next';

import {i7Markets, i7Sites} from "@customer-web-app/shared-constants";

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

export const BetSettingTab = () => {
  const SpecialMarket = i7Markets.includes(informationPageParams.site ?? "") || i7Sites.includes(informationPageParams?.siteLabel ?? "");

  return <div>{SpecialMarket ? <I7MarketBetSetting /> : <MBCMarketBetSetting />}</div>;
};
