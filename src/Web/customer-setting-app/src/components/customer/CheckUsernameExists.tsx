import { useContext, useEffect, useRef, useState } from "react";
import { SingleSelect } from "@components/form";
import { characters } from "src/constants/characters";
import { AppContext } from "src/pages/newcustomer";

export const CheckUsernameExists = ({ onChange, ...props }: any) => {
  const { existAccountName } = useContext(AppContext);
  const [suggestName, setSuggestName] = useState("");
  const newName = useRef("");

  const getSuggestName = async (actorName: string) => {
    const url = `${process.env.NEXT_PUBLIC_CUSTOMER_SETTING_APP}/customer/suggest/${actorName}`;
    const res = await fetch(url);
    const data = await res.json();
    setSuggestName(data.suggestName);
    const nameSuggest = existAccountName
      .substring(0, existAccountName.length - 3)
      .concat(data.suggestName);
    newName.current = nameSuggest;
    onChange(nameSuggest);
  };

  useEffect(() => {
    if (existAccountName !== "") {
      getSuggestName(
        existAccountName.substring(0, existAccountName.length - 3)
      );
    }
  }, [existAccountName]);

  const handleSelectChange = (e: any, index: number) => {
    const length = newName.current.length;
    const result = newName.current.split("");
    result[length - (3 - index)] = e.target.value;
    const t = result.join("");
    newName.current = t;
    onChange(t);
  };

  const renderSelect = (suggestName: string) => {
    return Array.from(suggestName).map((item, index) => {
      return (
        <SingleSelect
          key={index}
          onChange={(e) => handleSelectChange(e, index)}
          options={characters}
          selectedValue={item}
        />
      );
    });
  };

  {
    console.log("render check username exist");
  }
  return (
    <>
      <span>
        {existAccountName
          ? existAccountName.substring(0, existAccountName.length - 3)
          : "_____________"}
      </span>
      {suggestName !== "" ? renderSelect(suggestName) : ""}
    </>
  );
};
