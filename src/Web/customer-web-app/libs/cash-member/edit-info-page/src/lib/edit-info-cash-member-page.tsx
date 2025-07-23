import { ButtonTabActive, ButtonTabInactive, TabNav, Title, useSystems } from '@customer-web-app/shared/base/components';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { EditInfoCashMemberForm } from './edit-info-cash-member-form';
import { CustomerBankDetail } from './customer-bank-detail';
import { PageTitle } from './page-title';

const enum Tab {
    MemberInfo = 'MemberInfo',
    BankAccount = 'BankAccount'
}

export function EditInfoCashMemberPage() {
    const { isAlphaSystem, isExternalNapSystem } = useSystems(updateInfoCustomerPageConfig.systemId);
    const { t } = useTranslation();
    const [activeTab, setActiveTab] = useState('');
    const tabButton = (tab: string) => {
        const tabList: React.ReactNode[] = [];
        if (tab === Tab.MemberInfo) {
            tabList.push(<ButtonTabActive text={t('MemberInfo')} />)
            tabList.push(<ButtonTabInactive text={t('BankAccount')} onClick={() => setActiveTab(Tab.BankAccount)} />)
        }
        if (tab === Tab.BankAccount) {
            tabList.push(<ButtonTabInactive text={t('MemberInfo')} onClick={() => setActiveTab(Tab.MemberInfo)} />)
            tabList.push(<ButtonTabActive text={t('BankAccount')} />)
        }
        return tabList.map((item, index) => (
            <li key={`${tab}-${index}`} className="mr-[8px]">
                {item}
            </li>
        ));
    }
    useEffect(() => {
        if (isExternalNapSystem && updateInfoCustomerPageConfig.isCashMember) {
            setActiveTab(Tab.MemberInfo);
        }
        if(isAlphaSystem && updateInfoCustomerPageConfig.isCashMember){
            setActiveTab(Tab.BankAccount);
        }
    }, [isExternalNapSystem, isAlphaSystem, updateInfoCustomerPageConfig.isCashMember]);

    return (
        <>
            <PageTitle />
            {isAlphaSystem && <TabNav>{tabButton(activeTab)}</TabNav>}
            {activeTab === Tab.MemberInfo && <EditInfoCashMemberForm />}
            {activeTab === Tab.BankAccount && isAlphaSystem && <CustomerBankDetail />}
        </>
    );
}
