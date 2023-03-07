import React from "react";
import { Controller, useForm } from "react-hook-form";
import Header from "@components/header";
import { CheckMarkIcon, EyeIcon, InformationIcon } from "@components/icons";
import styles from "./NewCustomer.module.scss";
import {
  CheckUsernameExists,
  SearchCustomerExists,
} from "@components/customer";

export const CreateNewCustomer = ({ props }: any) => {
  const methods = useForm();
  const {
    register,
    handleSubmit,
    formState: { errors },
    control,
  } = methods;

  const onSubmit = handleSubmit((data) => {
    console.log(data);
  });

  return (
    <div className={styles.container}>
      <Header headerContent="Create New Customer"></Header>
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
                <SearchCustomerExists handleChange={onChange} />
              )}
            />
          </div>

          <label className={styles.text}>No. Of Accounts</label>
          <p className={styles.text}>
            Choose the number of accounts you want to create (Max=
            {props.maxAccounts})
          </p>
          <div className={styles.formcontrol}>
            <input className={styles.control} {...register("noOfAccounts")} />
            {/* <CheckMarkIcon height={16} width={16} /> */}
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
                <CheckUsernameExists onChange={onChange} />
              )}
            />
          </div>
          <label className={styles.sublabel}>
            Credit<span className={styles.require}>*</span>
          </label>
          <div className={styles.formcontrol}>
            <input
              className={styles.control + " " + styles.full}
              {...register("credit")}
            />
          </div>
          <div className={styles.credit}>
            <span>{props.minCredit.toFixed(2)}</span> =&lt; New Given Credit
            =&lt; <span>{props.maxCredit.toFixed(2)}</span>
          </div>
          <label className={styles.sublabel}>
            Password
            <span className={styles.require}>*</span>
            <InformationIcon height={16} width={16} />
          </label>
          <div className={styles.relative}>
            <input
              className={styles.control + " " + styles.full}
              {...register("password")}
              type="password"
            />
            <span className={styles.showpassword}>
              <EyeIcon height={16} width={16} />
            </span>
          </div>
          <input
            className={styles.button + " " + styles.submit}
            type="submit"
          />
        </form>
      </div>
    </div>
  );
};
