import type { SortingState } from "@tanstack/react-table";

export const toODataOrderBy = (sorting: SortingState): string => {
  if (!sorting?.length) return "";
  const orderBy = sorting
    .map((s) => `${s.id} ${s.desc ? "desc" : "asc"}`)
    .join(", ");
  return `$orderby=${encodeURIComponent(orderBy)}`;
};
