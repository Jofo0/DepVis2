import type { ColumnFiltersState, SortingState } from "@tanstack/react-table";

export const toODataOrderBy = (sorting: SortingState): string => {
  if (!sorting?.length) return "";
  const orderBy = sorting
    .map((s) => `${s.id} ${s.desc ? "desc" : "asc"}`)
    .join(", ");
  return `$orderby=${encodeURIComponent(orderBy)}`;
};

export const toODataFilter = (filters: ColumnFiltersState) => {
  const parts = filters
    .filter((f) => f.value != null && String(f.value).trim() !== "")
    .map((f) => {
      const v = String(f.value).trim().replace(/'/g, "''");
      return `contains(tolower(${f.id}), tolower('${v}'))`;
    });

  return parts.length ? `${parts.join(" and ")}` : "";
};

export const joinODataFilters = (filters: string[]): string => {
  const joinedFilers = filters.filter(Boolean).join(" and ");
  const filter = joinedFilers.length > 0 ? `$filter=${joinedFilers}` : "";
  return filter;
};
