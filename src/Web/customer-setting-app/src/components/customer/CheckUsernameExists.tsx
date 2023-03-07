import { useState } from "react";
import { SingleSelect } from "@components/form";
import { characters } from "src/constants/characters";
import { CheckMarkIcon } from "@components/icons";

export const CheckUsernameExists = ({ onChange, ...props }: any) => {
  const [userName, setUserName] = useState({
    first: "1",
    second: "1",
    third: "1",
  });

  const handleSelectChange = (e: any) => {
    const newUserName = {
      ...userName,
      [e.target.name]: e.target.value,
    };
    onChange(newUserName);
    setUserName(newUserName);
  };

  return (
    <>
      <span>Agent01</span>
      <SingleSelect
        name="first"
        options={characters}
        onChange={handleSelectChange}
      />
      <SingleSelect
        name="second"
        options={characters}
        onChange={handleSelectChange}
      />
      <SingleSelect
        name="third"
        options={characters}
        onChange={handleSelectChange}
      />
      <CheckMarkIcon height={16} width={16} />
    </>
  );
};
