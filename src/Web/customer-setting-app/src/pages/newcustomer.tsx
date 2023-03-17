import React from "react";
import { withIronSessionSsr } from "iron-session/next";
import { sessionOptions, User } from "src/lib/session";
import Layout from "@components/Layout";
import useTranslation from "next-translate/useTranslation";
import { CreateNewCustomer } from "@features/new-customer/CreateNewCustomer";

interface Props {
  user: User;
  minCredit: number;
  maxAccounts: number;
  actorName: string;
  isMasterLevel: boolean;
}

const NewCustomer = (props: Props) => {
  const { t } = useTranslation("newcustomer");
  return (
    <Layout title={t("header")}>
      <CreateNewCustomer props={props} />
    </Layout>
  );
};

export const getServerSideProps = withIronSessionSsr(async function ({
  req,
  res,
}) {
  const user = req.session.user;

  if (user === undefined) {
    res.setHeader("location", "/unauthorized");
    res.statusCode = 302;
    res.end();
    return {
      props: {
        user: {
          isLoggedIn: false,
          custId: 0,
          subAccId: 0,
          customerLevel: 0,
          username: "",
          language: "",
        } as User,
        minCredit: 0,
        maxAccounts: process.env.MAX_ACCOUNTS,
        actorName: "",
        isMasterLevel: false
      },
    };
  }

  return {
    props: {
      user: user,
      minCredit: 0,
      maxAccounts: process.env.MAX_ACCOUNTS,
      actorName: user.username,
      isMasterLevel: user.customerLevel === 3
    },
  };
},
sessionOptions);

export default NewCustomer;
