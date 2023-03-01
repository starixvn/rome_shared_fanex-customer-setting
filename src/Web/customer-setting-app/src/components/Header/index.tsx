import styles from "@components/Header/Header.module.scss";
interface HeaderPros {
  headerContent: string;
}

const Header = ({ headerContent }: HeaderPros) => {
  return <div className={styles.header}>{headerContent}</div>;
};

export default Header;
