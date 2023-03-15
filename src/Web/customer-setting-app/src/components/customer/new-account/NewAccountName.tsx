import { useContext, useEffect, useState } from "react";
import { SingleSelect } from "@components/form";
import { characters } from "src/constants/characters";
import { NewCustomerContext } from "@features/new-customer";

export const NewAccountName = ({ onChange, ...props }: any) => {
  const { copyCustomerName } = useContext(NewCustomerContext);
  const [suggestName, setSuggestName] = useState("000");

  const getSuggestName = async (actorName: string) => {
    const url = `${process.env.NEXT_PUBLIC_CUSTOMER_SETTING_APP}/customer/suggest/${actorName}`;
    const res = await fetch(url);
    const data = await res.json();
    setSuggestName(data.suggestName);
    handleOnChange(data.suggestName);
  };

  useEffect(() => {
    if (copyCustomerName !== "") {
      getSuggestName(
        copyCustomerName.substring(0, copyCustomerName.length - 3)
      );
    }
  }, [copyCustomerName]);

  const handleSelectChange = (e: any, index: number) => {
    const result = suggestName.split("");
    result[index] = e.target.value;
    setSuggestName(result.join(""));
    handleOnChange(result.join(""));
  };

  const handleOnChange = (newSuggestName: string) => {
    onChange(
      copyCustomerName
        .substring(0, copyCustomerName.length - newSuggestName.length)
        .concat(newSuggestName)
    );
  };

  const renderSelect = (suggestName: string) => {
    return Array.from(suggestName).map((item, index) => {
      return (
        <SingleSelect
          key={`${item}${index}`}
          handleChange={(e) => handleSelectChange(e, index)}
          options={characters}
          selectedValue={item.toString()}
        />
      );
    });
  };

  console.log("render check username exist");
  return (
    <>
      <span>
        {copyCustomerName
          ? copyCustomerName.substring(0, copyCustomerName.length - 3)
          : "_____________"}
      </span>
      {suggestName ? renderSelect(suggestName) : ""}
    </>
  );
};
