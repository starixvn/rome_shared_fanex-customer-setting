import { renderToStaticMarkup } from 'react-dom/server';
import { useTranslation } from 'react-i18next';

export default function SportsBookBetSettingMixSportParlayNote() {
  const { t } = useTranslation();
  const tooltipContent = () => (
    <>
      <div>
        <b>{`${t('MixSportParlay')}:`} </b>
        {t('MixSportParlayHint1')}
      </div>
      <div>{t('MixSportParlayHint2')}</div>
    </>
  );

  return (
    <span
      className="icon ic-info"
      style={{ position: 'absolute', top: '2px', fontSize: '17px', right: '0', margin: '0' }}
      data-tooltip-id="global-tooltip"
      data-tooltip-html={renderToStaticMarkup(tooltipContent())}
    />
  );
}
