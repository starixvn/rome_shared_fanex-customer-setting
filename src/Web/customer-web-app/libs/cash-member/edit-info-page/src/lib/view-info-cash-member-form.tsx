import { useEffect, useState } from 'react';
import styles from './view-info-cash-member-form.module.scss';
import { Label, useSystems } from '@customer-web-app/shared/base/components';
import { GetCustomerInfo } from './edit-info-cash-member-services';
import { splitMobile } from '@customer-web-app/shared-helpers';
import { useTranslation } from 'react-i18next';
import classNames from 'classnames';

interface CustomerInfo {
    custId: number;
    firstName: string;
    lastName: string;
    mobilePhone: string;
    mobileCode: string;
    mobileNumber: string;
    phone: string;
    email: string;
    fax: string;
    remark: string;
}

export const ViewInfoCashMemberForm = () => {
    const { t } = useTranslation();
    const { isAlphaSystem } = useSystems(updateInfoCustomerPageConfig.systemId);
    const [customerInfo, setCustomerInfo] = useState<CustomerInfo | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const fetchCustomerInfo = async () => {
            try {
                const data = await GetCustomerInfo<CustomerInfo>({
                    custId: updateInfoCustomerPageConfig.custId
                });
                const mobile = splitMobile(data.mobilePhone);
                data.mobileCode = mobile.countryCode;
                data.mobileNumber = mobile.mobile;
                setCustomerInfo(data);
            } catch (error) {
                console.error('Error fetching customer info:', error);
            } finally {
                setIsLoading(false);
            }
        }
        fetchCustomerInfo();
    }, [updateInfoCustomerPageConfig.custId]);

    if (isLoading) {
        return (
            <div className={styles['view-info-cash-member-form']}>
                <div>Loading...</div>
            </div>
        );
    }

    if (!customerInfo) {
        return (
            <div className={styles['view-info-cash-member-form']}>
                <div>No customer information found</div>
            </div>
        );
    }

    return (
        <div className={styles['view-info-cash-member-form']}>
            <div className={styles['form-row']}>
                <Label className={styles['label']}>{t('firstName')}</Label>
                <input 
                    className={styles['input-field']} 
                    value={customerInfo.firstName || '-'} 
                    disabled 
                />
            </div>
            <div className={styles['form-row']}>
                <Label className={styles['label']}>{t('lastName')}</Label>
                <input 
                    className={styles['input-field']} 
                    value={customerInfo.lastName || '-'} 
                    disabled 
                />
            </div>
          
            <div className={styles['form-row']}>
                <Label className={styles['label']}>{t('phone')}</Label>
                <input 
                    className={styles['input-field']} 
                    value={customerInfo.phone || '-'} 
                    disabled 
                />
            </div>
            <div className={styles['form-row']}>
                <Label className={styles['label']}>{t('mobilePhone')}</Label>
                <input 
                    className={styles['input-field']} 
                    value={customerInfo.mobileCode && customerInfo.mobileNumber
                        ? `${customerInfo.mobileCode}-${customerInfo.mobileNumber}`
                        : '-'} 
                    disabled 
                />
            </div>
            <div className={styles['form-row']}>
                <Label className={styles['label']}>{t('Email')}</Label>
                <input 
                    className={styles['input-field']} 
                    value={customerInfo.email || '-'} 
                    disabled 
                />
            </div>
            <div className={styles['form-row']}>
                <Label className={styles['label']}>{t('fax')}</Label>
                <input 
                    className={styles['input-field']} 
                    value={customerInfo.fax || '-'} 
                    disabled 
                />
            </div>
            {!isAlphaSystem && (
                <div className={styles['form-row']}>
                    <Label className={classNames(styles['label'], styles['remark'])}>{t('Remark')}</Label>
                    <textarea 
                        className={styles['textarea-field']} 
                        value={customerInfo.remark || '-'} 
                        disabled 
                    />
                </div>
            )}
        </div>
    );
};