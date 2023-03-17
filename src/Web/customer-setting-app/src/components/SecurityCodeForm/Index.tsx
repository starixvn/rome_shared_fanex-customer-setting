import useTranslation from "next-translate/useTranslation";
import React, { useState } from "react";
import styles from "./SecurityCode.module.scss";

const SecurityCodeForm = ({ username }: { username: string }) => {
  const { t } = useTranslation("common");
  const [securityCodeShown, setSecurityCodeShown] = useState(false);

  const showSecurityCodeHandler = () => {
    setSecurityCodeShown(!securityCodeShown);
  };

  return (
    <form className={styles["security-code"]}>
      <label htmlFor="SecurityCode">{t("SecurityCode")}</label>
      <p>
        {t("Hi")} {username},
      </p>
      <p>{t("SCNote")}</p>
      <div className={styles["form-group"]}>
        <input
          className={styles["form-control"]}
          id="SecurityCode"
          name="SecurityCode"
          type={securityCodeShown ? "text" : "password"}
          pattern="[0-9]*"
          autoComplete="off"
          maxLength={6}
        />
        <input type="submit" value={t("Verify")} />
      </div>
      <div className={styles["form-group"]}>
        <input type="checkbox" id="checkbox" checked={securityCodeShown} onChange={showSecurityCodeHandler} />
        <label htmlFor="checkbox">{t("ShowSecurityCode")}</label>
      </div>
    </form>
  );
};

export default SecurityCodeForm;
