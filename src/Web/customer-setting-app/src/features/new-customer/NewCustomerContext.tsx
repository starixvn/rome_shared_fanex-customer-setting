import { createContext, useState } from "react";

type NewCustomerContextProps = {
  copyCustomerName: string;
  setCopyCustomerName: (copyCustomerName: string) => void;
};

export const NewCustomerContext = createContext<NewCustomerContextProps>({
  copyCustomerName: "",
  setCopyCustomerName: () => {},
});

export const NewCustomerProvider = ({ children }: any) => {
  const [copyCustomerName, setCopyCustomerName] = useState("");

  const value = {
    copyCustomerName: copyCustomerName,
    setCopyCustomerName: setCopyCustomerName,
  };

  return (
    <NewCustomerContext.Provider value={value}>
      {children}
    </NewCustomerContext.Provider>
  );
};
