import { Block, BlockItem } from "@Components/Basic/Block";
import * as React from "react";
import { useTranslation } from "next-i18next";

import Styles from "./CustomerInfo.module.scss";

export const CustomerInfo: React.FC = () => {
  const { t } = useTranslation("common");
  return (
    <div className={Styles["customer-info"]}>
      <Block title={t("general-information")}>
        <BlockItem label={t("username")} value={"supertest007"} />
        <BlockItem label={t("phone")} value={"---"} isEven />
        <BlockItem label={t("first-name")} value={"---"} />
        <BlockItem label={t("mobile-phone")} value={"---"} isEven />
        <BlockItem label={t("last-name")} value={"---"} isLast />
        <BlockItem label={t("fax")} value={"---"} isEven isLast />
      </Block>
    </div>
  );
};
