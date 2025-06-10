import { Trans } from 'react-i18next';

const PotentialWinLimitOverview = () => {
  return (
    <div>
      <strong><Trans i18nKey="sportsbook.potentialWinLimit.overview" /></strong>
      <ul>
        <li><Trans i18nKey="sportsbook.potentialWinLimit.description" /></li>
        <li><Trans i18nKey="sportsbook.potentialWinLimit.appliesTo" /></li>
        <li><Trans i18nKey="sportsbook.potentialWinLimit.excludes" /></li>
      </ul>
    </div>
    
  );
};

const CalculationLogic = () => {
  return (
    <div>
      <strong><Trans i18nKey="sportsbook.potentialWinLimit.calculationLogic.title" /></strong>
      <ul>
        <li><Trans i18nKey="sportsbook.potentialWinLimit.calculationLogic.formula" /></li>
        <li><Trans i18nKey="sportsbook.potentialWinLimit.calculationLogic.accumulated" /></li>
      </ul>
      <div>
        <Trans i18nKey="sportsbook.potentialWinLimit.calculationLogic.example" />:
        <ul>
          <li><Trans i18nKey="sportsbook.potentialWinLimit.calculationLogic.example-limit" /></li>
          <li><Trans i18nKey="sportsbook.potentialWinLimit.calculationLogic.example-ticketA" /></li>
          <li><Trans i18nKey="sportsbook.potentialWinLimit.calculationLogic.example-ticketB" /></li>
        </ul>
      </div>
    </div>
  );
};

const TrackingStart = () => {
  return (
    <div>
      <strong><Trans i18nKey="sportsbook.potentialWinLimit.trackingStart.title" /></strong>
      <ul>
        <li><Trans i18nKey="sportsbook.potentialWinLimit.trackingStart.accumulation" /></li>
        <li><Trans i18nKey="sportsbook.potentialWinLimit.trackingStart.included" /></li>
      </ul>
      <div>
        <Trans i18nKey="sportsbook.potentialWinLimit.trackingStart.example" />:
        <ul>
          <li><Trans i18nKey="sportsbook.potentialWinLimit.trackingStart.example-limit" /></li>
          <li><Trans i18nKey="sportsbook.potentialWinLimit.trackingStart.example-ticketA" /></li>
          <li><Trans i18nKey="sportsbook.potentialWinLimit.trackingStart.example-ticketB" /></li>
          <li><Trans i18nKey="sportsbook.potentialWinLimit.trackingStart.example-ticketC" /></li>
        </ul>
      </div>
    </div>
  );
};

const ResetBehavior = () => {
  return (
    <div>
      <strong><Trans i18nKey="sportsbook.potentialWinLimit.resetBehavior.title" /></strong>
      <ul>
        <li><Trans i18nKey="sportsbook.potentialWinLimit.resetBehavior.dailyReset" /></li>
        <li><Trans i18nKey="sportsbook.potentialWinLimit.resetBehavior.manualReset" /></li>
        <li><Trans i18nKey="sportsbook.potentialWinLimit.resetBehavior.runningTickets" /></li>
      </ul>
    </div>
  );
};

const ResultAmendmentImpact = () => {
  return (
    <div>
      <strong><Trans i18nKey="sportsbook.potentialWinLimit.resultAmendment.title" /></strong>
      <ul>
        <li><Trans i18nKey="sportsbook.potentialWinLimit.resultAmendment.recalculation" /></li>
        <li>
          <Trans i18nKey="sportsbook.potentialWinLimit.resultAmendment.statusChange" />:
          <ul>
            <li><Trans i18nKey="sportsbook.potentialWinLimit.resultAmendment.statusChange-blockedToUnblocked" /></li>
            <li><Trans i18nKey="sportsbook.potentialWinLimit.resultAmendment.statusChange-unblockedToBlocked" /></li>
          </ul>
        </li>
      </ul>
      <div>
        <Trans i18nKey="sportsbook.potentialWinLimit.resultAmendment.example" />:
        <ul>
          <li><Trans i18nKey="sportsbook.potentialWinLimit.resultAmendment.example-limit" /></li>
          <li><Trans i18nKey="sportsbook.potentialWinLimit.resultAmendment.example-before" /></li>
          <li><Trans i18nKey="sportsbook.potentialWinLimit.resultAmendment.example-after" /></li>
        </ul>
      </div>
    </div>
  );
};

const ImportantNotes = () => {
  return (
    <div>
      <strong><Trans i18nKey="sportsbook.potentialWinLimit.importantNotes.title" /></strong>
      <ul>
        <li><Trans i18nKey="sportsbook.potentialWinLimit.importantNotes.defaultLimit" /></li>
        <li><Trans i18nKey="sportsbook.potentialWinLimit.importantNotes.defaultReset" /></li>
        <li><Trans i18nKey="sportsbook.potentialWinLimit.importantNotes.appliesTo" /></li>
        <li><Trans i18nKey="sportsbook.potentialWinLimit.importantNotes.excludes" /></li>
        <li><Trans i18nKey="sportsbook.potentialWinLimit.importantNotes.management" /></li>
      </ul>
    </div>
  );
};

export const PotentialWinLimitSettingTab = () => {    
  return (
    <>
      <PotentialWinLimitOverview />
      <br />
      <CalculationLogic />
      <br />
      <TrackingStart />
      <br />
      <ResetBehavior />
      <br />
      <ResultAmendmentImpact />
      <br />
      <ImportantNotes />
      <br />
      <br />
    </>
  );
};

