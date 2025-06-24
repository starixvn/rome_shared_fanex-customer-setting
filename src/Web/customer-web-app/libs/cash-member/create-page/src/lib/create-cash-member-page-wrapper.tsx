import { SettingService } from '@customer-web-app/shared-group-settings-services';
import { useState, useEffect } from 'react';
import { CreateCashMemberPage } from './create-cash-member-page';
import CashServiceAds from './cash-service-ads';
import { CashServiceStatus } from '@customer-web-app/share-models';
export interface CreateDirectMemberPageWrapperProps { }

export function CreateCashMemberPageWrapper(props: CreateDirectMemberPageWrapperProps) {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isEnabled, setIsEnabled] = useState(false);
  const [isError, setIsError] = useState(false);
  const [cashServiceStatus, setCashServiceStatus] = useState<CashServiceStatus>({ isCashServiceEnabled: false, isCanUpdateCashServiceStatus: false, isOfferedCashService: false });

  useEffect(() => {
    const checkCashServiceStatus = async () => {
      try {
        const result = await SettingService.GetCashServiceStatus();
        setCashServiceStatus(result);
        setIsModalOpen(!result.isCashServiceEnabled);
        setIsEnabled(result.isCashServiceEnabled);
      } catch (error) {
        setIsError(true);
        console.error('Error checking cash service status:', error);
      }
    };
    checkCashServiceStatus();
  }, []);

  const handleEnableCashService = async () => {
    const result = await SettingService.EnableCashService();
    if (result) {
      setIsEnabled(true);
      setIsModalOpen(false);
    }
  }

  return (
    <>
      {isModalOpen && <CashServiceAds onConfirm={handleEnableCashService} cashServiceStatus={cashServiceStatus} />}
      {!isError && <CreateCashMemberPage isEnabled={isEnabled} />}
    </>
  );
}