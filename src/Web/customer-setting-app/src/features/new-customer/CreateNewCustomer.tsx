import React, { useRef, useState } from "react";
import dynamic from "next/dynamic";
import { Controller, FormProvider, useForm } from "react-hook-form";
import styles from "./NewCustomer.module.scss";
import {
  CheckMarkIcon,
  EyeIcon,
  InformationIcon,
} from "@components/icons/Icons";
import { BasicModal } from "@components/modal/BasicModal";

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
  noOfAccounts: string;
  username: string;
  credit: string;
  password: string;
}

export const CreateNewCustomer = ({ props }: any) => {
  const methods = useForm<IFormInputs>();
  const {
    register,
    handleSubmit,
    control,
    reset,
    formState: { errors },
  } = methods;

  const [openModal, setOpenModal] = useState(false);
  const submitSuccessCount = useRef(0);

  const onSubmit = handleSubmit((data) => {
    console.log(data);
    setOpenModal(true);
  });

  const handleRequestClose = () => {
    reset();
    submitSuccessCount.current++;
    console.log(submitSuccessCount);
    setOpenModal(false);
  };

  console.log("create new customer");
  return (
    <>
      <BasicModal
        isOpen={openModal}
        handleRequestClose={handleRequestClose}
        data={[1, 2, 3, 4, 5]}
      />
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
                  submitSuccessCount={submitSuccessCount.current}
                />
              )}
              // rules={{ required: true }}
            />
          </div>

          <label className={styles.text}>No. Of Accounts</label>
          <p className={styles.text}>
            Choose the number of accounts you want to create (Max=
            {props.maxAccounts})
          </p>
          <div className={styles.formcontrol}>
            <input
              className={styles.control}
              {...register("noOfAccounts", {
                required: true,
              })}
            />
            {/* {errors.noOfAccounts && <CheckMarkIcon height={16} width={16} />} */}
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
                <NewAccountName onChange={onChange} />
              )}
              rules={{ required: true }}
            />
            {errors.username && <CheckMarkIcon height={16} width={16} />}
          </div>
          <label className={styles.sublabel}>
            Credit<span className={styles.require}>*</span>
          </label>
          <div className={styles.formcontrol}>
            <input
              className={styles.control + " " + styles.full}
              {...register("credit", {
                required: true,
              })}
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
              {...register("password", {
                required: true,
              })}
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
    </>
  );
};
