import React, { createContext, useState } from "react";
import { GetServerSideProps } from "next";
import { CreateNewCustomer } from "@features/newcustomer";

interface Props {
  isAgent: boolean;
  minCredit: number;
  maxCredit: number;
  maxAccounts: number;
}

export const AppContext = createContext({
  existAccountName: "",
  setExistAccountName: (name: any) => {},
});
const AppProvider = ({ children }: any) => {
  const [existAccountName, setExistAccountName] = useState("");
  return (
    <AppContext.Provider value={{ existAccountName, setExistAccountName }}>
      {children}
    </AppContext.Provider>
  );
};

const NewCustomer = (props: Props) => {
  return (
    <AppProvider>
      <CreateNewCustomer props={props} />;
    </AppProvider>
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
