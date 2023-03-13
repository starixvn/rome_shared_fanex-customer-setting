import { useContext, useEffect, useState } from "react";
import { SingleSelect } from "@components/form";
import { characters } from "src/constants/characters";
import { NewCustomerContext } from "@features/new-customer";

export const NewAccountName = ({ onChange, ...props }: any) => {
  const { existAccountName } = useContext(NewCustomerContext);
  const [suggestName, setSuggestName] = useState("000");
  const [isCorrect, setIsCorrect] = useState(0);

  const getSuggestName = async (actorName: string) => {
    const url = `${process.env.NEXT_PUBLIC_CUSTOMER_SETTING_APP}/customer/suggest/${actorName}`;
    const res = await fetch(url);
    const data = await res.json();
    setSuggestName(data.suggestName);
    const nameSuggest = existAccountName
      .substring(0, existAccountName.length - 3)
      .concat(data.suggestName);
    setSuggestName(nameSuggest);
    setIsCorrect(1);
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
    const length = suggestName.length;
    const result = suggestName.split("");
    result[length - (3 - index)] = e.target.value;
    const t = result.join("");
    setSuggestName(t);
    setIsCorrect(isCorrect + 1);
    onChange(t);
  };

  const renderSelect = (suggestName: string) => {
    const newArr = suggestName.slice(suggestName.length - 3);
    return Array.from(newArr).map((item, index) => {
      // console.log(item, index);
      return (
        <SingleSelect
          key={`${item}${index}`}
          onChange={(e) => handleSelectChange(e, index)}
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
        {existAccountName
          ? existAccountName.substring(0, existAccountName.length - 3)
          : "_____________"}
      </span>
      {suggestName ? renderSelect(suggestName) : ""}
      {/* {isCorrect === 0 ? "" : <CheckMarkIcon height={16} width={16} />} */}
    </>
  );
};
