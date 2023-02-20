import Router from "next/router";
import { useForm } from "react-hook-form";
import Header from "@/components/Header";
import styles from "@/styles/Home.module.css";

type FormValues = {
  existingAccount: string;
  noOfAccounts: number;
  usernameFirst: number;
  userSecond: string;
  usernameThird: string;
  credit: number;
  password: string;
};

export default function SecurityCode() {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormValues>();

  const onSubmit = handleSubmit((data) => {
    console.log(data);
  });

  return (
    <div className={styles.container}>
      <Header headerContent="Create new Customer"></Header>
      <div className={styles.main}>
        <p className={styles.text}>
          *Note: Settings will remain the same as the Source account setting for
          all accounts
        </p>
        <form onSubmit={onSubmit}>
          <label className={styles.text}>Copy Settings</label>
          <p className={styles.text}>
            Search existing account to copy the same settings
          </p>
          <input {...register("existingAccount")} />
          <label className={styles.text}>No. of Accounts</label>
          <p className={styles.text}>
            Choose the number of accounts you want to create (Max=10)
          </p>
          <input {...register("noOfAccounts")} />
          <label className={styles.text}>Account Details</label>
          <p className={styles.text}>
            Choose an available username as as starter and the remaining
            accounts will follow
          </p>
          <label className={styles.sublabel}>Username*</label>
          <span className={styles.text}>Agent01</span>
          <select {...register("usernameFirst")}>
            <option value="1">1</option>
            <option value="2">2</option>
            <option value="3">3</option>
          </select>
          <select {...register("userSecond")}>
            <option value="A">A</option>
            <option value="B">B</option>
            <option value="C">C</option>
          </select>
          <select {...register("usernameThird")}>
            <option value="A">A</option>
            <option value="B">B</option>
            <option value="C">C</option>
          </select>
          <label className={styles.sublabel}>Credit*</label>
          <input {...register("credit")} />
          <p className={styles.text}>0.00 New Given Credit 150,000.00</p>
          <label className={styles.sublabel}>Password*</label>
          <input {...register("password")} />

          <input className={styles.button} type="submit" />
        </form>
        <button
          onClick={() => {
            Router.push("/securitycode");
          }}
        >
          Back
        </button>
      </div>
    </div>
  );
}
