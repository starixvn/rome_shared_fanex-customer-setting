import React from "react";
import Header from "@components/Header/Header";
import styles from "@styles/Home.module.scss";
import { CreateNewCustomer, NewCustomerProvider } from "@features/new-customer";
import { withIronSessionSsr } from "iron-session/next";
import { sessionOptions, User } from "src/lib/session";

interface Props {
  user: User;
  isAgent: boolean;
  minCredit: number;
  maxCredit: number;
  maxAccounts: number;
}

const NewCustomer = (props: Props) => {
  return (
    <NewCustomerProvider>
      <div className={styles.container}>
        <Header headerContent="Create New Customer"></Header>
        <CreateNewCustomer props={props} />
      </div>
    </NewCustomerProvider>
  );
};

export const getServerSideProps = withIronSessionSsr(async function ({
  req,
  res,
}) {
  const user = req.session.user;

  if (user === undefined) {
    res.setHeader("location", "//unauthorized");
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
        isAgent: true,
        minCredit: 0,
        maxCredit: 100,
        maxAccounts: process.env.MAX_ACCOUNTS,
      },
    };
  }

  //const url = `${process.env.CUSTOMER_API}/${user.}/max-credit`;

  return {
    props: {
      user: user,
      isAgent: true,
      minCredit: 0,
      maxCredit: 100,
      maxAccounts: process.env.MAX_ACCOUNTS,
    },
  };
},
sessionOptions);

// export const getServerSideProps: GetServerSideProps = async (context) => {
//   const url = `${process.env.CUSTOMER_API}/${1176086}/max-credit`;
//   const res = await fetch(url);
//   return {
//     props: {
//       isAgent: true,
//       minCredit: 0,
//       maxCredit: 100,
//       maxAccounts: process.env.MAX_ACCOUNTS,
//     },
//   };
// };

export default NewCustomer;
