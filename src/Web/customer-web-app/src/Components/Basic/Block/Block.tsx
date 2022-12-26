import React from "react";

import { IBlock } from "./Block.d";
import styles from "./Block.module.scss";

export const Block: React.FunctionComponent<IBlock.IProps> = (props) => {
  return (
    <div className={`${styles.block} ${props.className}`}>
      <div className={styles.header}>{props.title}</div>
      {props.children}
    </div>
  );
};
