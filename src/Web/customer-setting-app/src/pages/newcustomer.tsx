import React from "react";
import { GetServerSideProps } from "next";
import Header from "@components/header/Header";
import styles from "@styles/Home.module.scss";
import { CreateNewCustomer, NewCustomerProvider } from "@features/new-customer";

interface Props {
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

export const getServerSideProps: GetServerSideProps = async (context) => {
  const url = `${process.env.CUSTOMER_API}/${1176086}/max-credit`;
  const res = await fetch(url);
  return {
    props: {
      isAgent: true,
      minCredit: 0,
      maxCredit: 100,
      maxAccounts: process.env.MAX_ACCOUNTS,
    },
  };
};

export default NewCustomer;
