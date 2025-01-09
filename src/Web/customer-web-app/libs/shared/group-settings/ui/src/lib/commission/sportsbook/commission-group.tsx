import { ChangeEvent } from 'react';

import { Currencies } from '@customer-web-app/shared-constants';

interface Props {
  site: string;
  currency: string;
  value: string;
  onChange: (event: ChangeEvent<HTMLSelectElement>) => void;
  isReadOnly?: boolean;
}

export default function CommissionGroup({ site, currency, value, onChange, isReadOnly = false }: Props) {
  let groups: string[];
  if (site === '76666B') {
    groups = ['a'];
  } else if (currency === Currencies.Won) {
    groups = ['dea', 'deb', 'dec', 'ded'];
  } else {
    groups = ['a', 'b', 'c', 'd', 'hka', 'hkb', 'hkc', 'hkd'];
  }

  return (
    <select value={value} onChange={onChange} disabled={isReadOnly}>
      {isReadOnly ? (
        <option key={value} value={value}>
          {value}
        </option>
      ) : (
        groups.map((group) => (
          <option key={group} value={group}>
            {group}
          </option>
        ))
      )}
    </select>
  );
}
