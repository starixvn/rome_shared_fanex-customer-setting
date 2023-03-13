import { SelectHTMLAttributes } from "react";

export type SelectData = {
  value: number | string;
  displayName: string;
};

type Props = SelectHTMLAttributes<HTMLSelectElement> & {
  options: SelectData[];
  selectedValue: string;
};

export const SingleSelect = ({ options, selectedValue }: Props) => {
  return (
    <>
      <select defaultValue={selectedValue}>
        {options.map((item) => (
          <option key={item.value} value={item.value}>
            {item.displayName}
          </option>
        ))}
      </select>
    </>
  );
};

SingleSelect.displayName = "SingleSelect";
