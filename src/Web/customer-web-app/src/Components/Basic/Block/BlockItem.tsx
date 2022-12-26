import React from "react";
import { IBlockItem } from "./BlockItem.d";
import Styles from "./BlockItem.module.scss";

export const BlockItem: React.FC<IBlockItem.IProps> = (props) => {
  const { label, value, isEven, isLast } = props;
  return (
    <div
      className={`${Styles.item} ${isEven ? Styles.even : ""} ${
        isLast ? Styles.last : ""
      }`}
    >
      <div className={Styles.label}>{label}</div>
      <div className={Styles.value}>{value}</div>
    </div>
  );
};
