import { ButtonTabActive, ButtonTabInactive, TabNav, Title, useSystems } from '@customer-web-app/shared/base/components';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { EditInfoCashMemberForm } from './edit-info-cash-member-form';
import { CustomerBankDetail } from './customer-bank-detail';
import { PageTitle } from './page-title';

export function EditInfoCashMemberPage() {
    const { isAlphaSystem, isExternalNapSystem } = useSystems(updateInfoCustomerPageConfig.systemId);
    const { t } = useTranslation();
    const [activeTab, setActiveTab] = useState('');
    const tabButton = (tab: string) => {
        const tabList: React.ReactNode[] = [];
        if (tab === 'memberInfo') {
            tabList.push(<ButtonTabActive text={t('MemberInfo')} />)
            tabList.push(<ButtonTabInactive text={t('BankInfo')} onClick={() => setActiveTab('bankInfo')} />)
        }
        if (tab === 'bankInfo') {
            tabList.push(<ButtonTabInactive text={t('MemberInfo')} onClick={() => setActiveTab('memberInfo')} />)
            tabList.push(<ButtonTabActive text={t('BankInfo')} />)
        }
        return tabList.map((item, index) => (
            <li key={`${tab}-${index}`} className="mr-[8px]">
                {item}
            </li>
        ));
    }
    useEffect(() => {
        if (isExternalNapSystem && updateInfoCustomerPageConfig.isCashMember) {
            setActiveTab('memberInfo');
        }
        if(isAlphaSystem && updateInfoCustomerPageConfig.isCashMember){
            setActiveTab('bankInfo');
        }
    }, [isExternalNapSystem, isAlphaSystem, updateInfoCustomerPageConfig.isCashMember]);

    return (
        <>
            <PageTitle />
            {isAlphaSystem && <TabNav>{tabButton(activeTab)}</TabNav>}
            {activeTab === 'memberInfo' && <EditInfoCashMemberForm />}
            {activeTab === 'bankInfo' && isAlphaSystem && <CustomerBankDetail />}
        </>
    );
}
