import { GeneralInfo } from '../types/general-information-types';

export enum GeneralInfoActionTypes {
  Update = '[GeneralInformation] Update',
}

export interface GeneralInformationAction {
  type: GeneralInfoActionTypes;
  payload: Partial<GeneralInfo>;
}
