import Router from "next/router";
import { useForm } from "react-hook-form";
import Header from "@/components/Header";
import styles from "@/styles/Home.module.css";

type FormValues = {
  securityCode: number;
};

export default function SecurityCode() {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormValues>();

  const onSubmit = handleSubmit((data) => {
    console.log(data.securityCode);
    if (data.securityCode == 123457) {
      Router.push(`/customer/cloneaccount`);
    }
  });
 
  return (
    <div className={styles.container}>
      <Header headerContent="Create New Customer"></Header>
      <div className={styles.main}>
        <form onSubmit={onSubmit}>
          <label className={styles.text}>Security Code</label>
          <p>To sure your valuable data, please input your Security Code used on the agency site before proceeding!</p>
          <div className={styles.controlsecuritycode}>
            <input className={styles.control} {...register("securityCode")} placeholder="123457" />
            <input type="submit" value="Verify" />
          </div>
          <label className={styles.showcode}>
            <input type="checkbox" />
            <span>Show security code</span>
          </label>
        </form>
      </div>
    </div>
  );
}
