import { forwardRef, SelectHTMLAttributes } from "react";

export type SelectData = {
  value: number | string;
  displayName: string;
};

type Props = SelectHTMLAttributes<HTMLSelectElement> & {
  options: SelectData[];
  selectedValue: string;
};

export const SingleSelect = forwardRef<HTMLSelectElement, Props>(
  ({ options, selectedValue, ...rest }: Props, ref) => {
    return (
      <>
        <select {...rest} ref={ref} defaultValue={selectedValue}>
          {options.map((item) => (
            <option key={item.value} value={item.value}>
              {item.displayName}
            </option>
          ))}
        </select>
      </>
    );
  }
);

SingleSelect.displayName = "SingleSelect";
