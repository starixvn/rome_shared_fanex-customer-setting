import useTranslation from "next-translate/useTranslation";
import styles from "@components/Header/Header.module.scss";

interface HeaderPros {
  headerContent: string;
}

const Header = ({ headerContent }: HeaderPros) => {
  const { t } = useTranslation("common");
  return (
    <>
      <div className={styles.header}>{t("header")}</div>
    </>
  );
};

export default Header;
