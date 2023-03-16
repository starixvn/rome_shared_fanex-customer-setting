import { CheckMarkIcon } from "@components/icons/Icons";
import React, { useEffect, useState } from "react";
import { components, GroupBase, OptionProps } from "react-select";
import AsyncCreatableSelect from "react-select/async-creatable";
import Select from "react-select/dist/declarations/src/Select";
import { customerStyles } from "./CustomStyle";

const getCustomers = async (query: string) => {
  const url = `${process.env.NEXT_PUBLIC_CUSTOMER_SETTING_APP}/customer/search?query=${query}`;
  const res = await fetch(url);
  const result = await res.json();
  return result;
};

export const customPromiseOptions = async (inputValue: string) => {
  const data = await getCustomers(inputValue);
  return data.map((item: any) => {
    return {
      ...item,
      value: item.userName,
    };
  });
};

const CustomOption = (props: OptionProps<any>) => {
  return (
    <components.Option {...props}>
      {props.data.userName ? (
        <span>
          {props.data.userName}: {props.data.userName2}
        </span>
      ) : (
        <span>{props.data.value}</span>
      )}
    </components.Option>
  );
};

export const SearchCustomerExists = ({
  onChange,
  changeMaxCredit,
  submitSuccessCount,
}: any) => {
  let selectInputRef: Select<any, boolean, GroupBase<any>> | null = null;

  const [isCorrectName, setIsCorrectName] = useState(false);

  const getMaxCredit = async (username: string) => {
    const url = `${process.env.NEXT_PUBLIC_CUSTOMER_SETTING_APP}/customer/credit/${username}`;
    const result = await fetch(url);
    return await result.json();
  };

  const checkExistUserName = async (username: string) => {
    const url = `${process.env.NEXT_PUBLIC_CUSTOMER_SETTING_APP}/customer/exist/${username}`;
    const res = await fetch(url);
    const data = await res.json();
    if (data.isExist) {
      setIsCorrectName(true);
    } else {
      setIsCorrectName(false);
    }
  };

  const onHandleChange = async (option: any) => {
    if (option) {
      const data = await getMaxCredit(option.value);
      await checkExistUserName(option.value)
      changeMaxCredit(data.maxCredit);
      onChange(option.value);
    }
  };

  useEffect(() => {
    setIsCorrectName(false);
    clearSingleValue();
  }, [submitSuccessCount]);

  const clearSingleValue = () => {
    selectInputRef?.clearValue();
  };

  console.log("render search");
  return (
    <div>
      <AsyncCreatableSelect
        ref={(ref) => {
          selectInputRef = ref;
        }}
        components={{ Option: CustomOption }}
        placeholder=""
        styles={customerStyles}
        loadOptions={customPromiseOptions}
        onChange={onHandleChange}
        formatOptionLabel={(option: any) => {
          return option.userName ? option.userName : option["value"];
        }}
      />
       {isCorrectName ? <CheckMarkIcon height={16} width={16} /> : null}
    </div>
  );
};
