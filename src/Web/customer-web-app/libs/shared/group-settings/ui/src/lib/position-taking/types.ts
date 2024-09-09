import React from 'react';

import { GroupSettingBlockProps } from '../types';

export type PositionTakingBlockProps = {
  isDisableMasterAutoPT?: boolean;
  title: string | React.ReactNode;
} & GroupSettingBlockProps;

export enum CopyStatus {
  Fail = 1,
  Success = 2,
  NotCopy = 3,
}

export type PTRowProps<T> = {
  isReadOnly: boolean;
  item: T;
  copyStatus: CopyStatus;
  label?: string | React.ReactNode;
  isDisableMasterAutoPT?: boolean;
};
