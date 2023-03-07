import React from "react";
import { GetServerSideProps } from "next";
import { CreateNewCustomer } from "@features/newcustomer";

interface Props {
  isAgent: boolean;
  minCredit: number;
  maxCredit: number;
  maxAccounts: number;
}

const NewCustomer = (props: Props) => {
  return <CreateNewCustomer props={props} />;
};

export const getServerSideProps: GetServerSideProps = async (context) => {
  const url = `${process.env.CUSTOMER_API}/${1176086}/max-credit`;
  const res = await fetch(url);
  return {
    props: {
      isAgent: true,
      minCredit: 0,
      maxCredit: await res.json(),
      maxAccounts: process.env.MAX_ACCOUNTS,
    },
  };
};

export default NewCustomer;
