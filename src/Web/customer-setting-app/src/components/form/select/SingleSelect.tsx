import { forwardRef, SelectHTMLAttributes } from "react";

export type SelectData = {
  value: number | string;
  displayName: string;
};

type Props = SelectHTMLAttributes<HTMLSelectElement> & {
  options: SelectData[];
};

export const SingleSelect = forwardRef<HTMLSelectElement, Props>(
  ({ options, ...rest }: Props, ref) => {
    return (
      <>
        <select {...rest} ref={ref}>
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
