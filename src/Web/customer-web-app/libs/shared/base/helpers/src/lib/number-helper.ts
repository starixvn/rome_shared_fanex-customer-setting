export const NumberHelper = {
  formatWithCommas(value: number, maximumFractionDigits = 2) {
    const formatter = Intl.NumberFormat('en-US', { maximumFractionDigits: maximumFractionDigits });
    return formatter.format(value);
  },
  parseNumber(value: string) {
    if (value) {
      return parseFloat(value.replace(/,/g, ''));
    }

    return undefined;
  },
  roundDownNumber(value: number) {
    return (Math.trunc(value * 100) / 100).toLocaleString('en-US');
  },
};

export const inputNumberOnly = (value: string) => {
  return value.replace(/[^0-9]/g, '');
}

