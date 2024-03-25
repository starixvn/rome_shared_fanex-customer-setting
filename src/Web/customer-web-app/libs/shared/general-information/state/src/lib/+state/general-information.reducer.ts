import { GeneralInfo } from '../types/general-information-types';
import { GeneralInfoActionTypes, GeneralInformationAction } from './general-information.actions';

export interface GeneralInformationState {
  generalInfo: GeneralInfo;
}

export const initialState: GeneralInformationState = {
  generalInfo: {
    site: '',
    siteLabel: '',
    isSpecialSite: false,
    currency: '',
    isInternal: false,
    userName: '',
    password: '',
    maxCredit: 0,
    roleId: 4,
    custId: 0,
    uplineRoleId: 5,
    uplineCustId: 0,
    isAutomateTransfer: false,
    transferOption: '',
    displayTransferCondition: false,
    group: 'a',
    commissions: [],
    uplineUserName: '',
    newUserName: '',
    disableAutoMPT: false,
  },
};

export function generalInformationReducer(
  state: GeneralInformationState = initialState,
  action: GeneralInformationAction
): GeneralInformationState {
  switch (action.type) {
    case GeneralInfoActionTypes.Update: {
      const generalInfo = { ...state.generalInfo, ...action.payload };
      return { ...state, generalInfo: { ...generalInfo, isSpecialSite: generalInfo.siteLabel === '789y' } };
    }
    default: {
      return state;
    }
  }
}
