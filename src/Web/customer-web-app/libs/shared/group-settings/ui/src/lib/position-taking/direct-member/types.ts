export type Props = {
  isReadOnly: boolean;
  onPTChange: (sportTypeId: number, betTypeId: number, data: { [x: string]: number | boolean | undefined }) => void;
};
