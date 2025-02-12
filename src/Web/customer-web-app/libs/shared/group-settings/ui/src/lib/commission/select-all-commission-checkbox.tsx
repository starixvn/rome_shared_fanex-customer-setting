import { useTranslate } from '@tolgee/react';

type Props = {
  checked: boolean;
  handleChange: (isChecked: boolean) => void;
};

export default function SelectAllCommissionCheckbox({ checked, handleChange }: Readonly<Props>) {
  const { t } = useTranslate();
  return (
    <>
      <input type="checkbox" style={{ top: '0' }} onChange={(e) => handleChange(e.target.checked)} checked={checked} />
      <div>{t('SelectAll')}</div>
    </>
  );
}
