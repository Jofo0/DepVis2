export const parseTime = (utcString: string) => {
  return new Date(utcString + "Z").toLocaleString();
};
