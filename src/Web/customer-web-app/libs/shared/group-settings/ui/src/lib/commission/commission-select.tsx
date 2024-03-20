import { ChangeEvent } from 'react';

interface CommissionSelectProps {
  max: number;
  value: number;
  isDisable: boolean;
  onChange: (event: ChangeEvent<HTMLSelectElement>) => void;
}

const multiplier = 10000;

export default function CommissionSelect({ max, value, isDisable = false, onChange }: Readonly<CommissionSelectProps>) {
  const options = [];
  let com = max * multiplier;

  while (com >= 0) {
    options.push(com);
    com -= 5;
  }

  return (
    <select value={value} onChange={onChange} disabled={isDisable}>
      {options.map((option) => (
        <option key={option} value={option / multiplier}>
          {option / multiplier}
        </option>
      ))}
    </select>
  );
}
