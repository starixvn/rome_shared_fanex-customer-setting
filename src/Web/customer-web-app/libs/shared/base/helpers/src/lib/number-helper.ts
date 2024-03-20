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
};
