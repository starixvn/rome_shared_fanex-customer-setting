namespace Alpha.CloneAccount.Tests.Criteria
{
    using System;
    using CloneAccount.Criteria;

    public class CopyCustomerCommandForTesting : CopyCustomerCommand
    {
        public CopyCustomerCommandForTesting(
            string userName,
            string userpwd,
            string firstName,
            string lastName,
            string phone,
            string mobilePhone,
            string fax,
            decimal credit,
            int creator,
            int sourceCustId,
            int subAccountId,
            string language,
            string ip,
            byte systemId,
            byte settingStatus)
            : base(userName, userpwd, firstName, lastName, phone, mobilePhone, fax, credit, creator, sourceCustId, subAccountId, language, ip, systemId, settingStatus)
        {
        }

        public override string GetSettingKey()
        {
            throw new NotImplementedException();
        }
    }
}
