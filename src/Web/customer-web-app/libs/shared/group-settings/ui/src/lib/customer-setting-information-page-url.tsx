import { useGroupSettingContext } from '@customer-web-app/shared-group-settings-context';

interface CustomerSettingInformationPageUrlProps {
  tab?: number;
}

export const CustomerSettingInformationPageUrl = ({ tab }: CustomerSettingInformationPageUrlProps) => {
  const { mode, generalInfo } = useGroupSettingContext();
  const siteCurrency = `&site=${generalInfo?.site}&siteLabel=${generalInfo?.siteLabel}&currency=${generalInfo?.currency}`;

  const getCustomerIdAndRoleId = () => {
    switch (mode) {
      case 'createSuper':
        return `&roleId=4`;
      case 'createCustomer':
        return `&custId=${generalInfo?.uplineCustId}&roleId=${generalInfo?.roleId}`;
      case 'edit':
        return `&custId=${params?.custId}&roleId=${params?.roleId}`;
      case 'editMultipleCustomers':
        return `&custId=${params?.custIds.split(',')[0]}&roleId=${params?.roleId}`;
      default:
        return `&custId=${params.custId || params.uplineId}&roleId=${params.roleId || params.uplineRoleId - 1}`;
    }
  };

  const queryParams = `${getCustomerIdAndRoleId()}${siteCurrency}`;
  const url = `${
    appConfig.applicationPath === '/' ? window.location.origin + '/' : appConfig.applicationPath
  }Customer/SportBookInformation?tabindex=${tab}${queryParams}`;

  return { url };
};
