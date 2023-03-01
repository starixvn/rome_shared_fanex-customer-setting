import { useForm } from "react-hook-form";
import styles from "@styles/Home.module.scss";
import Header from "@components/Header";
import { CheckMarkIcon, EyeIcon, InformationIcon } from "@components/Icons";

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
            <input
              className={styles.control}
              {...register("existingAccount")}
            />
            <CheckMarkIcon height={16} width={16} />
          </div>

          <label className={styles.text}>No. Of Accounts</label>
          <p className={styles.text}>
            Choose the number of accounts you want to create (Max=10)
          </p>
          <div className={styles.formcontrol}>
            <input className={styles.control} {...register("noOfAccounts")} />
            <CheckMarkIcon height={16} width={16} />
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
            <CheckMarkIcon height={16} width={16} />
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
            <span>0.00</span> =&lt; New Given Credit =&lt;{" "}
            <span> 150,000.000</span>
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
            value="create"
          />
        </form>
      </div>
    </div>
  );
}
