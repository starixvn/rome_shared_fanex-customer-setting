import { useEffect, useState } from "react";
import { SingleSelect } from "@components/form";
import { characters } from "src/constants/characters";
import { CheckMarkIcon, ErrorIcon } from "@components/icons/Icons";

export const NewAccountName = ({
  actorName,
  submitSuccessCount,
  onChange,
}: any) => {
  const [suggestName, setSuggestName] = useState("");
  const [isCorrectName, setIsCorrectName] = useState(false);

  useEffect(() => {
    getSuggestName(actorName);
  }, [submitSuccessCount]);

  useEffect(() => {
    if (suggestName !== "") {
      checkExistUserName();
    }
  }, [suggestName]);

  const getSuggestName = async (actorName: string) => {
    const url = `${process.env.NEXT_PUBLIC_CUSTOMER_SETTING_APP}/customer/suggest/${actorName}`;
    const res = await fetch(url);
    const data = await res.json();
    setSuggestName(data.suggestName);
    handleOnChange(data.suggestName);
  };

  const checkExistUserName = async () => {
    const userName = actorName.concat(suggestName);
    const url = `${process.env.NEXT_PUBLIC_CUSTOMER_SETTING_APP}/customer/exist/${userName}`;
    const res = await fetch(url);
    const data = await res.json();
    if (!data.isExist) {
      setIsCorrectName(true);
    } else {
      setIsCorrectName(false);
    }
  };

  const handleSelectChange = (e: any, index: number) => {
    const result = suggestName.split("");
    result[index] = e.target.value;
    setSuggestName(result.join(""));
    handleOnChange(result.join(""));
  };

  const handleOnChange = (newSuggestName: string) => {
    onChange(actorName.concat(newSuggestName));
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
      <span>{actorName}</span>
      {suggestName ? renderSelect(suggestName) : ""}
      {isCorrectName ? <CheckMarkIcon height={16} width={16} /> : null}
    </>
  );
};
