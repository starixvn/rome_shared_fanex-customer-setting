import React, { useState } from "react";
import dynamic from "next/dynamic";
import { Controller, useForm } from "react-hook-form";
import styles from "./NewCustomer.module.scss";
import {
  InformationIcon,
} from "@components/icons/Icons";

const NewAccountName = dynamic(
  () => import("@components/customer").then((mod) => mod.NewAccountName),
  {
    ssr: false,
  }
);

const SearchCustomerExists = dynamic(
  () => import("@components/customer").then((mod) => mod.SearchCustomerExists),
  {
    ssr: false,
  }
);
interface IFormInputs {
  existAccount: string;
  noOfAccounts: number;
  username: string;
  memberMaxCredit?: number;
  credit: number;
  password: string;
}

type Result = {
  usernames?: string[],
  code?: number,
  message?: string,
  isSuccessful?: boolean
  credit?: number,
  failAccounts?: number
}

export const CreateNewCustomer = ({ props }: any) => {
  const methods = useForm<IFormInputs>();
  const { register, handleSubmit, control, reset, formState, resetField } = methods;

  const [data, setData] = useState<Result>({});
  const [maxCredit, setMaxCredit] = useState(0);
  const [maxMemberCredit, setMaxMemberCredit] = useState(0);

  const onSubmit = handleSubmit(async (formData) => {
    const url = `${process.env.NEXT_PUBLIC_CUSTOMER_SETTING_APP}/customer/copy`;
    const result = await fetch(url, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        username: formData.username,
        password: formData.password,
        credit: Number(formData.credit),
        maxCredit: Number(formData.memberMaxCredit) || 0,
        sourceUserName: formData.existAccount,
        numberOfAccounts: Number(formData.noOfAccounts)
      }),
    });
    const data = await result.json();
    setData(data);
  });

  const handleChangeMaxCredit = (value: number) => {
    setMaxCredit(value);
  };

  const handleOnBlur = (e: any) => {
    if (Number(e.target.value) !== maxMemberCredit) {
      setMaxMemberCredit(Number(e.target.value));
    }
  };

  const handleContinue = () => { 
    setData({});
    resetField("credit");
    reset();
  }

  const renderResult = (data: Result) => {
    return <>
      <div>The following accounts have been created successful:</div>
      {data?.usernames && data?.usernames.length > 0 ? (
        <>
          <div>
            <span>Usernames: </span>
            {data.usernames?.map(item => <span key={item}>{item}, </span>)}
          </div>
          <div>
            <span>Credit: {data.credit} (per account)</span>
          </div>
        </>
      ) : null}
      {data?.failAccounts && data.failAccounts > 0 ? (<>
        <div>{data.failAccounts} accounts have been created failed.</div>
        <div>
          <span>Failed reason: {data.message}</span>
        </div>
      </>
      ) : null}
      <button onClick={handleContinue}>Create More</button>
    </>
  }

  console.log("create new customer");
  return (
    <>
      {JSON.stringify(data) !== "{}" ? renderResult(data) : (
        <div className={styles.main}>
        <p className={styles.note + " " + styles.notetop}>
          *Note: Settings will remain the same as the Source account setting for
          all accounts
        </p>
        <form onSubmit={onSubmit}>
          <label className={styles.text}>Copy Settings</label>
          <p className={styles.text}>
            Search existing account to copy the same settings
          </p>
          <div className={styles.formcontrol}>
            <Controller
              name="existAccount"
              control={control}
              render={({ field: { onChange } }) => (
                <SearchCustomerExists
                  onChange={onChange}
                  changeMaxCredit={handleChangeMaxCredit}
                />
              )}
              rules={{ required: true }}
            />
          </div>

          <label className={styles.text}>No. Of Accounts</label>
          <p className={styles.text}>
            Choose the number of accounts you want to create (Max=
            {props.maxAccounts})
          </p>
          <div className={styles.formcontrol}>
            <input
              type="number"
              className={styles.control}
              {...register("noOfAccounts", {
                required: true,
                min: 1,
                max: props.maxAccounts,
              })}
            />
          </div>
          <label className={styles.text}>Account Details</label>
          <p className={styles.text}>
            Choose an available username as as starter and the remaining
            accounts will follow
          </p>
          <label className={styles.sublabel}>
            Username<span className={styles.require}>*</span>
          </label>
          <div className={styles.agent}>
            <Controller
              name="username"
              control={control}
              render={({ field: { onChange } }) => (
                <NewAccountName
                  actorName={props.actorName}
                  onChange={onChange}
                />
              )}
              rules={{ required: true }}
            />
          </div>
          <label className={styles.sublabel}>
            Credit<span className={styles.require}>*</span>
          </label>
          <div className={styles.formcontrol}>
            <input
              type="number"
              className={styles.control + " " + styles.full}
              {...register("credit", {
                min: 0,
                max: maxCredit,
                required: true,
              })}
              onBlur={handleOnBlur}
              />
              <span>(per account)</span>
          </div>
          <div className={styles.credit}>
            <span>{props.minCredit.toFixed(2)}</span> =&lt; New Given Credit
            =&lt; <span>{maxCredit.toFixed(2)}</span>
          </div>
          {props.isMasterLevel ? (
            <div>
              <label className={styles.sublabel}>
                Member Max Credit<span className={styles.require}>*</span>
              </label>
              <input
                type="number"
                {...register("memberMaxCredit", {
                  required: true,
                  max: maxMemberCredit,
                })}
                />
                <span>(per account)</span>
              <div className={styles.credit}>
                <span>=&lt; {maxMemberCredit.toFixed(2)}</span>
              </div>
            </div>
          ) : null}
          <label className={styles.sublabel}>
            Password
            <span className={styles.require}>*</span>
            <InformationIcon height={16} width={16} />
          </label>
          <div className={styles.relative}>
            <input
              className={styles.control + " " + styles.full}
              {...register("password", {
                required: true,
              })}
              type="password"
            />
            <span className={styles.showpassword}></span>
          </div>
          <input
            className={styles.button + " " + styles.submit}
            type="submit"
          />
        </form>
      </div>
      )}
    </>
  );
};
