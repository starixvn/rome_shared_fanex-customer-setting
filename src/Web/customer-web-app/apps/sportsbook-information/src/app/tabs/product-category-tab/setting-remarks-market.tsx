import { Trans } from 'react-i18next';

export const UserLevelName: { [key: number]: string } = {
  1: 'Member',
  2: 'Agent',
  3: 'Master',
  4: 'Super',
};

interface Note {
  [key: string]: Note[] | string[] | Note;
}

interface SettingRemarksMarketBuilderProps {
  userRoleId: number;
  Is789y: boolean;
  showNoteRNGKenoNumberGame: boolean;
  showNoteBBin: boolean;
}

export default function SettingRemarksMarketBuilder({
  userRoleId,
  Is789y,
  showNoteRNGKenoNumberGame,
  showNoteBBin,
}: SettingRemarksMarketBuilderProps) {
  const mappingMarket: { [key: number]: string } = {
    0: 'MBCMarket',
    1: 'I7Market',
  };
  const mappingContentKey: Note = {
    CreateMasterMBCMarket: [
      {
        SportsBook: ['SMANote', 'MixSportsParlayBetSetting'],
      },
      {
        RNGGames: ['SMANote', 'CommonWinLossLimitNote', 'RNGKenoNumberGameWinLossLimitNote'],
      },
      {
        LiveRNGCasino: ['BBINBetSetting', 'WinLossLimitNote3'],
      },
      {
        OtherGames: ['SMANote', 'CommonWinLossLimitNote'],
      },
      {
        PTTemplate: ['CommonPTSettingNote1', 'PTSettingNote2'],
      },
    ],
    CreateMasterI7Market: [
      {
        SportsBook: ['SportsBookBetSettingI7', 'ESportMuayThaiBetSetting', 'MixSportsParlayBetSetting'],
      },
      {
        RNGGames: ['Y7SMANote', 'CommonWinLossLimitNote', 'RNGKenoNumberGameWinLossLimitNote'],
      },
      {
        LiveRNGCasino: ['BBINBetSetting', 'WinLossLimitNote3'],
      },
      {
        OtherGames: ['Y7SMANote', 'CommonWinLossLimitNote'],
      },
      {
        PTTemplate: ['CommonPTSettingNote1', 'PTSettingNote2'],
      },
    ],
    CreateAgentMBCMarket: [
      {
        SportsBook: ['SMANote', 'MixSportsParlayBetSetting'],
      },
      {
        RNGGames: ['SMANote', 'CommonWinLossLimitNote', 'RNGKenoNumberGameWinLossLimitNote'],
      },
      {
        LiveRNGCasino: ['BBINBetSetting', 'AgentStakeLevelWarning', 'WinLossLimitNote3'],
      },
      {
        OtherGames: ['SMANote', 'CommonWinLossLimitNote'],
      },
      {
        PTTemplate: ['CommonPTSettingNote1', 'MasterPTSetting'],
      },
    ],
    CreateAgentI7Market: [
      {
        SportsBook: ['SportsBookBetSettingI7', 'ESportMuayThaiBetSetting', 'MixSportsParlayBetSetting'],
      },
      {
        RNGGames: ['Y7SMANote', 'CommonWinLossLimitNote', 'RNGKenoNumberGameWinLossLimitNote'],
      },
      {
        LiveRNGCasino: ['BBINBetSetting', 'AgentStakeLevelWarning', 'WinLossLimitNote3'],
      },
      {
        OtherGames: ['Y7SMANote', 'CommonWinLossLimitNote'],
      },
      {
        PTTemplate: ['CommonPTSettingNote1', 'MasterPTSetting'],
      },
    ],
    CreateMemberMBCMarket: [
      {
        SportsBook: ['MemberNote', 'MixSportsParlayBetSetting'],
      },
      {
        RNGGames: ['MemberNote', 'CommonWinLossLimitNote', 'RNGKenoNumberGameWinLossLimitNote'],
      },
      {
        LiveRNGCasino: ['BBINBetSetting', 'WinLossLimitNote3'],
      },
      {
        OtherGames: ['MemberNote', 'CommonWinLossLimitNote'],
      },
      {
        PTTemplate: ['CommonPTSettingNote1', 'MemberPTSettingNote'],
      },
    ],
    CreateMemberI7Market: [
      {
        SportsBook: ['MemberI7SportBookBetSetting', 'ESportMuayThaiBetSetting', 'MixSportsParlayBetSetting'],
      },
      {
        RNGGames: ['CommonWinLossLimitNote', 'RNGKenoNumberGameWinLossLimitNote'],
      },
      {
        LiveRNGCasino: ['BBINBetSetting', 'WinLossLimitNote3'],
      },
      {
        OtherGames: ['CommonWinLossLimitNote'],
      },
      {
        PTTemplate: ['CommonPTSettingNote1', 'MemberPTSettingNote'],
      },
    ],
  };

  const Market = mappingMarket[Number(Is789y)];
  const ContentKeys = mappingContentKey[`Create${UserLevelName[userRoleId]}${Market}`] as Note[];

  const filterNotes = (notes: string[]) => {
    const notesToRemove = [''];
    if (!showNoteRNGKenoNumberGame) {
      notesToRemove.push('RNGKenoNumberGameWinLossLimitNote');
    }
    if (!showNoteBBin) {
      notesToRemove.push('BBINBetSetting');
    }
    return notes.filter((item) => !notesToRemove.includes(item));
  };

  const renderChildrenNote = (notes: string[]) => (
    <ul>
      {notes.map((note, index) => (
        <li key={note + index.toString()}>
          <Trans i18nKey={note} />
        </li>
      ))}
    </ul>
  );
  return (
    <div>
      {ContentKeys?.map((e, index) => (
        <div key={e + index.toString()}>
          <span>{index + 1}. </span> <Trans i18nKey={Object.keys(e)[0]} />
          {renderChildrenNote(filterNotes(Object.values(e)[0] as string[]))}
        </div>
      ))}
    </div>
  );
}
