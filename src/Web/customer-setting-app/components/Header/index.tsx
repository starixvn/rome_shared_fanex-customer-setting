import styles from "@/components/Header/Header.module.css";

interface HeaderPros {
  headerContent: string;
}

const Header = ({ headerContent }: HeaderPros) => {
  return (
    <div className={styles.header}>
      <p className={styles.t}>{headerContent}</p>
    </div>
  );
};

export default Header;
