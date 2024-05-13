import { Trans } from 'react-i18next';
export const WinLossLimitTab = () => {
  return (
    <ul>
      <li><Trans i18nKey="RNGGames"/>: <Trans i18nKey="RNGKenoNumberGameWinLossLimitNote"/></li>
      <li><Trans i18nKey="CommonWinLossLimitNote"/></li>
      <li><Trans i18nKey="WinLossLimitNote1"/></li>
      <li><Trans i18nKey="WinLossLimitNote2"/></li>
    </ul>
  );
};
