import { useTranslation } from 'react-i18next';

type Props = {
  checked: boolean;
  handleChange: (isChecked: boolean) => void;
};

export default function SelectAllCommissionCheckbox({ checked, handleChange }: Readonly<Props>) {
  const { t } = useTranslation();
  return (
    <>
      <input type="checkbox" style={{ top: '0' }} onChange={(e) => handleChange(e.target.checked)} checked={checked} />
      <div>{t('SelectAll')}</div>
    </>
  );
}
