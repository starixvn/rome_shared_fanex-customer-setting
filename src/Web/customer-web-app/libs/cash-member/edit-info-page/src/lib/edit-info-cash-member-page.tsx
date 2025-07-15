import { ButtonTabActive, ButtonTabInactive, TabNav, Title, useSystems } from '@customer-web-app/shared/base/components';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { EditInfoCashMemberForm } from './edit-info-cash-member-form';
import { EditBankInfoCashMember } from './edit-bank-info-cash-member';
import { SystemIds } from '@customer-web-app/shared-constants';

export function EditInfoCashMemberPage() {
    const { isAlpha } = useSystems(updateInfoCustomerPageConfig.systemId);
    const { t } = useTranslation();
    const [activeTab, setActiveTab] = useState('bankInfo');
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

    return (
        <>
            <Title>
                {t('InfoUsername', { username: updateInfoCustomerPageConfig.username.toLocaleUpperCase() })}
            </Title>
            {isAlpha && (
                <TabNav>
                    {tabButton(activeTab)}
                </TabNav>
            )}

            {activeTab === 'memberInfo' && (
                <EditInfoCashMemberForm />
            )}
            {activeTab === 'bankInfo' && (
                <EditBankInfoCashMember />
            )}

        </>
    );
}
