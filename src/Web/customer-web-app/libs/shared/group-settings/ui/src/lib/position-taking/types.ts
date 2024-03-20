import React from 'react';

import { GroupSettingBlockProps } from '../types';

export type PositionTakingBlockProps = {
  isDisableMasterAutoPT?: boolean;
  title: string | React.ReactNode;
} & GroupSettingBlockProps;
