import { createContext, useState } from "react";

export const NewCustomerContext = createContext({
  existAccountName: "",
  setExistAccountName: (name: any) => {},
});

export const NewCustomerProvider = ({ children }: any) => {
  const [existAccountName, setExistAccountName] = useState("");
  return (
    <NewCustomerContext.Provider
      value={{ existAccountName, setExistAccountName }}
    >
      {children}
    </NewCustomerContext.Provider>
  );
};
