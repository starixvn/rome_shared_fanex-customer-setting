import React from "react";
import styles from "./Layout.module.scss";

type Props = {
  children: React.ReactNode;
  title: string;
};

const Layout: React.FC<Props> = ({ children, title }) => {
  return (
    <div className={styles.container}>
      <div className={styles.header}>{title}</div>
      <div className={styles.main}>{children}</div>
    </div>
  );
};

export default Layout;
