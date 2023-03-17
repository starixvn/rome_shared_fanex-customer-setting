// import Router from "next/router";
// import { useForm } from "react-hook-form";
// import useTranslation from "next-translate/useTranslation";
// import styles from "@styles/Home.module.scss";
import Layout from "@components/Layout";
import SecurityCodeForm from "@components/SecurityCodeForm/Index";

// type FormValues = {
//   securityCode: number;
// };

export default function SecurityCode() {
  // const { t } = useTranslation("common");
  // const {
  //   register,
  //   handleSubmit,
  //   formState: { errors },
  // } = useForm<FormValues>();

  // const onSubmit = handleSubmit((data) => {
  //   console.log(data.securityCode);
  //   if (data.securityCode == 123457) {
  //     Router.push(`/customer/newcustomer`);
  //   }
  // });

  return (
    <Layout title="New Customer">
      <SecurityCodeForm username="Master1" />
    </Layout>
  );
}
