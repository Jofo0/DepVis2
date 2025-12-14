export const buildOdata = (filters: Record<string, string>): string => {
  const odataFilters = Object.entries(filters)
    .filter(([, value]) => value.length > 0)
    .map(([key, value]) => {
      return `${key} eq '${value}'`;
    })
    .join(" and ");

  return odataFilters ? `?$filter=${odataFilters}` : "?";
};
