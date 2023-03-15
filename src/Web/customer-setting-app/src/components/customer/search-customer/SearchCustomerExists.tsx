import { NewCustomerContext } from "@features/new-customer";
import React, { useContext, useEffect } from "react";
import { components, GroupBase, OptionProps } from "react-select";
import AsyncCreatableSelect from "react-select/async-creatable";
import Select from "react-select/dist/declarations/src/Select";
import { customerStyles, customPromiseOptions } from "./CustomSelect";

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

export const SearchCustomerExists = ({ onChange, submitSuccessCount }: any) => {
  let selectInputRef: Select<any, boolean, GroupBase<any>> | null = null;
  const { setCopyCustomerName } = useContext(NewCustomerContext);

  const onHandleChange = (option: any) => {
    if (option) {
      onChange(option.value);
      !option.__isNew__
        ? setCopyCustomerName(option.value)
        : setCopyCustomerName("");
    } else {
      setCopyCustomerName("");
    }
  };

  useEffect(() => {
    if (submitSuccessCount > 0) {
      clearSingleValue();
      setCopyCustomerName("");
    }
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
    </div>
  );
};
