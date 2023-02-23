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
      Router.push(`/cloneaccount`);
    }
  });

  return (
    <div className={styles.container}>
      <Header headerContent="Create new Customer"></Header>
      <div className={styles.main}>
        <form onSubmit={onSubmit}>
          <label className={styles.text}>Security Code</label>
          <input {...register("securityCode")} placeholder="123457" />
          <input type="submit" />
        </form>
      </div>
    </div>
  );
}
