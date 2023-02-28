import styles from "@/components/Header/Header.module.css";

interface HeaderPros {
  headerContent: string;
}

const Header = ({ headerContent }: HeaderPros) => {
  return (
    <div className={styles.header}>
      {headerContent}
    </div>
  );
};

export default Header;
