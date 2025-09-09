import { Trans } from 'react-i18next';

export const MaxWinSettingTab = ({ url }: { url: string }) => {
  return (
    <div>
      <div><strong><Trans i18nKey="MaxWinSettingInformation" /></strong></div>
      <div><Trans i18nKey="MaxWinSettingInformationContent_0" components={{ br: <br />, i: <i /> }} /></div>
      <div><Trans i18nKey="MaxWinSettingInformationContent_1" components={{ br: <br />, i: <i /> }} /></div>
      <div><Trans i18nKey="MaxWinSettingInformationContent_2" components={{ br: <br />, i: <i /> }} /></div>
      <br />
      <div><Trans i18nKey="MaxWinSettingInformationContent_3" components={{ br: <br />, i: <i /> }} /></div>
      <div><Trans i18nKey="MaxWinSettingInformationContent_4" components={{ br: <br />, i: <i /> }} /></div>
      <div><Trans i18nKey="MaxWinSettingInformationContent_5" components={{ br: <br />, i: <i /> }} /></div>
      <div><Trans i18nKey="MaxWinSettingInformationContent_6" components={{ br: <br />, i: <i /> }} /></div>
    </div>
  );
};
