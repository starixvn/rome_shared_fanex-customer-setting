import { Trans } from 'react-i18next';
export const MaxPayoutSettingTab = ({ url }: { url: string }) => {
  return (
    <div>
      <div>
        <div><Trans i18nKey={'MPS19'} /></div>
        <div><Trans i18nKey={'MPS2'} /></div>
        <div><Trans i18nKey={'MPS20'} /></div>
        <div><Trans i18nKey={'MPS21'} /></div>

        <div><Trans i18nKey={'MPS22'} /></div>
        <div><Trans i18nKey={'MPS23'} /></div>
        <div><Trans i18nKey={'MPS24'} /></div>
        <div><Trans i18nKey={'MPS24_1'} components={{ sup: <sup /> }} /></div>
        <div><Trans i18nKey={'MPS24_2'} components={{ sup: <sup /> }} /></div>
        <br />
      </div>
    </div>
  );
};