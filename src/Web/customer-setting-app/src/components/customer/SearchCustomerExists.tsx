import React, { useContext, useState } from "react";
import { components, OptionProps } from "react-select";
import AsyncCreatableSelect from "react-select/async-creatable";
import { AppContext } from "src/pages/newcustomer";

const getCustomers = async (query: string) => {
  const url = `${process.env.NEXT_PUBLIC_CUSTOMER_SETTING_APP}/customer/search?query=${query}`;
  const res = await fetch(url);
  const result = await res.json();
  return result;
};

const promiseOptions = async (inputValue: string) => {
  const data = await getCustomers(inputValue);
  return data.map((item: any) => {
    return {
      ...item,
      value: item.userName,
    };
  });
};

const customerStyle = {
  control: (baseStyles: any, state: any) => ({
    ...baseStyles,
    minHeight: "30px",
    height: "30px",
    width: "320px",
    display: "inline-block",
    paddingLeft: "3px",
  }),
  container: (provided: any, state: any) => ({
    ...provided,
    display: "inline-block",
  }),

  valueContainer: (provided: any, state: any) => ({
    ...provided,
    height: "30px",
    padding: "0 6px",
  }),

  input: (provided: any, state: any) => ({
    ...provided,
    margin: "0px 2px",
  }),

  indicatorSeparator: (state: any) => ({
    display: "none",
  }),
  indicatorsContainer: (provided: any, state: any) => ({
    ...provided,
    display: "none",
  }),
};

const Option = (props: OptionProps<any>) => {
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
  handleChange,
  handleCorrectAccount,
}: any) => {
  const { setExistAccountName } = useContext(AppContext);
  const onChange = (option: any, actionMeta: any) => {
    handleChange(option.value);
    handleCorrectAccount(option.value);
    setExistAccountName(option.value);
  };

  console.log("render search");
  return (
    <div>
      <AsyncCreatableSelect
        components={{ Option }}
        instanceId={1}
        placeholder=""
        styles={customerStyle}
        loadOptions={promiseOptions}
        onChange={onChange}
        formatOptionLabel={(option: any) => {
          return option.userName ? option.userName : option["value"];
        }}
      />
      {/* <span>
        <CheckMarkIcon height={16} width={16} />
      </span> */}
    </div>
  );
};
