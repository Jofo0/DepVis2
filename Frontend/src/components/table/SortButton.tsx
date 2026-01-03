import { ArrowUpDown, ArrowUp, ArrowDown } from "lucide-react";
import { Button } from "../ui/button";
import type { Column } from "@tanstack/react-table";

export type SortButtonProps<T> = {
  column: Column<T, unknown>;
};

export const SortButton = <T,>({ column }: SortButtonProps<T>) => {
  return (
    <Button
      variant="ghost"
      onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
    >
      {!column.getIsSorted() ? (
        <ArrowUpDown className="ml-2 h-4 w-4" />
      ) : column.getIsSorted() === "asc" ? (
        <ArrowUp className="ml-2 h-4 w-4" />
      ) : (
        <ArrowDown className="ml-2 h-4 w-4" />
      )}
    </Button>
  );
};
