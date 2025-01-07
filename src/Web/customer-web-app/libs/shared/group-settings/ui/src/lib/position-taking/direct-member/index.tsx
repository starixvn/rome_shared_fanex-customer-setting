import { forwardRef, useRef } from 'react';
import { useTranslation } from 'react-i18next';

import { ValidationRefType } from '@customer-web-app/share-models';
import { GroupTypeIds, ProductGroupIds, UserLevel } from '@customer-web-app/shared-constants';
import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';
import { Block, BlockRefType } from '@customer-web-app/shared/base/block';

import { GroupSettingBlockProps } from '../../types';
import PositionTakingNote from '../position-taking-note';
import styles from './direct-member-position-taking.module.scss';
import MasterDirectMemberPositionTaking from './master-direct-member-position-taking';
import SuperDirectMemberPositionTaking from './super-direct-member-position-taking';

const PositionTakingBlockId = 'position-taking';

const DirectMemberPositionTaking = forwardRef<ValidationRefType, GroupSettingBlockProps>(
  function DirectMemberPositionTaking(props, ref) {
    const blockRef = useRef<BlockRefType>(null);
    const { t } = useTranslation();
    const { productGroupId, isEditMode, handleSettingItemChange, updateGroupSetting } = useGroupSettingContext();

    const handlePTChange = (sportTypeId: number, betTypeId: number, data: { [x: string]: number | undefined }) => {
      handleSettingItemChange?.(Number(productGroupId), GroupTypeIds.PositionTaking, sportTypeId, betTypeId, data);
    };

    const renderContent = () => {
      if (params.uplineRoleId === UserLevel.Super) {
        return <SuperDirectMemberPositionTaking isReadOnly={props.isReadOnly} onPTChange={handlePTChange} />;
      }

      return <MasterDirectMemberPositionTaking isReadOnly={props.isReadOnly} onPTChange={handlePTChange} />;
    };

    return (
      <div className={styles['short-block']}>
        <Block
          ref={blockRef}
          title={
            <>
              {t('PositionTaking')}
              {productGroupId === ProductGroupIds.SportsBook && <PositionTakingNote />}
            </>
          }
          id={PositionTakingBlockId}
          isPartialUpdate={isEditMode === true && !props.isReadOnly}
          onSaveClick={() => updateGroupSetting?.(GroupTypeIds.PositionTaking)}
        >
          {renderContent()}
        </Block>
      </div>
    );
  }
);

export default DirectMemberPositionTaking;
